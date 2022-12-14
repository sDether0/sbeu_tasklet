using AutoMapper;

using FirebaseAdmin.Messaging;

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
    public class ChatController : ControllerExt
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub, IChat> _chatHub;

        public ChatController(ApiDbContext context, IMapper mapper, IHubContext<ChatHub, IChat> chatHub)
        {
            _context = context;
            _mapper = mapper;
            _chatHub = chatHub;
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<ChatDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _context.Users.Include(x => x.Chats).ThenInclude(x=>x.Users).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var chatsDto = user.Chats.Select(_mapper.Map<ChatDto>).ToList();
            for (var i=0;i<chatsDto.Count;i++)
            {
                if (chatsDto[i].Private)
                {
                    chatsDto[i].Title = user.Chats!.First(x => x.Id == chatsDto[i].Id)!.Users!.First(x => x.Id != user.Id)!.UserName!;
                }

                chatsDto[i].LastMessage = _context.Messages?.Where(x => x.Chat.Id == chatsDto[i].Id)?.OrderByDescending(x => x.Time)?
                    .FirstOrDefault()?.Text??"There are no messages yet";
            }
            return Json(chatsDto);
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<FullChatDto>))]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetById(string chatId)
        {
            var user = await _context.Users.Include(x => x.Chats).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var chat = user.Chats.FirstOrDefault(x => x.Id == chatId);
            if (chat == null)
            {
                return NotFound("Chat not found or not accessable");
            }
            var chatDto = _mapper.Map<FullChatDto>(chat);
            if (chatDto.Private)
            {
                chatDto.Users = chatDto.Users.Where(x => x.Id != user.Id).ToList();
                chatDto.Title = chatDto.Users.First(x => x.Id != user.Id).UserName;
            }
            return Json(chatDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var chat = new Chat()
            {
                Id = Guid.NewGuid().ToString(),
                Private = request.Private,
                Title = request.Title
            };
            chat.Users.Add(user);
            if (request.Private)
            {
                if (request.UserIds.Count > 1)
                {
                    return BadRequest();
                }
                var usr = await _context.Users.FindAsync(request.UserIds.First());
                if (usr == null)
                {
                    return NotFound();
                }
                chat.Users.Add(usr);
            }
            else
            {
                var users = _context.Users.Where(x => request.UserIds.Contains(x.Id));
                foreach (var usr in users)
                {
                    chat.Users.Add(usr);
                }
            }

            await _context.AddAsync(chat);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var mess = _mapper.Map<XMessage>(request);
            mess.Chat = request.ChatId.Get<Chat>(_context);
            mess.From = user;
            mess.Table = request.TableId?.Get<XTable>(_context);
            mess.Task = request.TaskId?.Get<XTask>(_context);
            mess.Time = DateTime.Now;
            await _context.Messages.AddAsync(mess);
            await _context.SaveChangesAsync();
            var messDto = _mapper.Map<MessageDto>(mess);

            if (mess.Chat.Private && _context.Chats.Include(x => x.Users).First(x => x.Id == mess.Chat.Id).Users
                    .First(x => x.Id != user.Id).IsPushOn)
            {
                await Send(
                    _context.Chats.Include(x => x.Users).First(x => x.Id == mess.Chat.Id).Users
                        .Select(x => x.PushToken).ToList(),
                    _context.Chats.Include(x => x.Users).First(x => x.Id == mess.Chat.Id).Users
                        .First(x => x.Id != user.Id).UserName, mess.Text);
            }
            else
            {
                await Send(
                    _context.Chats.Include(x => x.Users).First(x => x.Id == mess.Chat.Id).Users.Where(x => x.IsPushOn)
                        .Select(x => x.PushToken).ToList(), mess.Chat.Title, mess.Text);
            }


            await _chatHub.Clients.Group(mess.Chat.Id).Message(messDto);
            return Ok();
        }


        private async Task Send(List<string> tokens, string chat, string text)
        {
            MulticastMessage message;
            message = new MulticastMessage()
            {
                Tokens = tokens,
                Data = new Dictionary<string, string>()
                {
                    {"title", chat},
                    {"body", text},
                },
                Notification = new Notification()
                {
                    Title = chat,
                    Body = text
                }
            };
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }
    }
}
