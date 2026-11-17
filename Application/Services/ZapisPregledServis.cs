using Application.Interfaces.Repositories;
using Domain.Enums;
using Domain.Models;
using Shared;

namespace Application.Services;

// F3 i F4: zavisi samo od interfejsa za citanje.
public sealed class ZapisPregledServis
{
    private readonly IZapisiCitanje _citanje;

    public ZapisPregledServis(IZapisiCitanje citanje)
    {
        _citanje = citanje;
    }

    // F3: svi zapisi i preostalo planirano vreme (zbir trajanja nezavrsenih aktivnosti).
    public Result<PregledRezultat> Pregled()
    {
        Result<IReadOnlyList<Zapis>> svi = _citanje.Svi();
        if (svi.IsFailure)
        {
            return Result<PregledRezultat>.Failure(svi.Error!.Code, svi.Error.Message);
        }

        IReadOnlyList<Zapis> zapisi = svi.Value!;
        int preostalo = zapisi
            .Where(z => z.Status == StatusObrade.Zapoceta)
            .Sum(z => z.ProcenjenoTrajanjeMin);

        return Result<PregledRezultat>.Success(new PregledRezultat(zapisi, preostalo));
    }

    // F4: pretraga po oblasti, bez razlike velikih i malih slova.
    // Prazna lista je uspesan rezultat; poruku "nema rezultata" prikazuje Presentation.
    public Result<IReadOnlyList<Zapis>> PretraziPoOblasti(string? tekst)
    {
        if (string.IsNullOrWhiteSpace(tekst))
        {
            return Result<IReadOnlyList<Zapis>>.Failure("pretraga.prazan_upit", "Unesite naziv oblasti za pretragu.");
        }

        Result<IReadOnlyList<Zapis>> svi = _citanje.Svi();
        if (svi.IsFailure)
        {
            return Result<IReadOnlyList<Zapis>>.Failure(svi.Error!.Code, svi.Error.Message);
        }

        string upit = tekst.Trim();
        List<Zapis> nadjeni = svi.Value!
            .Where(z => z.Oblast.Contains(upit, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Result<IReadOnlyList<Zapis>>.Success(nadjeni);
    }
}
