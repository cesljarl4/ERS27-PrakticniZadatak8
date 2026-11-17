using Domain.Enums;
using Domain.Models;

namespace Presentation.Formatting;

// Zajednicki tekstovi za formatere (tip i status zapisa).
internal static class ZapisOznake
{
    public const string FormatDatumVreme = "dd.MM.yyyy HH:mm";
    public const string FormatDatum = "dd.MM.yyyy";

    public static string Tip(Zapis zapis) => zapis switch
    {
        NovaTema => "Nova tema",
        Ponavljanje => "Ponavljanje",
        _ => "Zapis"
    };

    public static string Status(Zapis zapis) =>
        zapis.Status == StatusObrade.Zavrsena ? "zavrsena" : "zapoceta";
}
