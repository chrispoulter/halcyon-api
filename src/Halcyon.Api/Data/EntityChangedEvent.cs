using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Data;

public class EntityChangedEvent
{
    public int? Id { get; set; }

    public EntityState ChangeType { get; set; }

    public string Entity { get; set; }
}
