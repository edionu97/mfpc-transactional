using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Persistence.Models
{
    public class Operation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OperationId { get; set; }

        public OperationType OperationType { get; private set; }

        private string _databaseQuery;
        [Required]
        public string DatabaseQuery
        {
            get => _databaseQuery;
            set
            {
                _databaseQuery = value;

                if (string.IsNullOrEmpty(_databaseQuery))
                {
                    return;
                }

                //get the operation keyword
                var operationKeyword = _databaseQuery.ToLower().Split(' ').First();
                Enum.TryParse<OperationType>(operationKeyword, true, out var parsedOperationType);
                OperationType = parsedOperationType;
            }
        }

        public int TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
