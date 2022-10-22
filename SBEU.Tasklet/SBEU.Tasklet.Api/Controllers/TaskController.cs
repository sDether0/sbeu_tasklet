using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
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
        public async Task<IActionResult> GetAll()
        {
            var user = await _context.Users.Include(x => x.Tasks).Include(x=>x.AuthoredTasks).Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var tasks = user.Tasks.Concat(user.AuthoredTasks);
            var tasksDto = tasks.Select(_mapper.Map<TaskDto>);
            foreach (var task in tasksDto)
            {
                task.Note = user.Notes.FirstOrDefault(x => x.TaskId == task.Id)?.Text ?? "";
                task.IsAuthor = task.Author.Id == user.Id;
                task.IsExecutor = task.Executor.Id == user.Id;
            }
            return Json(tasksDto);
        }
        
        [SwaggerResponse(200, "", typeof(IEnumerable<TaskDto>))]
        [HttpGet("{tableId}")]
        public async Task<IActionResult> GetByTable(string tableId)
        {
            var user = await _context.Users.Include(x => x.Tables).ThenInclude(x => x.Tasks).Include(x=>x.Notes)
                .FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var tasks = user.Tables.FirstOrDefault(x=>x.Id==tableId)?.Tasks;
            if (tasks == null)
            {
                return NotFound();
            }

            var tasksDto = tasks.Select(_mapper.Map<TaskDto>);
            foreach (var task in tasksDto)
            {
                task.Note = user.Notes.FirstOrDefault(x => x.TaskId == task.Id)?.Text ?? "";
                task.IsAuthor = task.Author.Id==user.Id;
                task.IsExecutor = task.Executor.Id==user.Id;
            }
            return Json(tasksDto);
        }

        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var user = await _context.Users.Include(x=>x.Tables).FirstOrDefaultAsync(x => x.Id == UserId);
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
            
            task.StartTime= request.StartTime??DateTime.Now;
            await _context.XTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            var taskDto = _mapper.Map<TaskDto>(task);
            var tableUsers = _context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)?.Users
                .Select(x => x.Id)??Enumerable.Empty<string>();
            if (task.Hidden)
            {
                await _taskHub.Clients.Users(task.Author.Id, task.Executor.Id).NewTask(taskDto);
            }
            else
            {
                await _taskHub.Clients.Users(tableUsers).NewTask(taskDto);
            }
            return Json(taskDto);
        }

        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody]UpdateTaskRequest request)
        {
            var user = await _context.Users.Include(x=>x.Tasks).Include(x=>x.AuthoredTasks).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var task = request.Id.Get<XTask>(_context);
            if (user.Tasks.Contains(task) || user.AuthoredTasks.Contains(task))
            {
                task.Title = request.Title??task.Title;
                task.Description = request.Description??task.Description;
                if (request.ExecutorId != null)
                {
                    if (_context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)!.Users.All(x => x.Id != request.ExecutorId))
                    {
                        return BadRequest("User has not access to that table");
                    }
                    task.Executor = request.ExecutorId.Get<XIdentityUser>(_context);
                }

                _context.Update(task);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> AddNote([FromBody]NoteRequest request)
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
