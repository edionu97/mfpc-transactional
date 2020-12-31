using System;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Impl
{
    public class LockRepository : Abstract.AbstractRepository<Lock, TransactionalDbContext>
    {
        public LockRepository(Func<TransactionalDbContext> getContext) : base(getContext)
        {
        }

        protected override DbSet<Lock> GetDatabaseSet(TransactionalDbContext context)
        {
            return context.Locks;
        }
    }
}
