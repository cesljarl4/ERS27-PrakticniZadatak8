using Domain.Models;

namespace Tests.Pomocno;

// Gotovi ispravni zapisi za testove.
internal static class TestPodaci
{
    public static readonly DateTimeOffset Vreme = new(2026, 3, 1, 10, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly DatumUcenja = new(2026, 2, 15);

    public static NovaTema KreirajNovuTemu(
        string naziv = "Integrali",
        string oblast = "Matematika",
        int trajanje = 60,
        string poglavlje = "Poglavlje 4",
        DateTimeOffset? kreirano = null)
    {
        var rezultat = NovaTema.Kreiraj(naziv, "Opis teme", oblast, trajanje, poglavlje, kreirano ?? Vreme);
        return rezultat.Vrednost!;
    }

    public static Ponavljanje KreirajPonavljanje(
        string naziv = "Izvodi",
        string oblast = "Matematika",
        int trajanje = 30,
        DateTimeOffset? kreirano = null)
    {
        var rezultat = Ponavljanje.Kreiraj(naziv, "Opis ponavljanja", oblast, trajanje, DatumUcenja, kreirano ?? Vreme);
        return rezultat.Vrednost!;
    }
}
