using Domain.Enums;
using Domain.Models;
using Domain.Rules;
using NUnit.Framework;
using Tests.Pomocno;

namespace Tests.Domen;

[TestFixture]
public sealed class DomenTests
{
    [TestCase(4, false)]
    [TestCase(5, true)]
    [TestCase(300, true)]
    [TestCase(301, false)]
    public void Proveri_GranicneVrednostiTrajanja_PrihvataSamoOd5Do300(int trajanje, bool ocekivanoUspeh)
    {
        var rezultat = ZapisPravila.Proveri("Naziv", "Opis", "Oblast", trajanje);

        Assert.That(rezultat.Uspeh, Is.EqualTo(ocekivanoUspeh));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Proveri_PrazanNaziv_VracaNeuspeh(string naziv)
    {
        var rezultat = ZapisPravila.Proveri(naziv, "Opis", "Oblast", 60);

        Assert.That(rezultat.Greska?.Kod, Is.EqualTo("zapis.naziv_prazan"));
    }

    [Test]
    public void Proveri_PrazanOpis_VracaNeuspeh()
    {
        var rezultat = ZapisPravila.Proveri("Naziv", "", "Oblast", 60);

        Assert.That(rezultat.Greska?.Kod, Is.EqualTo("zapis.opis_prazan"));
    }

    [Test]
    public void Kreiraj_NovaTemaSaSvimPodacima_CuvaPoljaIStatusZapoceta()
    {
        var rezultat = NovaTema.Kreiraj("Integrali", "Opis", "Matematika", 60, "Poglavlje 4", TestPodaci.Vreme);

        Assert.That(rezultat.Uspeh, Is.True);
        Assert.That(rezultat.Vrednost!.Poglavlje, Is.EqualTo("Poglavlje 4"));
        Assert.That(rezultat.Vrednost.Status, Is.EqualTo(StatusObrade.Zapoceta));
    }

    [Test]
    public void Kreiraj_NovaTemaBezPoglavlja_VracaNeuspeh()
    {
        var rezultat = NovaTema.Kreiraj("Integrali", "Opis", "Matematika", 60, "", TestPodaci.Vreme);

        Assert.That(rezultat.Uspeh, Is.False);
        Assert.That(rezultat.Greska?.Kod, Is.EqualTo("zapis.poglavlje_prazno"));
    }

    [Test]
    public void Kreiraj_PonavljanjeSaDatumom_CuvaDatumPrethodnogUcenja()
    {
        var rezultat = Ponavljanje.Kreiraj("Izvodi", "Opis", "Matematika", 30, TestPodaci.DatumUcenja, TestPodaci.Vreme);

        Assert.That(rezultat.Uspeh, Is.True);
        Assert.That(rezultat.Vrednost!.DatumPrethodnogUcenja, Is.EqualTo(TestPodaci.DatumUcenja));
    }

    [Test]
    public void Kreiraj_PonavljanjeBezDatuma_VracaNeuspeh()
    {
        var rezultat = Ponavljanje.Kreiraj("Izvodi", "Opis", "Matematika", 30, null, TestPodaci.Vreme);

        Assert.That(rezultat.Uspeh, Is.False);
        Assert.That(rezultat.Greska?.Kod, Is.EqualTo("zapis.datum_prazan"));
    }

    [Test]
    public void Zavrsi_ZapocetaAktivnost_PostajeZavrsena()
    {
        Zapis zapis = TestPodaci.KreirajNovuTemu();

        var rezultat = zapis.Zavrsi();

        Assert.That(rezultat.Uspeh, Is.True);
        Assert.That(zapis.Status, Is.EqualTo(StatusObrade.Zavrsena));
    }

    [Test]
    public void Zavrsi_VecZavrsenaAktivnost_VracaNeuspehBezPromene()
    {
        Zapis zapis = TestPodaci.KreirajNovuTemu();
        zapis.Zavrsi();

        var rezultat = zapis.Zavrsi();

        Assert.That(rezultat.Uspeh, Is.False);
        Assert.That(zapis.Status, Is.EqualTo(StatusObrade.Zavrsena));
    }

    [Test]
    public void PonovoOtvori_ZavrsenaAktivnost_PostajeZapoceta()
    {
        Zapis zapis = TestPodaci.KreirajNovuTemu();
        zapis.Zavrsi();

        var rezultat = zapis.PonovoOtvori();

        Assert.That(rezultat.Uspeh, Is.True);
        Assert.That(zapis.Status, Is.EqualTo(StatusObrade.Zapoceta));
    }

    [Test]
    public void PonovoOtvori_ZapocetaAktivnost_VracaNeuspehBezPromene()
    {
        Zapis zapis = TestPodaci.KreirajNovuTemu();

        var rezultat = zapis.PonovoOtvori();

        Assert.That(rezultat.Uspeh, Is.False);
        Assert.That(zapis.Status, Is.EqualTo(StatusObrade.Zapoceta));
    }
}
