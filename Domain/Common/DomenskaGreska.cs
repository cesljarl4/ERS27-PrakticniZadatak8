namespace Domain.Common;

public sealed class DomenskaGreska
{
    public DomenskaGreska(string kod, string poruka)
    {
        Kod = kod;
        Poruka = poruka;
    }

    public string Kod { get; }
    public string Poruka { get; }
}
