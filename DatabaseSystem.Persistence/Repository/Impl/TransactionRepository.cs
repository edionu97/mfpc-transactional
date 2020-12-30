using System;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository.Abstract.Impl;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Impl
{
    public class TransactionRepository : AbstractRepository<Transaction, TransactionalDbContext>
    {
        public TransactionRepository(Func<TransactionalDbContext> getContext) : base(getContext)
        {
        }

        protected override DbSet<Transaction> GetDatabaseSet(TransactionalDbContext context)
        {
            return context.Transactions;
        }
    }
}
