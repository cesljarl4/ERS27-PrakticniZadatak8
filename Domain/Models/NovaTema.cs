using Domain.Common;
using Domain.Enums;
using Domain.Rules;

namespace Domain.Models;

public sealed class NovaTema : Zapis
{
    private NovaTema(Guid id, string naziv, string opis, string oblast, int trajanjeMin, DateTimeOffset kreiranoUtc, StatusObrade status, string poglavlje) 
    : base(id, naziv, opis, oblast, trajanjeMin, kreiranoUtc, status)
    {
        Poglavlje = poglavlje;
    }

    public string Poglavlje { get; }

    // Vreme kreiranja se prosledjuje spolja (Domain ne cita sistemski sat).
    public static DomenskiRezultat<NovaTema> Kreiraj(
	string? naziv, string? opis, string? oblast, int trajanjeMin, string? poglavlje,
        DateTimeOffset kreiranoUtc, Guid? id = null, StatusObrade status = StatusObrade.Zapoceta)
    {
        DomenskiRezultat provera = ZapisPravila.Proveri(naziv, opis, oblast, trajanjeMin);
        if (!provera.Uspeh)
        	return DomenskiRezultat<NovaTema>.Neuspeh(provera.Greska!.Kod, provera.Greska.Poruka);

        if (string.IsNullOrWhiteSpace(poglavlje))
        
        	return DomenskiRezultat<NovaTema>.Neuspeh("zapis.poglavlje_prazno", "Poglavlje je obavezno za novu temu.");

        return DomenskiRezultat<NovaTema>.Ok(new NovaTema(
		id ?? Guid.NewGuid(), naziv!.Trim(), opis!.Trim(), oblast!.Trim(), trajanjeMin,
		kreiranoUtc, status, poglavlje.Trim()));
    }
}
