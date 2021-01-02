using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Persistence.Models
{
    public class WaitForGraph
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WaitForGraphId { get; set; }

        public LockType LockType { get; set; }

        public string LockObject { get; set; }

        public string LockTable { get; set; }

        public int TransactionThatHasLockId { get; set; }
        public virtual Transaction TransactionThatHasLock { get; set; }

        public int TransactionThatWantsLockId { get; set; }
        public virtual Transaction TransactionThatWantsLock { get; set; }
    }
}
