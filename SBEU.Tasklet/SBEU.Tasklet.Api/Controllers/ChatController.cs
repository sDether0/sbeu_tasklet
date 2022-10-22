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

        [SwaggerResponse(200,"",typeof(IEnumerable<ChatDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _context.Users.Include(x => x.Chats).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var chatsDto = user.Chats.Select(_mapper.Map<ChatDto>);
            foreach (var chat in chatsDto)
            {
                if (chat.Private)
                {
                    chat.Title = user.Chats!.First(x => x.Id == chat.Id)!.Users!.First(x => x.Id != user.Id)!.UserName!;
                }
            }
            return Json(chatsDto);
        }

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
        public async Task<IActionResult> CreateChat([FromBody]CreateChatRequest request)
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
        public async Task<IActionResult> SendMessage([FromBody]MessageRequest request)
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
            await _chatHub.Clients.Group(mess.Chat.Id).Message(messDto);
            return Ok();
        }
        
    }
}
