using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Core.Database;

public class EntityChangedEvent
{
    public Guid? Id { get; set; }

    public EntityState ChangeType { get; set; }

    public string Entity { get; set; }
}
