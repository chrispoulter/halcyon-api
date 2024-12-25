using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Common.Events;

public record EntityChangedEvent(string Entity, EntityState State, Guid? Id) { }
