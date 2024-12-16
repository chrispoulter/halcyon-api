using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Services.Events;

public record EntityChangedEvent(string Entity, EntityState State, Guid? Id) { }
