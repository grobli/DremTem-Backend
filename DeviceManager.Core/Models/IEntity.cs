using System;

namespace DeviceManager.Core.Models
{
    public interface IEntity<out TIdentifier, in T> where T : class
    {
        TIdentifier Id { get; }
        string Name { get; init; }
        string DisplayName { get; set; }
        DateTime? LastModified { get; set; }
        DateTime Created { get; }

        void MapEditableFields(T source);
    }
}