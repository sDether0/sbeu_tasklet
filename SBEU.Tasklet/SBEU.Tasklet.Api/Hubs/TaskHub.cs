using Microsoft.AspNetCore.SignalR;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Hubs
{
    public interface IXTask
    {
        public Task UpdateTask(TaskDto task);
        public Task NewTask(TaskDto task);
    }
    public class TaskHub : Hub<IXTask>
    {
        /*
                var tables = _context.Users.Include(x => x.Tables).FirstOrDefault(x => x.Id == UserId)?.Tables
                    .Select(x => x.Id)??Enumerable.Empty<string>();
                foreach (var tbl in tables)
                {
                    Groups.AddToGroupAsync(Context.ConnectionId, tbl);
                }

         */
    }
}
