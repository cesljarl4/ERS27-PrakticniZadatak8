using Domain.Common;
using Domain.Enums;

namespace Domain.Models;

public abstract class Zapis
{
    protected Zapis(Guid id, string naziv, string opis, string oblast, int trajanjeMin,
                    DateTimeOffset kreiranoUtc, StatusObrade status)
    {
        Id = id;
        Naziv = naziv;
        Opis = opis;
        Oblast = oblast;
        ProcenjenoTrajanjeMin = trajanjeMin;
        KreiranoUtc = kreiranoUtc;
        Status = status;
    }

    public Guid Id {get;}
    public string Naziv {get;}
    public string Opis {get;}
    public string Oblast {get;}
    public int ProcenjenoTrajanjeMin {get;}
    public DateTimeOffset KreiranoUtc {get; }
    public StatusObrade Status {get; private set;}

    // F5: dozvoljeno je samo iz Zapocetog stanja.
    public DomenskiRezultat Zavrsi()
    {
        if (Status != StatusObrade.Zapoceta)
		return DomenskiRezultat.Neuspeh("zapis.nije_zapoceta", "Aktivnost nije u stanju 'zapoceta'.");

        Status = StatusObrade.Zavrsena;
        return DomenskiRezultat.Ok();
    }
    // F6: dozvoljeno je samo iz Zavrsenog stanja.
    public DomenskiRezultat PonovoOtvori()
    {
        if (Status != StatusObrade.Zavrsena)
		return DomenskiRezultat.Neuspeh("zapis.nije_zavrsena", "Aktivnost nije bila zavrsena.");

        Status = StatusObrade.Zapoceta;
        return DomenskiRezultat.Ok();
    }
}
