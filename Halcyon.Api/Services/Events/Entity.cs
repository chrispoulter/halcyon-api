namespace Halcyon.Api.Services.Events;

public abstract class Entity
{
    private readonly List<object> _domainEvents = [];

    public List<object> DomainEvents => [.. _domainEvents];

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Raise(params object[] domainEvents)
    {
        _domainEvents.AddRange(domainEvents);
    }
}
