using Application.Interfaces.Formatters;
using Application.Services;
using Database.Repositories;
using Domain.Models;
using NUnit.Framework;
using Presentation.Formatting;
using Tests.Pomocno;

namespace Tests.Prikaz;

// F3: oba formata prikaza i treca implementacija bez izmene servisa (OCP).
[TestFixture]
public sealed class FormaterTests
{
    [Test]
    public void Formatiraj_SazetiPrikaz_SadrziTipIdNazivIStatus()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu("Integrali", "Matematika", 60, "Poglavlje 4");

        string tekst = new SazetiFormater().Formatiraj(tema);

        Assert.That(tekst, Does.Contain("Nova tema"));
        Assert.That(tekst, Does.Contain(tema.Id.ToString()));
        Assert.That(tekst, Does.Contain("Integrali"));
        Assert.That(tekst, Does.Contain("zapoceta"));
        Assert.That(tekst, Does.Not.Contain("Poglavlje 4"));
    }

    [Test]
    public void Formatiraj_DetaljniPrikazNoveTeme_SadrziPoglavlje()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu("Integrali", "Matematika", 60, "Poglavlje 4");

        string tekst = new DetaljniFormater().Formatiraj(tema);

        Assert.That(tekst, Does.Contain("oblast: Matematika"));
        Assert.That(tekst, Does.Contain("poglavlje: Poglavlje 4"));
    }

    [Test]
    public void Formatiraj_DetaljniPrikazPonavljanja_SadrziDatumPrethodnogUcenja()
    {
        Ponavljanje ponavljanje = TestPodaci.KreirajPonavljanje("Izvodi");

        string tekst = new DetaljniFormater().Formatiraj(ponavljanje);

        Assert.That(tekst, Does.Contain("datum prethodnog ucenja: 15.02.2026"));
    }

    // Treca implementacija je definisana u testu; Domain i Application se ne menjaju.
    private sealed class SamoNazivFormater : IZapisFormater
    {
        public string Naziv => "Samo naziv";

        public string Formatiraj(Zapis zapis) => zapis.Naziv;
    }

    [Test]
    public void Formatiraj_TrecaImplementacija_RadiBezIzmeneServisa()
    {
        var repo = new InMemoryZapisiRepozitorijum();
        repo.Sacuvaj(TestPodaci.KreirajNovuTemu("Integrali"));
        IZapisFormater formater = new SamoNazivFormater();

        var pregled = new ZapisPregledServis(repo).Pregled().Value!;

        Assert.That(pregled.Zapisi.Select(formater.Formatiraj), Is.EqualTo(new[] { "Integrali" }));
    }
}
