using System.ComponentModel.DataAnnotations;
using AutoMapper;

using FirebaseAdmin.Messaging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Repositories.Interfaces;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

using Swashbuckle.AspNetCore.Annotations;

namespace SBEU.Tasklet.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerExt
    {
        private readonly ApiDbContext _context;
        private readonly IMessageRepository _message;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub, IChat> _chatHub;

        public ChatController(ApiDbContext context, IMapper mapper, IHubContext<ChatHub, IChat> chatHub, IMessageRepository message)
        {
            _context = context;
            _mapper = mapper;
            _chatHub = chatHub;
            _message = message;
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<ChatDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }
            var chatsDto = user.Chats.Select(_mapper.Map<ChatDto>).ToList();
            for (var i = 0; i < chatsDto.Count; i++)
            {
                if (chatsDto[i].Private)
                {
                    chatsDto[i].Title = user.Chats!.FirstOrDefault(x => x.Id == chatsDto[i].Id)?.Users
                        ?.FirstOrDefault(x => x.Id != user.Id)?.UserName ?? "_";
                }

                var last = _context.Messages?.Where(x => x.Chat.Id == chatsDto[i].Id)
                    ?.OrderByDescending(x => x.Time)?
                    .FirstOrDefault();
                MessageDto? lastDto;
                if (last != null)
                {
                    lastDto = _mapper.Map<MessageDto>(last);
                    lastDto.Self = last.From.Id == UserId;
                }
                else
                {
                    lastDto = new MessageDto() { ChatId = chatsDto[i].Id, Text = "There are no messages yet", Id = -1, Self = false };
                }

                chatsDto[i].LastMessage = lastDto ;
            }
            return Json(chatsDto);
        }

        [SwaggerResponse(200, "", typeof(IEnumerable<FullChatDto>))]
        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetById([StringLength(36, MinimumLength = 36)]string chatId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
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
                chatDto.Title = chatDto.Users.FirstOrDefault(x => x.Id != user.Id)?.UserName ?? "Something goes wrong";
            }
            return Json(chatDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            var chat = new Chat()
            {
                Id = Guid.NewGuid().ToString(),
                Private = request.Private,
                Title = request.Title??"_"
            };
            chat.Users = new HashSet<XIdentityUser>
            {
                user
            };
            if (request.Private)
            {
                if (request.UserIds.Count > 1 || request.UserIds.FirstOrDefault()==user.Id)
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
        [SwaggerResponse(200,"",typeof(MessageDto))]
        [HttpGet("history/{chatId}")]
        public async Task<IActionResult> GetHistory([StringLength(36, MinimumLength = 36)] string chatId, [FromQuery] int skip = 0, [FromQuery] int take = 40)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var history = _context.Messages.Where(x => x.Chat.Id == chatId).OrderByDescending(x => x.Time).Skip(skip).Take(take).ToList();
            var historyDto = history.Select(_mapper.Map<MessageDto>).ToList();
            for (int i = 0; i < historyDto.Count; i++)
            {
                historyDto[i].Self = historyDto[i].From.Id == UserId;
            }
            return Json(historyDto);
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _context.Users.FirstOrDefault(x => x.Id == UserId);
            if (user == null)
            {
                return NotFound("User was not identify");
            }

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return NoContent();
            }
            var mess = _mapper.Map<XMessage>(request);
            await _message.CreateAsync(mess, user);
            
            return Ok();
        }

        [HttpDelete("message/{id}")]
        public async Task<IActionResult> DeleteMessage([StringLength(36, MinimumLength = 36)] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            var mess = await _context.Messages.FirstOrDefaultAsync(x => x.From.Id == id);
            if (mess == null)
            {
                return NotFound();
            }

            await _message.DeleteAsync(id);
            return Ok();
        }

        
    }
}
