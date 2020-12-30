using System;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository.Abstract.Impl;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Impl
{
    public class DependenciesRepository : AbstractRepository<WaitForGraph, TransactionalDbContext>
    {
        public DependenciesRepository(Func<TransactionalDbContext> getContext) : base(getContext)
        {
        }

        protected override DbSet<WaitForGraph> GetDatabaseSet(TransactionalDbContext context)
        {
            return context.WaitForGraphs;
        }
    }
}
