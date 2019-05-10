using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace JHipsterNetSampleApplication.Models {
    [Table("label")]
    public class Label {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [MinLength(3)]
        [Column("nhi_label")]
        // Currently, we need it because the frontend model and backend model are not the same on this property.
        // But, later in the .Net generator we will ask to the user that the properties need a different name from the entity
        [JsonProperty(PropertyName = "label")]
        public string Name { get; set; }

        //        [JsonIgnore]
        //        public IList<OperationLabel> OperationLabels { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var label = obj as Label;
            if (label?.Id == null || label?.Id == 0 || Id == 0) return false;
            return EqualityComparer<long>.Default.Equals(Id, label.Id);
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
