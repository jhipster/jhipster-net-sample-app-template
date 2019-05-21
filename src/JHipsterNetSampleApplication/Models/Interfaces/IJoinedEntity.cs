
namespace JHipsterNetSampleApplication.Models.Interfaces {
    public interface IJoinedEntity<TEntity> {
        TEntity Join { get; set; }
    }
}
