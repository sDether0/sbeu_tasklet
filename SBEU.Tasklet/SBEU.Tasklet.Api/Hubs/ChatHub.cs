using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SBEU.Tasklet.Models.Responses;

using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SBEU.Tasklet.DataLayer.DataBase;

namespace SBEU.Tasklet.Api.Hubs
{
    public interface IChat
    {
        public Task Message(MessageDto message);
        public Task History(IEnumerable<MessageDto> history);
    }
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub<IChat>
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;

        public ChatHub(ApiDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public override Task OnConnectedAsync()
        {
            if (UserId != null)
            {
                var chats = _context.Users.Include(x => x.Chats).FirstOrDefault(x => x.Id==UserId)?.Chats.Select(x => x.Id) ??
                            Enumerable.Empty<string>();
                foreach (var cht in chats)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, cht);
                    var history = _context.Messages.Where(x => x.Chat.Id == cht).OrderByDescending(x=>x.Time).Take(25).ToList();
                    var historyDto = history.Select(_mapper.Map<MessageDto>);
                    Clients.Caller.History(historyDto);
                }
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
