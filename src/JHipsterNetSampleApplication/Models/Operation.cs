using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JHipsterNetSampleApplication.Models {
    [Table("operation")]
    public class Operation {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required] [Column("nhi_date")] public DateTime? Date { get; set; }


        [Column("description")] public string Description { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

//        [JsonIgnore]
//        public virtual BankAccount BankAccount { get; set; }

//        public IList<OperationLabel> OperationLabels { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var operation = obj as Operation;
            if (operation?.Id == null || Id == null) return false;
            return Equals(Id, operation.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Operation{" +
                   $"ID='{Id}'" +
                   $", Date='{Date}'" +
                   $", Description='{Description}'" +
                   $", Amount='{Amount}'" +
                   "}";
        }
    }
}
