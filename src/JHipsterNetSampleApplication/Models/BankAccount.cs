using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JHipsterNetSampleApplication.Models {
    [Table("bank_account")]
    public class BankAccount {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required] [Column("name")] public string Name { get; set; }

        [Required] [Column("balance")] public decimal? Balance { get; set; }

        public User User { get; set; }

        public virtual ICollection<Operation> Operations { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var bankAccount = obj as BankAccount;
            if (bankAccount?.Id == null || Id == null) return false;
            return EqualityComparer<string>.Default.Equals(Id, bankAccount.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "BankAccount{" +
                   $"ID='{Id}'" +
                   $", Name='{Name}'" +
                   $", Balance='{Balance}'" +
                   "}";
        }
    }
}
