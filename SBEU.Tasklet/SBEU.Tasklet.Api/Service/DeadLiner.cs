using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;

using SBEU.Tasklet.DataLayer.DataBase;

using System.Threading.Tasks;

namespace SBEU.Tasklet.Api.Service
{
    public class DeadLiner
    {
        private readonly PeriodicTimer _deadTimer = new PeriodicTimer(TimeSpan.FromMinutes(20));
        private readonly IServiceScopeFactory scopeFactory;
        public DeadLiner(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task Start()
        {
            Console.WriteLine("Deadliner initialized");
            while (await _deadTimer.WaitForNextTickAsync())
            {
                using var context = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApiDbContext>();
                await context.XTasks.LoadAsync();
                var tasks = await context.XTasks.Include(x=>x.Executor).ToListAsync();
                var dead2h = tasks.Where(task => (DateTime.Now - (task.StartTime + task.Duration)).Hours < 2);
                var dead8h = tasks.Where(task => (DateTime.Now - (task.StartTime + task.Duration)).Hours < 8);
                
                MulticastMessage message;
                message = new MulticastMessage()
                {
                    Tokens = dead2h.Select(x=>x.Executor.PushToken).ToList(),
                    Data = new Dictionary<string, string>()
                    {
                        {"title", "Сроки горят!"},
                        {"body", $"У вас есть невыполненные задачи истекающие меньше чем через 2 часа"},
                    },
                    Notification = new Notification()
                    {
                        Title = "Сроки горят!",
                        Body = $"У вас есть невыполненные задачи истекающие меньше чем через 2 часа"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

                message = new MulticastMessage()
                {
                    Tokens = dead8h.Select(x => x.Executor.PushToken).ToList(),
                    Data = new Dictionary<string, string>()
                    {
                        {"title", "Осталось мало времени"},
                        {"body", $"У вас есть невыполненные задачи истекающие меньше чем через 8 часов"},
                    },
                    Notification = new Notification()
                    {
                        Title = "Осталось мало времени",
                        Body = $"У вас есть невыполненные задачи истекающие меньше чем через 8 часов"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            }
        }
    }
}
