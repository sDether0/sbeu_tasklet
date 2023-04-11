using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using Org.BouncyCastle.Asn1.Ocsp;

using SBEU.Exceptions;
using SBEU.Tasklet.Api.Hubs;
using SBEU.Tasklet.Api.Repositories.Interfaces;
using SBEU.Tasklet.Api.Service;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.DataLayer.Migrations;
using SBEU.Tasklet.Models.Enums;
using SBEU.Tasklet.Models.Requests;
using SBEU.Tasklet.Models.Responses;

namespace SBEU.Tasklet.Api.Repositories
{
    public class TaskRepository : BaseRepository<XTask>, ITaskRepository
    {

        private readonly IHubContext<TaskHub, IXTask> _taskHub;
        public TaskRepository(ApiDbContext context, IHubContext<TaskHub, IXTask> taskHub) : base(context)
        {
            _taskHub = taskHub;
        }

        public async Task<XTask> CreateAsync(XTask entity, XIdentityUser user, XHistory history)
        {
            //var entity = new XTask();
            entity.Id = Guid.NewGuid().ToString();
            if (entity.Status is { Status: { } })
            {
                entity.StatusId = entity.Status.Status.GetProgress(_context).Id;
            }

            if (!_context.XTables.Any(x => x.Id == entity.Table.Id))
            {
                throw new EntityNotFoundException("Table not found");
            }

            if (user.Tables.All(x => x.Id != entity.Table.Id))
            {
                throw new NoAccessException("You has not access to that table");
            }
            entity.Author = user;
            entity.Table = entity.Table.Id.Get<XTable>(_context);
            if ((await _context.XTables.FindAsync(entity.Table.Id))!.Users.All(x => x.Id != entity.Executor.Id))
            {
                throw new NoAccessException("User has not access to that table");
            }
            entity.Executor = entity.Executor.Id.Get<XIdentityUser>(_context);
            //entity.Status = entity.Status;

            entity.StartTime = entity.StartTime is { } start ? start.ToUniversalTime() : DateTime.UtcNow;
            var contIds = entity.Contents.Select(x => x.Id);
            entity.Contents = _context.Contents.Where(x => contIds.Contains(x.Id)).ToList();

            history.Updater = user;
            history.Executor = entity.Executor;
            history.Status = entity.Status;

            var result = await base.CreateAsync(entity);
            //await _context.XTasks.AddAsync(task);
            //await _context.SaveChangesAsync();
            var tableUsers = _context.XTables.FirstOrDefault(x => x.Id == entity.Table.Id)?.Users
                .Select(x => x.Id) ?? Enumerable.Empty<string>();

            await Notifier.FullNotify(entity, _taskHub, tableUsers, user.Id);

            

            history.Task = result;
            var ids = history.Contents.Select(x => x.Id);
            history.Contents = _context.Contents.Where(x => ids.Contains(x.Id)).ToList();
            _context.Add(history);
            await _context.SaveChangesAsync();
            
            return result;
        }


        public async Task<XTask> UpdateAsync(XTask entity, XIdentityUser user)
        {
            var task = await GetByIdAsync(entity.Id);

            if (user.Tasks.Contains(task) || user.AuthoredTasks.Contains(task))
            {
                var newst = false;
                var upTask = new XHistory();
                upTask.UpdateDate = DateTime.UtcNow;
                upTask.Updater = user;
                if (entity.Title is { } title && title != task.Title)
                {
                    task.Title = title;
                    upTask.Title = title;
                }

                if (entity.Description is { } description && description != task.Description)
                {
                    task.Description = description;
                    upTask.Description = description;
                }

                if (entity.Executor?.Id is { } executorId && executorId != task.Executor.Id)
                {
                    if (_context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)!.Users
                        .All(x => x.Id != entity.Executor.Id))
                    {
                        throw new NoAccessException("User has no access to that table");
                    }

                    var executor = entity.Executor.Id.Get<XIdentityUser>(_context);
                    task.Executor = executor;
                    upTask.Executor = executor;
                }

                if (entity.Status is { } status && status.Status != task.Status.Status)
                {
                    newst = true;
                    var tstatus = status.Status.GetProgress(_context);
                    task.Status = tstatus;
                    upTask.Status = tstatus;
                    if (status.Status == "Done" || status.Status == "Closed")
                    {
                        task.EndTime = DateTime.UtcNow;
                    }
                }

                task.Links = entity.Links ?? task.Links;
                if (entity.Contents is { } contents && contents != task.Contents.Select(x => x.Id))
                {
                    var ids = contents.Select(x => x.Id);
                    var cont = _context.Contents.Where(x => ids.Contains(x.Id)).ToList();
                    task.Contents = cont;
                    upTask.Contents = cont;
                }

                upTask.Task = task;
                var result = await base.UpdateAsync(task);
                var tableUsers = _context.XTables.Include(x => x.Users).FirstOrDefault(x => x.Id == task.Table.Id)
                    ?.Users
                    .Select(x => x.Id) ?? Enumerable.Empty<string>();
                if (newst && (task.Author.Id == user.Id ? task.Executor.IsMailNotify : task.Author.IsMailNotify))
                {
                    await Notifier.FullNotify(task, _taskHub, tableUsers, user.Id);
                }
                else
                {
                    await Notifier.SocketNotify(task, _taskHub, tableUsers);
                }

                
                if (upTask is { })
                {
                    upTask.Task = result;

                    var ids = upTask.Contents.Select(x => x.Id);
                    upTask.Contents = _context.Contents.Where(x => ids.Contains(x.Id)).ToList();
                    //upTask.Contents = _context.Contents.Where(x => upTask.Contents.Any(s => s.Id == x.Id)).ToList();
                    _context.Add(upTask);
                    await _context.SaveChangesAsync();
                }

                return result;
            }

            throw new NoAccessException("User has no access to that task");
        }
    }
}
