using Microsoft.EntityFrameworkCore;

namespace Halcyon.Common.Events;

public record EntityChangedEvent(string Entity, EntityState State, Guid? Id) { }
