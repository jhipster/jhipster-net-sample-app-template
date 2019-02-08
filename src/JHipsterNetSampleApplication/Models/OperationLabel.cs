namespace JHipsterNetSampleApplication.Models {
    public class OperationLabel {
        public int OperationId { get; set; }
        public Operation Operation { get; set; }

        public int LabelId { get; set; }
        public Label Label { get; set; }
    }
}
