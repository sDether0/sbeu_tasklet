using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SBEU.Exceptions;
using SBEU.Tasklet.DataLayer.DataBase;
using SBEU.Tasklet.DataLayer.DataBase.Entities;
using SBEU.Tasklet.DataLayer.DataBase.Entities.Interfaces;
using SBEU.Tasklet.Api.Repositories.Interfaces;
using Serilog;

namespace SBEU.Tasklet.Api.Repositories
{
    public class BaseRepository<Entity> : IRepository<Entity> where Entity : DeletableEntity
    {
        protected readonly ApiDbContext _context;

        public BaseRepository(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Entity> GetByIdAsync(string id)
        {
            try
            {
                var entity = await _context.Set<Entity>().FindAsync(id);
                if (entity == null || entity is { IsDeleted: true })
                {
                    Log.Error("Entity {@entity} with id {@id} was not found", typeof(Entity).Name, id);
                    throw new EntityNotFoundException(string.Format($"Entity {0} with id {1} was not found", typeof(Entity).Name, id));
                }
                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something was wrong in {@method} in class {@class}", nameof(GetByIdAsync), GetType().Name);
                throw new CriticalRepositoryException(string.Format($"Something was wrong in {0} in class {1}",
                    nameof(GetByIdAsync), GetType().Name));
            }
        }

        public async Task<IQueryable<Entity>> GetAllAsync()
        {
            try
            {
                var entities = _context.Set<Entity>().Where(x => x.IsDeleted == false);
                return entities;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something was wrong in {@method} in class {@class}", nameof(GetAllAsync), GetType().Name);
                throw new CriticalRepositoryException(string.Format($"Something was wrong in {0} in class {1}",
                    nameof(GetAllAsync), GetType().Name));
            }
        }

        public async Task<Entity> CreateAsync(Entity entity)
        {
            try
            {
                entity.Id = Guid.NewGuid().ToString();
                _context.Set<Entity>().Add(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something was wrong in {@method} in class {@class}", nameof(CreateAsync), GetType().Name);
                throw  new CriticalRepositoryException(string.Format($"Something was wrong in {0} in class {1}",
                    nameof(CreateAsync), GetType().Name),ex);
            }
        }

        public virtual async Task<Entity> UpdateAsync(Entity entity)
        {
            try
            {
                if ((await _context.Set<Entity>().FindAsync(entity.Id)) == null)
                {
                    Log.Error("Entity {@entity} with id {@id} was not found", typeof(Entity).Name, entity.Id);
                    throw new EntityNotFoundException(string.Format($"Entity {0} with id {1} was not found", typeof(Entity).Name, entity.Id));
                }

                _context.Set<Entity>().Update(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something was wrong in {@method} in class {@class}", nameof(UpdateAsync), GetType().Name);
                throw new CriticalRepositoryException(string.Format($"Something was wrong in {0} in class {1}",
                    nameof(UpdateAsync), GetType().Name));
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                var entity = await _context.Set<Entity>().FindAsync(id);
                if (entity == null)
                {
                    Log.Error("Entity {@entity} with id {@id} was not found", typeof(Entity).Name, id);
                    throw new EntityNotFoundException(string.Format($"Entity {0} with id {1} was not found", typeof(Entity).Name, id));
                }

                if (entity is IDeletable deletable)
                {
                    deletable.IsDeleted = true;
                    _context.Set<Entity>().Update(entity);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Log.Error("Entity {0} is not deletable", typeof(Entity).Name);
                    throw new NonDeletableEntityException(string.Format($"Entity {0} is not deletable", typeof(Entity).Name));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something was wrong in {@method} in class {@class}", nameof(DeleteAsync), GetType().Name);
                throw new CriticalRepositoryException(string.Format($"Something was wrong in {0} in class {1}",
                    nameof(DeleteAsync), GetType().Name));
            }
        }
    }
}
