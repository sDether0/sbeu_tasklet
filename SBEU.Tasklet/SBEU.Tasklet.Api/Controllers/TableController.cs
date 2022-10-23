using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

using Swashbuckle.AspNetCore.Annotations;

namespace SBEU.Tasklet.Api.Controllers
{
    [Route("[controller]")]
    public class TableController : ControllerExt
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;


        public TableController(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<TableDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = _context.Users.Include(x => x.Tables).ThenInclude(x=>x.Users).FirstOrDefault(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var tableDto = user.Tables.Select(_mapper.Map<TableDto>);
            return Json(tableDto);
        }

        [SwaggerResponse(200, "", typeof(TableDto))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTableRequest request)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var table = _mapper.Map<XTable>(request);
            table.Id = Guid.NewGuid().ToString();
            table.Users = table.Users.Select(x => x.Id.Get<XIdentityUser>(_context)).ToList();
            if (!table.Users.Contains(user))
            {
                table.Users.Add(user);
            }
            await _context.XTables.AddAsync(table);
            await _context.SaveChangesAsync();
            var tableDto = _mapper.Map<TableDto>(table);
            return Json(tableDto);
        }

        [SwaggerResponse(200, "", typeof(TableDto))]
        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateTableRequest request)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var table = _context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == request.Id);

            if (table == null)
            {
                return NotFound();
            }
            if (!table.Users.Contains(user))
            {
                return Unauthorized();
            }
            table.Title = request.Title ?? table.Title;
            if (request.Users != null)
            {
                foreach (var usr in request.Users)
                {
                    var usrr = await _context.Users.FindAsync(usr.Id);
                    if (usrr != null)
                    {
                        table.Users.Add(usrr);
                    }
                }
            }
            _context.Update(table);
            await _context.SaveChangesAsync();
            var tableDto = _mapper.Map<TableDto>(table);
            return Json(tableDto);
        }


        [HttpPost("adduser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserToTableRequest request)
        {
            var user = await _context.Users.Include(x => x.Tables).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var addUser = await _context.Users.FindAsync(request.UserId);
            if (addUser == null)
            {
                return NotFound("User was not found");
            }

            if (user.Tables.Any(x => x.Id == request.TableId))
            {
                var table = await _context.XTables.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == request.TableId);
                if (!table.Users.Any(x => x.Id == request.UserId))
                {
                    table.Users.Add(addUser);
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                return Ok("Already on table");
            }

            return Unauthorized("Not enough permission");
        }
    }
}
