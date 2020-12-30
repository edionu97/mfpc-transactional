using System;
using Microsoft.EntityFrameworkCore.Design;

namespace DatabaseSystem.Persistence.DatabaseContext.ContextFactory
{
    /// <summary>
    /// This class will be used by the EF Core when the DbContext will be created
    /// </summary>
    public class TransactionalDbContextFactory : IDesignTimeDbContextFactory<TransactionalDbContext>
    {
        public TransactionalDbContext CreateDbContext(string[] args)
        {
            return new TransactionalDbContext(
                "Data Source=DESKTOP-VQ4KD11;Initial Catalog=Transactional;Integrated Security=True");
        }
    }
}

