using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Api.Repositories.Interfaces;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.Models.Responses;
using Microsoft.AspNetCore.SignalR;
using SBEU.Tasklet.Api.Hubs;

namespace SBEU.Tasklet.Api.Repositories
{
    public class MessageRepository : BaseRepository<XMessage>, IMessageRepository
    {

        private readonly IHubContext<ChatHub, IChat> _chatHub;
        public MessageRepository(ApiDbContext context, IHubContext<ChatHub, IChat> chatHub) : base(context)
        {
            _chatHub = chatHub;
        }


        public async Task<XMessage> CreateAsync(XMessage mess, XIdentityUser user)
        {
            mess.Chat = mess.Chat.Id.Get<Chat>(_context);
            mess.From = user;
            mess.Table = mess.Table.Id?.Get<XTable>(_context);
            mess.Task = mess.Task.Id?.Get<XTask>(_context);
            mess.Time = DateTime.Now;
            
            await CreateAsync(mess);
            
            if (mess.Chat.Private && _context.Chats.First(x => x.Id == mess.Chat.Id).Users
                    .First(x => x.Id != user.Id).IsPushOn)
            {
                await Notifier.FullMessageNotify(new[]{
                        _context.Chats.First(x => x.Id == mess.Chat.Id).Users.First(x => x.Id != user.Id)
                            .PushToken }.Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().ToList(),
                    user.UserName,
                    mess.Text,
                    _chatHub,
                    mess);
            }
            else
            {
                await Notifier.FullMessageNotify(
                    _context.Chats.First(x => x.Id == mess.Chat.Id).Users.Where(x => x.IsPushOn)
                        .Select(x => x.PushToken).Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().ToList(), 
                    mess.Chat.Title,
                    mess.Text,
                    _chatHub,
                    mess);
            }

            return mess;
        }
    }
}
