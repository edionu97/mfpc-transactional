using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Utility;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Abstract
{
    public abstract partial class AbstractRepository<T, TContext> : IRepository<T>
                                                                    where TContext : DbContext
                                                                    where T : class
    {
        protected readonly SemaphoreSlim LockSemaphore = new SemaphoreSlim(1, 1);

        protected Func<TContext> GetContext { get; }

        protected AbstractRepository(Func<TContext> getContext)
        {
            GetContext = getContext;
        }

        protected abstract DbSet<T> GetDatabaseSet(TContext context);

        public async Task<T> FindByIdAsync(int id, IList<string> fieldsToBeIncluded = null)
        {
            await LockSemaphore.WaitAsync();
            try
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
            finally
            {
                LockSemaphore.Release();
            }
        }

        public async Task<IList<T>> GetAllAsync(IList<string> fieldsToBeIncluded = null)
        {
            await LockSemaphore.WaitAsync();
            try
            {
                //create the context
                await using var context = GetContext();

                //get all items from database
                return await IncludeFields(GetDatabaseSet(context), fieldsToBeIncluded).ToListAsync();
            }
            finally
            {
                LockSemaphore.Release();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            await LockSemaphore.WaitAsync();
            try
            {
                //create the context
                await using var context = GetContext();

                //begin new transaction
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    //update the entity
                    context.Update(entity);

                    //save the changes
                    await context.SaveChangesAsync();

                    //commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            finally
            {
                LockSemaphore.Release();
            }
        }

        public async Task AddAsync(T entity)
        {
            await LockSemaphore.WaitAsync();
            try
            {
                //create the context
                await using var context = GetContext();

                //begin new transaction
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    //add the entity
                    await context.AddAsync(entity);

                    //save the changes
                    await context.SaveChangesAsync();

                    //commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            finally
            {
                LockSemaphore.Release();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            await LockSemaphore.WaitAsync();
            try
            {
                //create the context
                await using var context = GetContext();

                //begin new transaction
                await using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    //add the entity
                    context.Remove(entity);

                    //save the changes
                    await context.SaveChangesAsync();

                    //commit the transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            finally
            {
                LockSemaphore.Release();
            }
        }
    }
}
