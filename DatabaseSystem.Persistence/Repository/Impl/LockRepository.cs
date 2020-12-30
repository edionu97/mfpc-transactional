using System;
using System.Collections.Generic;
using System.Text;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository.Abstract.Impl;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Impl
{
    public class LockRepository : AbstractRepository<Lock, TransactionalDbContext>
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
