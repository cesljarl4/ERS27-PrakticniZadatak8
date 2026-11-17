using Application.Interfaces.Formatters;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Database.Repositories;
using Infrastructure.Services;
using Presentation.Console;
using Presentation.Formatting;
using Presentation.Interfaces;

namespace Presentation.Composition;

// Sastavljanje zavisnosti na jednom mestu; Domain i Application ne biraju adaptere.
// Repozitorijum: "in-memory" (podrazumevano) ili "json".
// Logger: "file" (podrazumevano) ili "console".
public sealed class AppContainer
{
    public required IConsoleIO Io { get; init; }
    public required IZapisiRepozitorijum Repozitorijum { get; init; }
    public required ILoggerServis Logger { get; init; }
    public required IReadOnlyList<IZapisFormater> Formateri { get; init; }
    public required ZapisServis Servis { get; init; }
    public required ZapisPregledServis PregledServis { get; init; }
    public required ZapisStatusServis StatusServis { get; init; }

    public static AppContainer Create(string rezim, string logerRezim = "file")
    {
        IZapisiRepozitorijum repo = rezim.Equals("json", StringComparison.OrdinalIgnoreCase)
            ? new JsonZapisiRepozitorijum("podaci.json")
            : new InMemoryZapisiRepozitorijum();

        ILoggerServis logger = logerRezim.Equals("console", StringComparison.OrdinalIgnoreCase)
            ? new ConsoleLoggerServis()
            : new FileLoggerServis();

        IClock clock = new SystemClock();

        return new AppContainer
        {
            Io = new SystemConsoleIO(),
            Repozitorijum = repo,
            Logger = logger,
            Formateri = new List<IZapisFormater> { new SazetiFormater(), new DetaljniFormater() },
            Servis = new ZapisServis(repo, logger, clock),
            PregledServis = new ZapisPregledServis(repo),
            StatusServis = new ZapisStatusServis(repo, repo, logger)
        };
    }
}
