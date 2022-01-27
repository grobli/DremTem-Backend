using System;
using System.Text.Json;

namespace DeviceManager.Core.Models
{
    public abstract class EntityBase<TIdentifier, TEntity> : IEntity<TIdentifier, TEntity>
        where TEntity : EntityBase<TIdentifier, TEntity>
    {
        public TIdentifier Id { get; set; }
        private readonly string _name;

        public string Name
        {
            get => _name;
            init => _name = value.Trim();
        }

        private string _displayName;

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        public DateTime? LastModified { get; set; }

        public DateTime Created { get; } = DateTime.UtcNow;

        public virtual void MapEditableFields(TEntity source)
        {
            DisplayName = source.DisplayName;
            LastModified = source.LastModified;
        }

        protected EntityBase()
        {
        }

        /**copy constructor */
        protected EntityBase(TEntity original)
        {
            Id = original.Id;
            Name = original.Name;
            DisplayName = original.DisplayName;
            LastModified = original.LastModified;
            Created = original.Created;
        }

        public override string ToString() => JsonSerializer.Serialize((TEntity)this);
    }
}