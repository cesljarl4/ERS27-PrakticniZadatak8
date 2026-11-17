using Application.Interfaces.Services;

namespace Infrastructure.Services;

// Drugi logger (S5): ispisuje poruke na konzolu umesto u datoteku.
// Izlaz moze da se prosledi spolja, npr. StringWriter u testu.
public sealed class ConsoleLoggerServis : ILoggerServis
{
    private readonly TextWriter _izlaz;

    public ConsoleLoggerServis(TextWriter? izlaz = null)
    {
        _izlaz = izlaz ?? Console.Error;
    }

    public bool Loguj(string poruka)
    {
        _izlaz.WriteLine($"[LOG]: {poruka}");
        return true;
    }
}
