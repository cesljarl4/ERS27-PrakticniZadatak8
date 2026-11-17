using Application.Interfaces.Formatters;
using Domain.Models;

namespace Presentation.Formatting;

// Sazeti prikaz F3: tip, ID, naziv i status.
public sealed class SazetiFormater : IZapisFormater
{
    public string Naziv => "Sazeti prikaz";

    public string Formatiraj(Zapis zapis) =>
        $"[{ZapisOznake.Tip(zapis)}] {zapis.Id} | {zapis.Naziv} | {ZapisOznake.Status(zapis)}";
}
