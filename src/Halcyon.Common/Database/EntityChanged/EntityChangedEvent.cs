using Microsoft.EntityFrameworkCore;

namespace Halcyon.Common.Database.EntityChanged;

public record EntityChangedEvent(string Entity, EntityState State, Guid? Id) { }
