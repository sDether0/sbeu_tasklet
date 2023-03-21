using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SBEU.Tasklet.Api.Service.Interface;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Service
{
    public class CustomCache : ICCache
    {
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public CustomCache(IDistributedCache cache, IMapper mapper)
        {
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetUser(string userId, ApiDbContext context)
        {
            XIdentityUser? user;
            UserDto? dto;
            var json = await _cache.GetStringAsync(userId);
            if (json != null)
            {
                dto = JsonConvert.DeserializeObject<UserDto>(json);
                Console.WriteLine("Cached");
                return dto;
            }

            user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            dto = _mapper.Map<UserDto>(user);
            json = JsonConvert.SerializeObject(dto);
            await _cache.SetStringAsync(userId,json,new DistributedCacheEntryOptions(){AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)});
            return dto;
        }
    }
}
