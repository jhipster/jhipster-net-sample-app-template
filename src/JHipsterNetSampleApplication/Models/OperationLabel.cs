namespace JHipsterNetSampleApplication.Models {
    public class OperationLabel {
        public long OperationId { get; set; }
        public Operation Operation { get; set; }

        public long LabelId { get; set; }
        public Label Label { get; set; }
    }
}
