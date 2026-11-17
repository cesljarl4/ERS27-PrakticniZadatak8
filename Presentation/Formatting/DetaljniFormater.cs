using System.Globalization;
using Application.Interfaces.Formatters;
using Domain.Models;

namespace Presentation.Formatting;

// Detaljni prikaz F3: sve iz sazetog, plus oblast, trajanje, vreme kreiranja (UTC),
// kao i "poglavlje" (nova tema) odnosno "datum prethodnog ucenja" (ponavljanje).
public sealed class DetaljniFormater : IZapisFormater
{
    public string Naziv => "Detaljni prikaz";

    public string Formatiraj(Zapis zapis)
    {
        string kreirano = zapis.KreiranoUtc.ToString(ZapisOznake.FormatDatumVreme, CultureInfo.InvariantCulture);
        string osnovno = $"[{ZapisOznake.Tip(zapis)}] {zapis.Id} | {zapis.Naziv} | {ZapisOznake.Status(zapis)}"
            + $" | oblast: {zapis.Oblast} | trajanje: {zapis.ProcenjenoTrajanjeMin} min"
            + $" | kreirano (UTC): {kreirano} | opis: {zapis.Opis}";

        string dodatak = zapis switch
        {
            NovaTema nova => $" | poglavlje: {nova.Poglavlje}",
            Ponavljanje ponavljanje =>
                $" | datum prethodnog ucenja: {ponavljanje.DatumPrethodnogUcenja.ToString(ZapisOznake.FormatDatum, CultureInfo.InvariantCulture)}",
            _ => string.Empty
        };

        return osnovno + dodatak;
    }
}
