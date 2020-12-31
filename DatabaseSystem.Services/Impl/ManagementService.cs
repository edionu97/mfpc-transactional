using System;
using System.Collections.Generic;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;

namespace DatabaseSystem.Services.Impl
{
    public class ManagementService : IManagementService
    {
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<WaitForGraph> _dependencyRepository;

        public ManagementService(IRepository<WaitForGraph> dependencyRepository,
                                 IRepository<Lock> lockRepository, 
                                 IRepository<Transaction> transactionRepository)
        {
            _dependencyRepository = dependencyRepository;
            _lockRepository = lockRepository;
            _transactionRepository = transactionRepository;
        }

        public Transaction CreateTransaction(IList<string> operations)
        {
            throw new NotImplementedException();
        }
    }
}
