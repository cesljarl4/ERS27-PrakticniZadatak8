namespace Application.Interfaces.Services;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
