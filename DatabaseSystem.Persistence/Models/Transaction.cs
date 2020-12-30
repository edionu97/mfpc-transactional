using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseSystem.Persistence.Enums;

namespace DatabaseSystem.Persistence.Models
{
    public class Transaction
    {
        /// <summary>
        /// The primary key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        /// <summary>
        /// This field will be populated either on add either on update
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The status of the transaction
        /// </summary>
        public TransactionStatusType Status { get; set; }

        public virtual IList<Lock> Locks { get; set; } = new List<Lock>();

        public virtual IList<WaitForGraph> WaitForGraphsHasLocks { get; set; } = new List<WaitForGraph>();

        public virtual IList<WaitForGraph> WaitForGraphsWantsLocks { get; set; } = new List<WaitForGraph>();
    }
 }
