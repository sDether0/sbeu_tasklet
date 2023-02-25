using AutoMapper;

using FirebaseAdmin.Messaging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Enums;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

using Swashbuckle.AspNetCore.Annotations;

namespace SBEU.Tasklet.Api.Controllers
{
    [Route("[controller]")]
    public class TaskController : ControllerExt
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<TaskHub, IXTask> _taskHub;

        public TaskController(ApiDbContext context, IMapper mapper, IHubContext<TaskHub, IXTask> taskHub)
        {
            _context = context;
            _mapper = mapper;
            _taskHub = taskHub;
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<TaskDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 30)
        {
            var user = await _context.Users.Include(x => x.Tasks).ThenInclude(x => x.Author)
                .Include(x => x.AuthoredTasks).ThenInclude(x => x.Executor).Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var tasks = user.Tasks.Concat(user.AuthoredTasks).Distinct().Skip(skip).Take(take);
            var tasksDto = tasks.Select(_mapper.Map<TaskDto>).ToList();
            for (var i = 0; i < tasksDto.Count; i++)
            {
                tasksDto[i].Note = user.Notes.FirstOrDefault(x => x.TaskId == tasksDto[i].Id)?.Text ?? "";
                tasksDto[i].IsAuthor = tasksDto[i].Author.Id == user.Id;
                tasksDto[i].IsExecutor = tasksDto[i].Executor.Id == user.Id;
            }
            var tasksHid = tasksDto.Where(x => !x.Hidden || (x.IsAuthor || x.IsExecutor));
            return Json(tasksHid);
        }


        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _context.Users.Include(x=>x.Notes).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var task = await _context.XTasks.Include(x=>x.Author).Include(x=>x.Executor).FirstOrDefaultAsync(x=>x.Id==id);
            var dto = _mapper.Map<TaskDto>(task);
            dto.IsAuthor = task.Author.Id == UserId;
            dto.IsExecutor = task.Executor.Id == UserId;
            dto.Note = user.Notes.FirstOrDefault(x => x.TaskId == task.Id)?.Text ?? "";
            return Json(dto);
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<TaskDto>))]
        [HttpGet("table/{tableId}")]
        public async Task<IActionResult> GetByTable(string tableId, [FromQuery]int skip = 0, [FromQuery]int take = 30)
        {
            var user = await _context.Users
                .Include(x => x.Tables).ThenInclude(x => x.Tasks).ThenInclude(x => x.Executor)
                .Include(x => x.Tables).ThenInclude(x => x.Tasks).ThenInclude(x => x.Author)
                .Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var table = user.Tables.FirstOrDefault(x => x.Id == tableId);
            if (table == null)
            {
                return NotFound();
            }
            var tasks = table.Tasks;
            if (tasks.Count == 0)
            {
                return Json(new object[] { });
            }

            var tasksDto = tasks.Skip(skip).Take(take).Select(_mapper.Map<TaskDto>).ToList();
            for (var i = 0; i < tasksDto.Count; i++)
            {
                tasksDto[i].Note = user.Notes.FirstOrDefault(x => x.TaskId == tasksDto[i].Id)?.Text ?? "";
                tasksDto[i].IsAuthor = tasksDto[i].Author.Id == user.Id;
                tasksDto[i].IsExecutor = tasksDto[i].Executor.Id == user.Id;
            }

            var tasksHid = tasksDto.Where(x => !x.Hidden || (x.IsAuthor || x.IsExecutor));
            return Json(tasksHid);
        }

        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var user = await _context.Users.Include(x => x.Tables).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var task = _mapper.Map<XTask>(request);
            task.Id = Guid.NewGuid().ToString();

            if (!_context.XTables.Any(x => x.Id == task.Table.Id))
            {
                return NotFound("Table not found");
            }

            if (user.Tables.All(x => x.Id != task.Table.Id))
            {
                return Unauthorized("You has not access to that table");
            }
            task.Author = user;
            task.Table = task.Table.Id.Get<XTable>(_context);
            if (_context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)!.Users.All(x => x.Id != task.Executor.Id))
            {
                return BadRequest("User has not access to that table");
            }
            task.Executor = task.Executor.Id.Get<XIdentityUser>(_context);
            task.Status = request.Status ?? TaskProgress.New;
            task.StartTime = request.StartTime ?? DateTime.Now;
            await _context.XTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            var taskDto = _mapper.Map<TaskDto>(task);
            var tableUsers = _context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)?.Users
                .Select(x => x.Id) ?? Enumerable.Empty<string>();
            if (task.Hidden)
            {
                await _taskHub.Clients.Users(task.Author.Id, task.Executor.Id).NewTask(taskDto);
            }
            else
            {
                await _taskHub.Clients.Users(tableUsers).NewTask(taskDto);
            }
            if (task.Executor.IsMailNotify)
            {
                await ConfirmationEmail.SendNotification(task, task.Executor, true);
            }
            if (task.Executor.PushToken != null && task.Executor.IsPushOn)
            {
                MulticastMessage message;
                message = new MulticastMessage()
                {
                    Tokens = new[] { task.Executor.PushToken },
                    Data = new Dictionary<string, string>()
                    {
                        { "title", "У вас новая задача" },
                        { "body", $"{task.Title}" },
                    },
                    Notification = new Notification()
                    {
                        Title = "У вас новая задача",
                        Body = $"{task.Title}"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }

            return Json(taskDto);
        }

        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateTaskRequest request)
        {
            var user = await _context.Users.Include(x => x.Tasks).Include(x => x.AuthoredTasks).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var task = _context.XTasks.Include(x => x.Table).Include(x => x.Executor).Include(x=>x.Author).FirstOrDefault(x => x.Id == request.Id);
            if (user.Tasks.Contains(task) || user.AuthoredTasks.Contains(task))
            {
                var newst = false;
                task.Title = request.Title ?? task.Title;
                task.Description = request.Description ?? task.Description;
                if (request.ExecutorId != null)
                {
                    if (_context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)!.Users.All(x => x.Id != request.ExecutorId))
                    {
                        return BadRequest("User has not access to that table");
                    }
                    task.Executor = request.ExecutorId.Get<XIdentityUser>(_context);
                }
                if (request.Status is { } status)
                {

                    if(task.Status != status)
                    {
                        newst = true;
                    }
                    task.Status = status;
                    if (status == TaskProgress.Done || status == TaskProgress.Closed)
                    {
                        task.EndTime = DateTime.Now;
                    }
                }

                task.Links = request.Links ?? task.Links;

                _context.Update(task);
                await _context.SaveChangesAsync();
                if (newst && (task.Author.Id == UserId ? task.Executor.IsMailNotify : task.Author.IsMailNotify))
                {
                    await ConfirmationEmail.SendNotification(task, task.Author.Id == UserId ? task.Executor : task.Author, false);
                }
                var taskDto = _mapper.Map<TaskDto>(task);
                var tableUsers = _context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)?.Users
                    .Select(x => x.Id) ?? Enumerable.Empty<string>();
                if (task.Hidden)
                {
                    await _taskHub.Clients.Users(task.Author.Id, task.Executor.Id).UpdateTask(taskDto);
                }
                else
                {
                    await _taskHub.Clients.Users(tableUsers).UpdateTask(taskDto);
                }
                return Json(taskDto);
            }

            return Unauthorized();
        }

        [HttpPost("note")]
        public async Task<IActionResult> AddNote([FromBody] NoteRequest request)
        {
            var user = await _context.Users.FindAsync(UserId);
            var task = await _context.XTasks.FindAsync(request.TaskId);
            if (task == null || user == null)
            {
                return NotFound();
            }

            var note = await _context.FindAsync<Note>(UserId, request.TaskId);

            if (note == null)
            {
                note = new Note()
                {
                    UserId = user.Id,
                    TaskId = task.Id,
                    Text = request.Text
                };
                user.Notes.Add(note);
                _context.Update(user);
            }
            else
            {
                note.Text = request.Text;
                _context.Update(note);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
