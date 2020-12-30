using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Utility;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Abstract.Impl
{
    public abstract partial class AbstractRepository<T, TContext> : IRepository<T> 
                                                                    where TContext : DbContext
                                                                    where T : class
    {
        protected Func<TContext> GetContext { get; }

        protected AbstractRepository(Func<TContext> getContext)
        {
            GetContext = getContext;
        }

        protected abstract DbSet<T> GetDatabaseSet(TContext context);

        public async Task<T> FindByIdAsync(int id, IList<string> fieldsToBeIncluded = null)
        {
            //create the context
            await using var context = GetContext();
            
            //get all items from database
            var resultList = await IncludeFields(GetDatabaseSet(context), fieldsToBeIncluded).ToListAsync();

            //get the id property
            var idProperty = await ReflectionHelpers.GetPropertyValueFromObjectAs<T, KeyAttribute>();

            //get the the item thar respects the condition
            return resultList.FirstOrDefault(x =>
            {
                var value = idProperty.GetValue(x);
                return value is int @int && @int == id;
            });
        }

        public async Task<IList<T>> GetAllAsync(IList<string> fieldsToBeIncluded = null)
        {
            //create the context
            await using var context = GetContext();

            //get all items from database
            return await IncludeFields(GetDatabaseSet(context), fieldsToBeIncluded).ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            //create the context
            await using var context = GetContext();

            //update the entity
            context.Update(entity);

            //save the changes
            await context.SaveChangesAsync();
        }

        public async Task AddAsync(T entity)
        {
            //create the context
            await using var context = GetContext();

            //add the entity
            await context.AddAsync(entity);

            //save the changes
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            //create the context
            await using var context = GetContext();

            //add the entity
            context.Remove(entity);

            //save the changes
            await context.SaveChangesAsync();
        }
       
    }
}
