using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.Models.Enums;
using SBEU.Tasklet.Models.Responses;

using Swashbuckle.AspNetCore.Annotations;

namespace SBEU.Tasklet.Api.Controllers
{
    [Route("[controller]")]
    public class ChartsController : ControllerExt
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        public ChartsController(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [SwaggerResponse(200,"Total tasks chart with all users",typeof(TotalChartDto))]
        [HttpGet("userstotal")]
        public async Task<IActionResult> GetUsersTotal()
        {
            var allTasks = _context.XTasks.ToList();
            var users = _context.Users.Select(_mapper.Map<UserDto>);

            var chart = new TotalChartDto
            {
                Chart = users.Select(t =>
            {
                var userTasks = allTasks.Where(x => x.Executor.Id == t.Id).ToList();
                return new UserChartDto
                {
                    User = t,
                    Chart = new ChartDto
                    {
                        TotalTaskCount = (uint)userTasks.Count,
                        CompletedTaskCount = (uint)userTasks.Where(s => s.Status == TaskProgress.Done).ToList().Count,
                        ClosedTaskCount = (uint)userTasks.Where(s => s.Status == TaskProgress.Closed).ToList().Count,
                        AuthoredTaskCount = (uint)allTasks.Where(x => x.Author.Id == t.Id).ToList().Count,
                        TotalPrice = (uint)userTasks.Where(x=>x.Status==TaskProgress.Done).Sum(x => x.Price)
                    }
                };
            }).ToList()
            };

            return Json(chart);
        }

        [SwaggerResponse(200, "Total timed tasks chart with all users", typeof(List<TimedUserChartDto>))]
        [HttpGet("timeduserstotal")]
        public async Task<IActionResult> GetTimedChart()
        {
            var allTasks = _context.XTasks.Include(x => x.Author).Include(x => x.Executor).ToList();
            var users = _context.Users.ToList().Select(_mapper.Map<UserDto>);

            var chart = users.Select(t =>
                {
                    var userTasks = allTasks.Where(x => x.Executor.Id == t.Id && x.EndTime!=null);
                    return new TimedUserChartDto
                    {
                        User = t,
                        Chart = userTasks.GroupBy(x => x.EndTime.Value.Date).Select((group)=> new TimedChartDto
                        {
                            Date = DateOnly.FromDateTime(group.Key),
                            TotalTaskCount = (uint)userTasks.Where(s=>s.EndTime.Value.Date == group.Key).Count(),
                            CompletedTaskCount = (uint)userTasks.Where(s => s.Status == TaskProgress.Done && s.EndTime.Value.Date == group.Key).ToList().Count,
                            ClosedTaskCount = (uint)userTasks.Where(s => s.Status == TaskProgress.Closed && s.EndTime.Value.Date == group.Key).ToList().Count,
                            AuthoredTaskCount = (uint)allTasks.Where(x => x.Author.Id == t.Id && x.StartTime.Date == group.Key).ToList().Count,
                            TotalPrice = (uint)userTasks.Where(x => x.Status == TaskProgress.Done && x.EndTime.Value.Date == group.Key).Sum(x => x.Price)
                        }).ToList()
                    };
                }).ToList();

            return Json(chart);
        }

        [SwaggerResponse(200,"",typeof(TotalTabledChartDto))]
        [HttpGet("userstabledtotal")]
        public async Task<IActionResult> GetUsersTabletTotal()
        {
            var allTasks = _context.XTasks.Include(x => x.Author).Include(x => x.Executor).Include(x => x.Table).ToList();
            var tables = _context.XTables.Include(x=>x.Users).ToList();
            var chart = new TotalTabledChartDto
            {
                Chart = tables.Where(x=>x.Users.Count>0).Select(t =>
                {
                    var tableTasks = allTasks.Where(a => a.Table.Id == t.Id).ToList();
                    return new TabledChartDto
                    {
                        Table = _mapper.Map<SmallTableDto>(t),
                        Chart = t.Users.Select(s =>
                        {
                            var userTasks = tableTasks.Where(x => x.Executor.Id == s.Id || x.Author.Id == s.Id).ToList();
                            return new UserChartDto
                            {
                                User = _mapper.Map<UserDto>(s),
                                Chart = new ChartDto
                                {
                                    TotalTaskCount = (uint)userTasks.Count,
                                    CompletedTaskCount = (uint)userTasks.Where(a => a.Status == TaskProgress.Done).ToList().Count,
                                    ClosedTaskCount = (uint)userTasks.Where(a => a.Status == TaskProgress.Closed).ToList().Count,
                                    AuthoredTaskCount = (uint)tableTasks.Where(x => x.Author.Id == s.Id).ToList().Count,
                                    TotalPrice = (uint)userTasks.Where(x => x.Status == TaskProgress.Done).Sum(x => x.Price)
                                }
                            };
                        }).ToList()
                    };
                }).ToList()
            };
            return Json(chart);
        }



        [SwaggerResponse(200,"Total chart by task progress",typeof(TotalProgressChartDto))]
        [HttpGet("total")]
        public async Task<IActionResult> GetProgressChart()
        {
            var allTasks = _context.XTasks.Include(x => x.Author).Include(x => x.Executor).ToList();
            var chart = new TotalProgressChartDto
            {
                TotalTaskCount = (uint)allTasks.Count,
                DoneTaskCount = (uint)allTasks.Where(x => x.Status == TaskProgress.Done).ToList().Count,
                DoingTaskCount = (uint)allTasks.Where(x => x.Status == TaskProgress.Doing).ToList().Count,
                ReviewTaskCount = (uint)allTasks.Where(x => x.Status == TaskProgress.Review).ToList().Count,
                ClosedTaskCount = (uint)allTasks.Where(x => x.Status == TaskProgress.Closed).ToList().Count,
                NewTaskCount = (uint)allTasks.Where(x => x.Status == TaskProgress.New).ToList().Count,
            };
            return Json(chart);
        }
    }
}
