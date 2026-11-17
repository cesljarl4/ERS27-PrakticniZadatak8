using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Models;
using Infrastructure.Services;
using Moq;
using NUnit.Framework;
using Shared;
using Tests.Pomocno;

namespace Tests.Servisi;

// F1 i F2 uz laznu (mock) implementaciju repozitorijuma i loggera, bez datoteka.
[TestFixture]
public sealed class ZapisServisTests
{
    private Mock<IZapisiUpis> _upis = null!;
    private Mock<ILoggerServis> _logger = null!;
    private ZapisServis _servis = null!;

    [SetUp]
    public void Pripremi()
    {
        _upis = new Mock<IZapisiUpis>();
        _logger = new Mock<ILoggerServis>();
        _servis = new ZapisServis(_upis.Object, _logger.Object, new SystemClock());
    }

    [Test]
    public void DodajNovuTemu_IspravniPodaci_CuvaZapisILogujeUspeh()
    {
        _upis.Setup(u => u.Sacuvaj(It.IsAny<Zapis>())).Returns(Result.Success());

        Result<Guid> rezultat = _servis.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4");

        Assert.That(rezultat.IsSuccess, Is.True);
        _upis.Verify(u => u.Sacuvaj(It.Is<NovaTema>(t => t.Id == rezultat.Value)), Times.Once);
        _logger.Verify(l => l.Loguj(It.Is<string>(p => p.StartsWith("USPEH"))), Times.Once);
    }

    [Test]
    public void DodajPonavljanje_IspravniPodaci_CuvaZapis()
    {
        _upis.Setup(u => u.Sacuvaj(It.IsAny<Zapis>())).Returns(Result.Success());

        Result<Guid> rezultat = _servis.DodajPonavljanje("Izvodi", "Opis", "Matematika", 30, TestPodaci.DatumUcenja);

        Assert.That(rezultat.IsSuccess, Is.True);
        _upis.Verify(u => u.Sacuvaj(It.IsAny<Ponavljanje>()), Times.Once);
    }

    [TestCase(4)]
    [TestCase(301)]
    public void DodajNovuTemu_TrajanjeVanOpsega_NeCuvaZapisILogujeNeuspeh(int trajanje)
    {
        Result<Guid> rezultat = _servis.DodajNovuTemu("Integrali", "Opis", "Matematika", trajanje, "Poglavlje 4");

        Assert.That(rezultat.IsFailure, Is.True);
        _upis.Verify(u => u.Sacuvaj(It.IsAny<Zapis>()), Times.Never);
        _logger.Verify(l => l.Loguj(It.Is<string>(p => p.StartsWith("NEUSPEH"))), Times.Once);
    }

    [Test]
    public void DodajPonavljanje_BezDatuma_NeCuvaZapis()
    {
        Result<Guid> rezultat = _servis.DodajPonavljanje("Izvodi", "Opis", "Matematika", 30, null);

        Assert.That(rezultat.IsFailure, Is.True);
        _upis.Verify(u => u.Sacuvaj(It.IsAny<Zapis>()), Times.Never);
    }

    [Test]
    public void DodajNovuTemu_RepozitorijumVracaGresku_VracaNeuspeh()
    {
        _upis.Setup(u => u.Sacuvaj(It.IsAny<Zapis>())).Returns(Result.Failure("repo.upis_neuspesan", "Upis nije uspeo."));

        Result<Guid> rezultat = _servis.DodajNovuTemu("Integrali", "Opis", "Matematika", 60, "Poglavlje 4");

        Assert.That(rezultat.IsFailure, Is.True);
        Assert.That(rezultat.Error?.Code, Is.EqualTo("repo.upis_neuspesan"));
    }
}
