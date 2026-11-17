using Domain.Common;
using Domain.Enums;
using Domain.Rules;

namespace Domain.Models;

public sealed class Ponavljanje : Zapis
{
    private Ponavljanje(Guid id, string naziv, string opis, string oblast, int trajanjeMin, DateTimeOffset kreiranoUtc, StatusObrade status, DateOnly datumPrethodnogUcenja)
    : base(id, naziv, opis, oblast, trajanjeMin, kreiranoUtc, status)
    {
	DatumPrethodnogUcenja = datumPrethodnogUcenja;
    }

    public DateOnly DatumPrethodnogUcenja { get; }

    // Vreme kreiranja se prosledjuje spolja (Domain ne cita sistemski sat).
    public static DomenskiRezultat<Ponavljanje> Kreiraj(
        string? naziv, string? opis, string? oblast, int trajanjeMin, DateOnly? datumPrethodnogUcenja,
        DateTimeOffset kreiranoUtc, Guid? id = null, StatusObrade status = StatusObrade.Zapoceta)
    {
        DomenskiRezultat provera = ZapisPravila.Proveri(naziv, opis, oblast, trajanjeMin);
        if (!provera.Uspeh)
		return DomenskiRezultat<Ponavljanje>.Neuspeh(provera.Greska!.Kod, provera.Greska.Poruka);

        if (datumPrethodnogUcenja is null)
		return DomenskiRezultat<Ponavljanje>.Neuspeh("zapis.datum_prazan", "Datum prethodnog ucenja je obavezan.");

        return DomenskiRezultat<Ponavljanje>.Ok(new Ponavljanje(
		id ?? Guid.NewGuid(), naziv!.Trim(), opis!.Trim(), oblast!.Trim(), trajanjeMin,
		kreiranoUtc, status, datumPrethodnogUcenja.Value));
    }
}
