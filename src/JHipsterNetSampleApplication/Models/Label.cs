using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JHipsterNetSampleApplication.Models {
    [Table("label")]
    public class Label {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        [Column("nhi_label")]
        public string Name { get; set; }

//        [JsonIgnore]
//        public IList<OperationLabel> OperationLabels { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var label = obj as Label;
            if (label?.Id == null || Id == null) return false;
            return EqualityComparer<string>.Default.Equals(Id, label.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string ToString()
        {
            return "Label{" +
                   $"ID='{Id}'" +
                   $", Name='{Name}'" +
                   "}";
        }
    }
}
