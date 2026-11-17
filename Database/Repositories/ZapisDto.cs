using Domain.Common;
using Domain.Enums;
using Domain.Models;

namespace Database.Repositories;

// Ravni oblik zapisa za JSON. Pri citanju se zapis ponovo gradi preko Kreiraj(...),
// pa se pravila P1-P3 primenjuju i na podatke iz datoteke.
internal sealed class ZapisDto
{
    public string Tip { get; set; } = string.Empty;
    public Guid Id { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public string Oblast { get; set; } = string.Empty;
    public int ProcenjenoTrajanjeMin { get; set; }
    public DateTimeOffset KreiranoUtc { get; set; }
    public StatusObrade Status { get; set; }
    public string? Poglavlje { get; set; }
    public DateOnly? DatumPrethodnogUcenja { get; set; }

    public static ZapisDto IzZapisa(Zapis zapis)
    {
        var dto = new ZapisDto
        {
            Id = zapis.Id,
            Naziv = zapis.Naziv,
            Opis = zapis.Opis,
            Oblast = zapis.Oblast,
            ProcenjenoTrajanjeMin = zapis.ProcenjenoTrajanjeMin,
            KreiranoUtc = zapis.KreiranoUtc,
            Status = zapis.Status
        };

        if (zapis is NovaTema nova)
        {
            dto.Tip = nameof(NovaTema);
            dto.Poglavlje = nova.Poglavlje;
        }
        else if (zapis is Ponavljanje ponavljanje)
        {
            dto.Tip = nameof(Ponavljanje);
            dto.DatumPrethodnogUcenja = ponavljanje.DatumPrethodnogUcenja;
        }

        return dto;
    }

    public DomenskiRezultat<Zapis> UZapis()
    {
        if (Tip == nameof(NovaTema))
        {
            var nova = NovaTema.Kreiraj(Naziv, Opis, Oblast, ProcenjenoTrajanjeMin, Poglavlje, KreiranoUtc, Id, Status);
            if (!nova.Uspeh)
            {
                return DomenskiRezultat<Zapis>.Neuspeh(nova.Greska!.Kod, nova.Greska.Poruka);
            }

            return DomenskiRezultat<Zapis>.Ok(nova.Vrednost!);
        }

        if (Tip == nameof(Ponavljanje))
        {
            var ponavljanje = Ponavljanje.Kreiraj(Naziv, Opis, Oblast, ProcenjenoTrajanjeMin, DatumPrethodnogUcenja, KreiranoUtc, Id, Status);
            if (!ponavljanje.Uspeh)
            {
                return DomenskiRezultat<Zapis>.Neuspeh(ponavljanje.Greska!.Kod, ponavljanje.Greska.Poruka);
            }

            return DomenskiRezultat<Zapis>.Ok(ponavljanje.Vrednost!);
        }

        return DomenskiRezultat<Zapis>.Neuspeh("repo.nepoznat_tip", $"Nepoznat tip zapisa: '{Tip}'.");
    }
}
