﻿using Microsoft.EntityFrameworkCore;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Persistence.DatabaseContext
{
    public class TransactionalDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Lock> Locks { get; set; }

        public TransactionalDbContext()
            : base(GetConnectionOptions("Data Source=DESKTOP-VQ4KD11;Initial Catalog=Transactional;Integrated Security=True"))
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Transaction
            modelBuilder
                .Entity<Transaction>()
                .Property(t => t.Timestamp)
                .HasDefaultValueSql("getdate()");
        }

        /// <summary>
        /// This function it is used for specifying  the connection string
        /// </summary>
        /// <param name="connectionString">the string thar represents the connection string</param>
        /// <returns>an instance of DbContextOptions</returns>
        private static DbContextOptions GetConnectionOptions(string connectionString)
        {
            return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
        }
    }
}
