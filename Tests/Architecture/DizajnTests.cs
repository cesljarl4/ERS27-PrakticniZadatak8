using Application.Interfaces.Repositories;
using Application.Services;
using Database.Repositories;
using Infrastructure.Services;
using NUnit.Framework;
using Presentation.Composition;

namespace Tests.Architecture;

// Provere ISP i DIP, kao i izbora repozitorijuma i loggera u AppContainer (S5).
[TestFixture]
public sealed class DizajnTests
{
    [Test]
    public void ZapisPregledServis_Konstruktor_ZavisiSamoOdInterfejsaZaCitanje()
    {
        var parametri = typeof(ZapisPregledServis).GetConstructors().Single().GetParameters();

        Assert.That(parametri, Has.Length.EqualTo(1));
        Assert.That(parametri[0].ParameterType, Is.EqualTo(typeof(IZapisiCitanje)));
    }

    [TestCase(typeof(ZapisServis))]
    [TestCase(typeof(ZapisPregledServis))]
    [TestCase(typeof(ZapisStatusServis))]
    public void PoslovniServisi_Konstruktor_PrimaSamoInterfejse(Type servis)
    {
        var parametri = servis.GetConstructors().Single().GetParameters();

        Assert.That(parametri.All(p => p.ParameterType.IsInterface), Is.True);
    }

    [TestCase("in-memory", typeof(InMemoryZapisiRepozitorijum))]
    [TestCase("json", typeof(JsonZapisiRepozitorijum))]
    public void AppContainer_Create_BiraRepozitorijumPremaRezimu(string rezim, Type ocekivani)
    {
        AppContainer container = AppContainer.Create(rezim);

        Assert.That(container.Repozitorijum, Is.InstanceOf(ocekivani));
    }

    [TestCase("file", typeof(FileLoggerServis))]
    [TestCase("console", typeof(ConsoleLoggerServis))]
    public void AppContainer_Create_BiraLoggerPremaRezimu(string logerRezim, Type ocekivani)
    {
        AppContainer container = AppContainer.Create("in-memory", logerRezim);

        Assert.That(container.Logger, Is.InstanceOf(ocekivani));
    }
}
