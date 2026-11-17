using Application.Interfaces.Repositories;
using Database.Repositories;
using Domain.Enums;
using Domain.Models;
using NUnit.Framework;
using Shared;
using Tests.Pomocno;

namespace Tests.Repozitorijumi;

// Zajednicki testovi ugovora. Izvode ih klase za memoriju i za JSON, pa oba repozitorijuma
// prolaze iste testove (LSP).
public abstract class ZapisiRepozitorijumUgovorTests
{
    protected IZapisiRepozitorijum Repozitorijum { get; private set; } = null!;

    protected abstract IZapisiRepozitorijum NapraviRepozitorijum();

    [SetUp]
    public void PripremiRepozitorijum()
    {
        Repozitorijum = NapraviRepozitorijum();
    }

    [Test]
    public void Sacuvaj_NovaTema_PosleTogaSePronalaziSaSvimPoljima()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu("Integrali", "Matematika", 60, "Poglavlje 4");

        Result upis = Repozitorijum.Sacuvaj(tema);
        Result<Zapis> citanje = Repozitorijum.Pronadji(tema.Id);

        Assert.That(upis.IsSuccess, Is.True);
        Assert.That(citanje.Value, Is.InstanceOf<NovaTema>());
        var procitana = (NovaTema)citanje.Value!;
        Assert.That(procitana.Naziv, Is.EqualTo("Integrali"));
        Assert.That(procitana.Oblast, Is.EqualTo("Matematika"));
        Assert.That(procitana.ProcenjenoTrajanjeMin, Is.EqualTo(60));
        Assert.That(procitana.Poglavlje, Is.EqualTo("Poglavlje 4"));
        Assert.That(procitana.Status, Is.EqualTo(StatusObrade.Zapoceta));
    }

    [Test]
    public void Sacuvaj_Ponavljanje_PosleTogaSePronalaziSaSvimPoljima()
    {
        Ponavljanje ponavljanje = TestPodaci.KreirajPonavljanje("Izvodi", "Matematika", 30);

        Result upis = Repozitorijum.Sacuvaj(ponavljanje);
        Result<Zapis> citanje = Repozitorijum.Pronadji(ponavljanje.Id);

        Assert.That(upis.IsSuccess, Is.True);
        Assert.That(citanje.Value, Is.InstanceOf<Ponavljanje>());
        var procitano = (Ponavljanje)citanje.Value!;
        Assert.That(procitano.Naziv, Is.EqualTo("Izvodi"));
        Assert.That(procitano.DatumPrethodnogUcenja, Is.EqualTo(TestPodaci.DatumUcenja));
    }

    [Test]
    public void Svi_ObeVrsteZapisa_VracaSveZapise()
    {
        Repozitorijum.Sacuvaj(TestPodaci.KreirajNovuTemu());
        Repozitorijum.Sacuvaj(TestPodaci.KreirajPonavljanje());

        Result<IReadOnlyList<Zapis>> rezultat = Repozitorijum.Svi();

        Assert.That(rezultat.Value, Has.Count.EqualTo(2));
        Assert.That(rezultat.Value!.OfType<NovaTema>().Count(), Is.EqualTo(1));
        Assert.That(rezultat.Value!.OfType<Ponavljanje>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void Pronadji_NepostojeciIdentifikator_VracaNeuspeh()
    {
        Result<Zapis> rezultat = Repozitorijum.Pronadji(Guid.NewGuid());

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(rezultat.Error?.Code, Is.EqualTo("repo.nije_pronadjen"));
    }

    [Test]
    public void Azuriraj_NepostojeciZapis_VracaNeuspehINeDodajeZapis()
    {
        Result rezultat = Repozitorijum.Azuriraj(TestPodaci.KreirajNovuTemu());

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(Repozitorijum.Svi().Value, Is.Empty);
    }

    [Test]
    public void Sacuvaj_IstiIdentifikatorDvaPuta_VracaNeuspehINeDuplira()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu();
        Repozitorijum.Sacuvaj(tema);

        Result drugi = Repozitorijum.Sacuvaj(tema);

        Assert.That(drugi.IsFailure, Is.True);
        Assert.That(Repozitorijum.Svi().Value, Has.Count.EqualTo(1));
    }

    [Test]
    public void Azuriraj_PromenjenStatus_VidiSePriSledecemCitanju()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu();
        Repozitorijum.Sacuvaj(tema);
        tema.Zavrsi();

        Result rezultat = Repozitorijum.Azuriraj(tema);

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(Repozitorijum.Pronadji(tema.Id).Value!.Status, Is.EqualTo(StatusObrade.Zavrsena));
    }
}

[TestFixture]
public sealed class InMemoryZapisiRepozitorijumTests : ZapisiRepozitorijumUgovorTests
{
    protected override IZapisiRepozitorijum NapraviRepozitorijum() => new InMemoryZapisiRepozitorijum();
}

// Isti testovi ugovora (nasledjeni), plus provere trajnosti podataka u JSON datoteci.
[TestFixture]
public sealed class JsonZapisiRepozitorijumTests : ZapisiRepozitorijumUgovorTests
{
    private string _putanja = string.Empty;

    protected override IZapisiRepozitorijum NapraviRepozitorijum()
    {
        _putanja = Path.Combine(Path.GetTempPath(), $"zapisi-{Guid.NewGuid():N}.json");
        return new JsonZapisiRepozitorijum(_putanja);
    }

    [TearDown]
    public void ObrisiDatoteku()
    {
        if (File.Exists(_putanja))
        {
            File.Delete(_putanja);
        }
    }

    [Test]
    public void Svi_NovaInstancaNadIstomDatotekom_VracaSacuvaneZapise()
    {
        NovaTema tema = TestPodaci.KreirajNovuTemu("Integrali", "Matematika", 60, "Poglavlje 4");
        Ponavljanje ponavljanje = TestPodaci.KreirajPonavljanje("Izvodi");
        Repozitorijum.Sacuvaj(tema);
        Repozitorijum.Sacuvaj(ponavljanje);

        var ponovoOtvoren = new JsonZapisiRepozitorijum(_putanja);

        Assert.That(ponovoOtvoren.Svi().Value, Has.Count.EqualTo(2));
        Assert.That(((NovaTema)ponovoOtvoren.Pronadji(tema.Id).Value!).Poglavlje, Is.EqualTo("Poglavlje 4"));
        Assert.That(((Ponavljanje)ponovoOtvoren.Pronadji(ponavljanje.Id).Value!).DatumPrethodnogUcenja, Is.EqualTo(TestPodaci.DatumUcenja));
    }

    [Test]
    public void Svi_NeispravanJson_VracaNeuspehINePrepisujeDatoteku()
    {
        const string sadrzaj = "{ ovo nije ispravan json";
        File.WriteAllText(_putanja, sadrzaj);

        Result<IReadOnlyList<Zapis>> citanje = Repozitorijum.Svi();
        Result upis = Repozitorijum.Sacuvaj(TestPodaci.KreirajNovuTemu());

        Assert.That(citanje.IsFailure, Is.True);
        Assert.That(upis.IsFailure, Is.True);
        Assert.That(File.ReadAllText(_putanja), Is.EqualTo(sadrzaj));
    }
}
