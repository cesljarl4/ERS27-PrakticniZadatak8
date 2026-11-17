using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Models;
using Shared;

namespace Application.Services;

// F5 i F6: promena statusa.
public sealed class ZapisStatusServis
{
    private readonly IZapisiCitanje _citanje;
    private readonly IZapisiUpis _upis;
    private readonly ILoggerServis _logger;

    public ZapisStatusServis(IZapisiCitanje citanje, IZapisiUpis upis, ILoggerServis logger)
    {
        _citanje = citanje;
        _upis = upis;
        _logger = logger;
    }

    public Result Zavrsi(Guid id) => Promeni(id, "zavrsavanje aktivnosti", z => z.Zavrsi());

    public Result PonovoOtvori(Guid id) => Promeni(id, "ponovno otvaranje aktivnosti", z => z.PonovoOtvori());

    private Result Promeni(Guid id, string operacija, Func<Zapis, DomenskiRezultat> akcija)
    {
        Result<Zapis> nadjen = _citanje.Pronadji(id);
        if (nadjen.IsFailure)
        {
            return Neuspeh(operacija, id, nadjen.Error!.Code, nadjen.Error.Message);
        }

        Zapis zapis = nadjen.Value!;
        DomenskiRezultat promena = akcija(zapis);
        if (!promena.Uspeh)
        {
            return Neuspeh(operacija, id, promena.Greska!.Kod, promena.Greska.Poruka);
        }

        Result upis = _upis.Azuriraj(zapis);
        if (upis.IsFailure)
        {
            return Neuspeh(operacija, id, upis.Error!.Code, upis.Error.Message);
        }

        _logger.Loguj($"USPEH {operacija}: {id}");
        return Result.Success();
    }

    private Result Neuspeh(string operacija, Guid id, string kod, string poruka)
    {
        _logger.Loguj($"NEUSPEH {operacija} ({id}): {kod} - {poruka}");
        return Result.Failure(kod, poruka);
    }
}
