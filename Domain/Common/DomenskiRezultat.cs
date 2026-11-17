namespace Domain.Common;

public sealed class DomenskiRezultat
{
    private DomenskiRezultat(DomenskaGreska? greska)
    {
        Greska = greska;
    }

    public bool Uspeh => Greska is null;
    public DomenskaGreska? Greska { get; }

    public static DomenskiRezultat Ok() => new(null);
    public static DomenskiRezultat Neuspeh(string kod, string poruka) => new(new DomenskaGreska(kod, poruka));
}

public sealed class DomenskiRezultat<T> where T : class
{
    private DomenskiRezultat(T? vrednost, DomenskaGreska? greska)
    {
        Vrednost = vrednost;
        Greska = greska;
    }

    public bool Uspeh => Greska is null;
    public T? Vrednost { get; }
    public DomenskaGreska? Greska { get; }

    public static DomenskiRezultat<T> Ok(T vrednost) => new(vrednost, null);
    public static DomenskiRezultat<T> Neuspeh(string kod, string poruka) => new(null, new DomenskaGreska(kod, poruka));
}
