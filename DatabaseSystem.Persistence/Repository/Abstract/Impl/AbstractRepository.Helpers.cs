using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Abstract.Impl
{
    public abstract partial class AbstractRepository<T, TContext>
    {
        private static IQueryable<T> IncludeFields(IQueryable<T> queryable, IEnumerable<string> fieldsToBeIncluded)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var propertyName in fieldsToBeIncluded ?? new List<string>())
            {
                queryable = queryable
                    .Include(typeof(T).GetProperty(propertyName)?.Name ?? throw new Exception("Not found"));
            }

            return queryable;
        }
    }
}
