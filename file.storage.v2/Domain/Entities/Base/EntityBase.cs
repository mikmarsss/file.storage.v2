namespace file_storage.Domain.Entities.Base
{
    public abstract class EntityBase
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }


        protected EntityBase() { }

        protected void SetId(Guid id) => Id = id;
        protected void UpdateTimestamps() => UpdatedAt = DateTime.UtcNow;
    }
}
