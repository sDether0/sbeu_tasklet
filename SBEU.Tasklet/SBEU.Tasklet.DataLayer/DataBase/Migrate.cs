using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SBEU.Tasklet.DataLayer.DataBase
{
    public class Migrate : IDesignTimeDbContextFactory<ApiDbContext>
    {
        public ApiDbContext CreateDbContext(string[] args)
        {
            var factory = new DbContextOptionsBuilder<ApiDbContext>();
            factory.UseLazyLoadingProxies().UseNpgsql("User ID = postgres; Password = 1namQfeg1; Host = localhost; Port = 5432; Database = SBEU.Task;");
            var context = new ApiDbContext(factory.Options);
            return context;
        }
    }
}
