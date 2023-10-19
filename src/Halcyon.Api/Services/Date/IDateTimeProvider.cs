namespace Halcyon.Api.Services.Date;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
