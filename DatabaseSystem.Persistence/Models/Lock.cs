using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseSystem.Persistence.Enums;

namespace DatabaseSystem.Persistence.Models
{
    public class Lock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LockId { get; set; }

        public LockType LockType { get; set; }
        
        public string Object { get; set; }

        public string TableName { get; set; }

        /// <summary>
        /// These represents the navigation properties (the foreign key)
        /// </summary>
        public int TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
