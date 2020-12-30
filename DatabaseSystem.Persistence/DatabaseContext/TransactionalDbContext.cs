﻿using Microsoft.EntityFrameworkCore;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Persistence.DatabaseContext
{
    public class TransactionalDbContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Lock> Locks { get; set; }
        public DbSet<WaitForGraph> WaitForGraphs { get; set; }

        public TransactionalDbContext(string connectionString) : base(GetConnectionOptions(connectionString))
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<WaitForGraph>()
                .HasOne(t => t.TransactionThatHasLock)
                .WithMany(t => t.WaitForGraphsHasLocks)
                .HasForeignKey(m => m.TransactionThatHasLockId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<WaitForGraph>()
                .HasOne(t => t.TransactionThatWantsLock)
                .WithMany(t => t.WaitForGraphsWantsLocks)
                .HasForeignKey(m => m.TransactionThatWantsLockId)
                .OnDelete(DeleteBehavior.NoAction);
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