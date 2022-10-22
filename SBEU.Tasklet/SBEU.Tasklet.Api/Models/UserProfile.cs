using AutoMapper;

using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Models
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<XIdentityUser, UserDto>();
            CreateMap<UserDto, XIdentityUser>();
            CreateMap<TableDto, XTable>();
            CreateMap<XTable, TableDto>();
            CreateMap<CreateTableRequest, XTable>();
            CreateMap<XTask, TaskDto>();
            CreateMap<CreateTaskRequest, XTask>()
                .ForMember(x => x.Executor, s => s.MapFrom(o => new XIdentityUser(){Id = o.ExecutorId}))
                .ForMember(x=>x.Table,s=>s.MapFrom(o=>new XTable(){Id = o.TableId}));
            CreateMap<Chat, ChatDto>();
            CreateMap<Chat, FullChatDto>();
            CreateMap<MessageRequest, XMessage>();
            CreateMap<XMessage, MessageDto>()
                .ForMember(x=>x.ChatId,s=>s.MapFrom(o=>o.Chat.Id))
                .ForMember(x=>x.TableId,s=>s.MapFrom(o=>o.Table.Id))
                .ForMember(x=>x.TaskId,s=>s.MapFrom(o=>o.Task.Id));
        }
    }
}
