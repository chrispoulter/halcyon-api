using Microsoft.EntityFrameworkCore;

namespace Halcyon.Api.Common.Database.EntityChanged;

public record EntityChangedEvent(string Entity, EntityState State, Guid? Id) { }
