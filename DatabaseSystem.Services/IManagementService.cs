using System.Collections.Generic;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services
{
    public interface IManagementService
    {
        public Transaction CreateTransaction(IList<string> operations);


    }
}
