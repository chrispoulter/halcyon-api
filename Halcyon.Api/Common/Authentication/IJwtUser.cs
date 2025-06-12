namespace Halcyon.Api.Common.Authentication;

public interface IJwtUser
{
    public Guid Id { get; }

    public string EmailAddress { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public List<string>? Roles { get; }
}
