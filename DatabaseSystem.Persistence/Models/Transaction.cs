using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DatabaseSystem.Utility.Enums;

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
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The status of the transaction
        /// </summary>
        [DefaultValue(typeof(TransactionStatusType), nameof(TransactionStatusType.Active))]
        public TransactionStatusType Status { get; set; }

        public virtual IList<Lock> Locks { get; set; } = new List<Lock>();
        public virtual IList<Operation> Operations { get; set; } = new List<Operation>();
    }
 }
