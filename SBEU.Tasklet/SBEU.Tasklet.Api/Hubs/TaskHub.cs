using AutoMapper;

using Microsoft.AspNetCore.SignalR;

using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.Models.Responses;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SBEU.Tasklet.Api.Hubs
{
    public interface IXTask
    {
        public Task UpdateTask(TaskDto task);
        public Task NewTask(TaskDto task);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskHub : Hub<IXTask>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public TaskHub(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override Task OnConnectedAsync()
        {
            var tables = _context.Users.Include(x => x.Tables).FirstOrDefault(x => x.Id == UserId)?.Tables
                .Select(x => x.Id) ?? Enumerable.Empty<string>();
            foreach (var tbl in tables)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, tbl);
            }
            return base.OnConnectedAsync();
        }

        public string? UserId
        {
            get
            {
                var claimIdentity = this.Context?.User?.Identity as ClaimsIdentity;
                var userId = claimIdentity?.Claims.First(x => x.Type == "Id").Value;
                return userId;
            }
        }
    }
}
