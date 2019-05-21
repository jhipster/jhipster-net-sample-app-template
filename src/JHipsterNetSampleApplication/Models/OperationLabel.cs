using JHipsterNetSampleApplication.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace JHipsterNetSampleApplication.Models {
    [Table("operation_label")]
    public class OperationLabel : IJoinedEntity<Operation>, IJoinedEntity<Label> {
        public long OperationId { get; set; }
        public Operation Operation { get; set; }
        Operation IJoinedEntity<Operation>.Join {
            get => Operation;
            set => Operation = value;
        }

        public long LabelId { get; set; }
        public Label Label { get; set; }            
        Label IJoinedEntity<Label>.Join {
            get => Label;
            set => Label = value;
        }
    }
}
