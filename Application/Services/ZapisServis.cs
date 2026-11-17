using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using Shared;

namespace Application.Services;

// F1 i F2: dodavanje zapisa. Zavisi samo od uskog interfejsa za upis, loggera i sata.
public sealed class ZapisServis
{
    private readonly IZapisiUpis _upis;
    private readonly ILoggerServis _logger;
    private readonly IClock _clock;

    public ZapisServis(IZapisiUpis upis, ILoggerServis logger, IClock clock)
    {
        _upis = upis;
        _logger = logger;
        _clock = clock;
    }

    public Result<Guid> DodajNovuTemu(string? naziv, string? opis, string? oblast, int trajanjeMin, string? poglavlje)
    {
        var kreiranje = NovaTema.Kreiraj(naziv, opis, oblast, trajanjeMin, poglavlje, _clock.UtcNow);
        if (!kreiranje.Uspeh)
        {
            return Neuspeh("dodavanje nove teme", kreiranje.Greska!.Kod, kreiranje.Greska.Poruka);
        }

        return Sacuvaj("dodavanje nove teme", kreiranje.Vrednost!);
    }

    public Result<Guid> DodajPonavljanje(string? naziv, string? opis, string? oblast, int trajanjeMin, DateOnly? datumPrethodnogUcenja)
    {
        var kreiranje = Ponavljanje.Kreiraj(naziv, opis, oblast, trajanjeMin, datumPrethodnogUcenja, _clock.UtcNow);
        if (!kreiranje.Uspeh)
        {
            return Neuspeh("dodavanje ponavljanja", kreiranje.Greska!.Kod, kreiranje.Greska.Poruka);
        }

        return Sacuvaj("dodavanje ponavljanja", kreiranje.Vrednost!);
    }

    private Result<Guid> Sacuvaj(string operacija, Zapis zapis)
    {
        Result upis = _upis.Sacuvaj(zapis);
        if (upis.IsFailure)
        {
            return Neuspeh(operacija, upis.Error!.Code, upis.Error.Message);
        }

        _logger.Loguj($"USPEH {operacija}: {zapis.Id}");
        return Result<Guid>.Success(zapis.Id);
    }

    private Result<Guid> Neuspeh(string operacija, string kod, string poruka)
    {
        _logger.Loguj($"NEUSPEH {operacija}: {kod} - {poruka}");
        return Result<Guid>.Failure(kod, poruka);
    }
}
