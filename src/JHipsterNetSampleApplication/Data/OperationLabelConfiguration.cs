using JHipsterNetSampleApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JHipsterNetSampleApplication.Data {
    public class OperationLabelConfiguration : IEntityTypeConfiguration<OperationLabel> {
        public void Configure(EntityTypeBuilder<OperationLabel> entity)
        {
//            entity.ToTable("operation_label");
//
//            entity.HasKey(it => new { it.OperationId, it.LabelId });
//
//            entity
//                .HasOne(it => it.Operation)
//                .WithMany(it => it.OperationLabels)
//                .HasForeignKey(it => it.OperationId);
//
//            entity.Property(it => it.OperationId)
//                .HasColumnName("operations_id");
//
//            entity
//                .HasOne(it => it.Label)
//                .WithMany(it => it.OperationLabels)
//                .HasForeignKey(it => it.LabelId);
//
//            entity.Property(it => it.LabelId)
//                .HasColumnName("labels_id");
        }
    }
}
