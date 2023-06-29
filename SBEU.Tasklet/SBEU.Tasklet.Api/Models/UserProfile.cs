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
            Console.WriteLine("user");
            CreateMap<XIdentityUser, UserDto>();
            CreateMap<UserDto, XIdentityUser>();
            Console.WriteLine("table");
            CreateMap<TableDto, XTable>();
            CreateMap<XTable, TableDto>();
            CreateMap<XTable, SmallTableDto>();
            CreateMap<CreateTableRequest, XTable>();
            CreateMap<TaskProgress, StatusDto>();
            Console.WriteLine("task");
            CreateMap<XHistory, HistoryDto>()
                .ForMember(x => x.Status, s => s.MapFrom(o => o.Status.Status));
            CreateMap<XTask, TaskDto>()
                .ForMember(x => x.Note, s => s.Ignore())
                .ForMember(x => x.Duration, s => s.MapFrom(o => o.Duration.TotalHours));
            CreateMap<CreateTaskRequest, XTask>()
                .ForMember(x => x.Executor, s => s.MapFrom(o => new XIdentityUser() { Id = o.ExecutorId }))
                .ForMember(x => x.Table, s => s.MapFrom(o => new XTable() { Id = o.TableId }))
                .ForMember(x => x.Duration, s => s.MapFrom(o => TimeSpan.FromHours(o.Duration)))
                .ForMember(x=>x.Contents,s=>s.MapFrom(o=>o.Contents.Select(t=>new XContent(){Id=t})))
                .ForMember(x=>x.Status, s=>s.MapFrom(o=>new TaskProgress(){Status = o.Status}))
                .ForMember(x => x.Notes, s => s.Ignore());
            CreateMap<CreateTaskRequest, XHistory>()
                .ForMember(x => x.UpdateDate, s => s.MapFrom(o => DateTime.UtcNow))
                .ForMember(x => x.Contents, s => s.MapFrom(o => o.Contents.Select(t => new XContent() { Id = t })))
                .ForMember(x=>x.Status,s=>s.MapFrom(o=>new TaskProgress(){Status = o.Status}))
                .ForMember(x => x.Task, s => s.Ignore())
                .ForMember(x=>x.Updater,s=>s.Ignore())
                //.ForMember(x => x.IsDeleted, s => s.Ignore())
                .ForMember(x => x.Id, s => s.Ignore());
            CreateMap<UpdateTaskRequest, XTask>()
                .ForMember(x => x.Author, s => s.Ignore())
                .ForMember(x => x.Executor, s => s.MapFrom(o => o.ExecutorId != null ? new XIdentityUser() { Id = o.ExecutorId } : null))
                .ForMember(x => x.Contents, s => s.MapFrom(o => o.Contents.Select(t => new XContent() { Id = t })))
                .ForMember(x => x.Status, s => s.MapFrom(o => new TaskProgress() { Status = o.Status }))
                .ForMember(x => x.History, s => s.Ignore())
                .ForMember(x => x.Notes, s => s.Ignore())
                .ForMember(x => x.Table, s => s.Ignore());
            Console.WriteLine("chat");
            CreateMap<Chat, ChatDto>();
            CreateMap<IQueryable<XTask>, List<TaskDto>>();
            CreateMap<Chat, FullChatDto>();
            Console.WriteLine("message");
            CreateMap<MessageRequest, XMessage>()
                .ForMember(x => x.Table, s => s.MapFrom(o => new XTable() { Id = o.TableId }))
                .ForMember(x => x.Task, s => s.MapFrom(o => new XTask() { Id = o.TaskId }))
                .ForMember(x => x.Chat, s => s.MapFrom(o => new Chat() { Id = o.ChatId }));
            CreateMap<XMessage, MessageDto>()
                .ForMember(x => x.ChatId, s => s.MapFrom(o => o.Chat.Id))
                .ForMember(x => x.TableId, s => s.MapFrom(o => o.Table.Id))
                .ForMember(x => x.TaskId, s => s.MapFrom(o => o.Task.Id));
            Console.WriteLine("complete");
        }
    }
}
