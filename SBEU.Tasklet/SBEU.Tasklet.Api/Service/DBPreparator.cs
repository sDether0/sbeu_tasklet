using Microsoft.EntityFrameworkCore;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;

namespace SBEU.Tasklet.Api.Service
{
    public static class DBPreparator
    {
        public static async Task EnsureMigrate(ApiDbContext context)
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        public static async Task FromZeroAppend(ApiDbContext context)
        {
            
                var newtp = new TaskProgress() { Id = 0, Status = "New" };
                if (!context.TaskProgress.Contains(newtp))
                {
                    context.Add(newtp);
                }

                var doing = new TaskProgress() { Id = 1, Status = "Doing" };
                if (!context.TaskProgress.Contains(doing))
                {
                    context.Add(doing);
                }

                var review = new TaskProgress() { Id = 2, Status = "Review" };
                if (!context.TaskProgress.Contains(review))
                {
                    context.Add(review);
                }

                var done = new TaskProgress() { Id = 3, Status = "Done" };
                if (!context.TaskProgress.Contains(done))
                {
                    context.Add(done);
                }

                var closed = new TaskProgress() { Id = 4, Status = "Closed" };
                if (!context.TaskProgress.Contains(closed))
                {
                    context.Add(closed);
                }

                await context.SaveChangesAsync();
        }

    }
}
