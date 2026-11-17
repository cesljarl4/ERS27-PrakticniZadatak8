using Application.Interfaces.Services;
using Application.Services;
using Database.Repositories;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;
using Shared;
using Tests.Pomocno;

namespace Tests.Servisi;

// F3-F6 nad memorijskim repozitorijumom: pregled, pretraga, zavrsavanje i ponovno otvaranje.
[TestFixture]
public sealed class PregledIStatusServisTests
{
    private InMemoryZapisiRepozitorijum _repo = null!;
    private ZapisServis _unos = null!;
    private ZapisPregledServis _pregled = null!;
    private ZapisStatusServis _status = null!;

    [SetUp]
    public void Pripremi()
    {
        var logger = new Mock<ILoggerServis>();
        _repo = new InMemoryZapisiRepozitorijum();
        _unos = new ZapisServis(_repo, logger.Object, new SystemClock());
        _pregled = new ZapisPregledServis(_repo);
        _status = new ZapisStatusServis(_repo, _repo, logger.Object);
    }

    private StatusObrade StatusZapisa(Guid id) => _repo.Pronadji(id).Value!.Status;

    [Test]
    public void Pregled_NemaZapisa_PreostaloVremeJeNula()
    {
        Result<PregledRezultat> rezultat = _pregled.Pregled();

        Assert.That(rezultat.Value!.Zapisi, Is.Empty);
        Assert.That(rezultat.Value.PreostaloMinuta, Is.EqualTo(0));
    }

    [Test]
    public void Pregled_ZavrsenaAktivnost_NeUracunavaSeUPreostaloVreme()
    {
        Guid tema = _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4").Value;
        _unos.DodajPonavljanje("Izvodi", "Opis", "Matematika", 30, TestPodaci.DatumUcenja);
        Assert.That(_pregled.Pregled().Value!.PreostaloMinuta, Is.EqualTo(90));

        _status.Zavrsi(tema);

        Assert.That(_pregled.Pregled().Value!.PreostaloMinuta, Is.EqualTo(30));
    }

    [TestCase("matematika")]
    [TestCase("MATEMATIKA")]
    public void PretraziPoOblasti_RazlicitaVelikaMalaSlova_NalaziIsteZapise(string upit)
    {
        _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4");
        _unos.DodajNovuTemu("Kinematika", "Opis", "Fizika", 40, "Poglavlje 2");

        Result<IReadOnlyList<Zapis>> rezultat = _pregled.PretraziPoOblasti(upit);

        Assert.That(rezultat.Value, Has.Count.EqualTo(1));
        Assert.That(rezultat.Value![0].Naziv, Is.EqualTo("Integrali"));
    }

    [Test]
    public void PretraziPoOblasti_NemaPodudaranja_VracaPraznuListu()
    {
        _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4");

        Result<IReadOnlyList<Zapis>> rezultat = _pregled.PretraziPoOblasti("Hemija");

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(rezultat.Value, Is.Empty);
    }

    [Test]
    public void Zavrsi_ZapocetaAktivnost_PostajeZavrsena()
    {
        Guid id = _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4").Value;

        Result rezultat = _status.Zavrsi(id);

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(StatusZapisa(id), Is.EqualTo(StatusObrade.Zavrsena));
    }

    [Test]
    public void Zavrsi_VecZavrsenaAktivnost_VracaNeuspeh()
    {
        Guid id = _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4").Value;
        _status.Zavrsi(id);

        Result rezultat = _status.Zavrsi(id);

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(StatusZapisa(id), Is.EqualTo(StatusObrade.Zavrsena));
    }

    [Test]
    public void Zavrsi_NepostojeciIdentifikator_VracaNeuspeh()
    {
        Result rezultat = _status.Zavrsi(Guid.NewGuid());

        Assert.That(rezultat.IsFailure, Is.True);
    }

    [Test]
    public void PonovoOtvori_ZavrsenaAktivnost_PostajeZapoceta()
    {
        Guid id = _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4").Value;
        _status.Zavrsi(id);

        Result rezultat = _status.PonovoOtvori(id);

        Assert.That(rezultat.IsSuccess, Is.True);
        Assert.That(StatusZapisa(id), Is.EqualTo(StatusObrade.Zapoceta));
    }

    [Test]
    public void PonovoOtvori_ZapocetaAktivnost_VracaNeuspehBezPromene()
    {
        Guid id = _unos.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4").Value;

        Result rezultat = _status.PonovoOtvori(id);

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(StatusZapisa(id), Is.EqualTo(StatusObrade.Zapoceta));
    }
}
