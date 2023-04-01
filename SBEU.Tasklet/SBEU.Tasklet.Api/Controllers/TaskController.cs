using AutoMapper;

using FirebaseAdmin.Messaging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SBEU.Response;
using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Middleware;
using SBEU.Tasklet.Api.Repositories.Interfaces;
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
        private readonly ITaskRepository _tasks;
        private readonly IMapper _mapper;
        private readonly IHubContext<TaskHub, IXTask> _taskHub;
        private readonly CatchMiddlware _catch;

        public TaskController(ApiDbContext context, IMapper mapper, IHubContext<TaskHub, IXTask> taskHub, ITaskRepository tasks, CatchMiddlware @catch)
        {
            _context = context;
            _mapper = mapper;
            _taskHub = taskHub;
            _tasks = tasks;
            _catch = @catch;
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<TaskDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 30)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var tasks = (await _tasks.GetAllAsync()).Skip(skip).Take(take).ToList().Concat(user.AuthoredTasks.ToList()).Distinct().ToList();
            //var tasks = user.Tasks.Concat(user.AuthoredTasks).Distinct().Skip(skip).Take(take);
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
            var user = await _context.Users.Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var task = await _tasks.GetByIdAsync(id);
            //var task = await _context.XTasks.Include(x => x.Author).Include(x => x.Executor).FirstOrDefaultAsync(x => x.Id == id);
            var dto = _mapper.Map<TaskDto>(task);
            dto.IsAuthor = task.Author.Id == UserId;
            dto.IsExecutor = task.Executor.Id == UserId;
            dto.Note = user.Notes.FirstOrDefault(x => x.TaskId == task.Id)?.Text ?? "";
            return Json(dto);
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<TaskDto>))]
        [HttpGet("table/{tableId}")]
        public async Task<IActionResult> GetByTable(string tableId, [FromQuery] int skip = 0, [FromQuery] int take = 30)
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }
            var task = _mapper.Map<XTask>(request);
            var upTask = _mapper.Map<XHistory>(request);
            var result = await _tasks.CreateAsync(task, user, upTask);
            var taskDto = _mapper.Map<TaskDto>(result);

            return Json(taskDto);
        }

        [SwaggerResponse(200, "", typeof(TaskDto))]
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateTaskRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var xTask = _mapper.Map<XTask>(request);
            var result = await _tasks.UpdateAsync(xTask, user);
            var taskDto = _mapper.Map<TaskDto>(result);
            return Json(taskDto);
        }

        [SwaggerResponse(200, "", typeof(BaseResponse<TaskDto>))]
        [HttpPatch("baser")]
        public async Task<BaseResponse<TaskDto>> UpdateB([FromBody] UpdateTaskRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            var xTask =  _mapper.Map<XTask>(request);
            var task = _tasks.UpdateAsync(xTask,user);
            var taskResult = await _catch.Try<XTask,TaskDto>(task, user);
            return taskResult;
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
