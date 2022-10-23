using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;
using AutoMapper;
using FirebaseAdmin.Messaging;
using Swashbuckle.AspNetCore.Annotations;

namespace SBEU.Tasklet.Api.Controllers
{
    [Route("[controller]")]
    public class UserController : ControllerExt
    {
        private readonly UserManager<XIdentityUser> _userManager;
        private readonly ApiDbContext _context;
        private readonly JwtConfig _jwtConfig;
        private readonly IMapper _mapper;
        public UserController(UserManager<XIdentityUser> userManager, ApiDbContext context, IOptionsMonitor<JwtConfig> optionsMonitor, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> AuthByMail([FromBody] EmailAuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                if (request.UserName == null)
                {
                    return BadRequest("User not found and username not provided");
                }
                user = new XIdentityUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                };
                await _userManager.CreateAsync(user);
            }

            var hash = await user.SendConfirmationEmail(_context);
            return Json(new { Code = hash });
        }

        [SwaggerResponse(200,"",typeof(IEnumerable<UserDto>))]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = _context.Users.ToList();
            var usersDto = users.Select(_mapper.Map<UserDto>);
            return Json(usersDto);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmAuth([FromBody] ConfirmEmailAuthRequest request)
        {
            var confirmation = _context.UserConfirmations.Include(x => x.User)
                .FirstOrDefault(x => x.HashCode == request.Code);
            if (confirmation == null)
            {
                return BadRequest("Invalid hash code");
            }

            if (confirmation.MailCode == request.MailCode)
            {
                _context.Remove(confirmation);
                await _context.SaveChangesAsync();
                var jwt = await GenerateJwtToken(confirmation.User);
                return Json(jwt);
            }
            return BadRequest("Invalid mail code");
        }


        [HttpPost("hookPush")]
        public async Task<IActionResult> PushHook([FromBody]PushHookRequest request)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound("User was not found or identify");
            }
            user.PushToken = request.PushToken;
            user.IsPushOn = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("switchPush/{bit}")]
        public async Task<IActionResult> SwitchPush(bool bit)
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound("User was not found or identify");
            }

            user.IsPushOn = bit;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("push")]
        public async Task<IActionResult> Push()
        {
            var user = await _context.Users.FindAsync(UserId);
            if (user == null)
            {
                return NotFound("User was not found or identify");
            }
            MulticastMessage message;
            message = new MulticastMessage()
            {
                Tokens = new []{ user.PushToken},
                Data = new Dictionary<string, string>()
                {
                    {"title", "Test push"},
                    {"body", $"Test push"},
                },
                Notification = new Notification()
                {
                    Title = "Test push",
                    Body = $"Test push"
                }
            };
            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            return Ok();
        }

        private async Task<AuthResult> GenerateJwtToken(XIdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
