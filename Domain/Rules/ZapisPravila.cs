using Domain.Common;

namespace Domain.Rules;

// Pravilo P3 i provera obaveznih tekstualnih polja na jednom mestu (SRP).
public static class ZapisPravila
{
    public const int MinTrajanje = 5;
    public const int MaxTrajanje = 300;

    public static DomenskiRezultat Proveri(string? naziv, string? opis, string? oblast, int trajanjeMin)
    {
        if (string.IsNullOrWhiteSpace(naziv))
        {
            return DomenskiRezultat.Neuspeh("zapis.naziv_prazan", "Naziv ne sme biti prazan.");
        }

        if (string.IsNullOrWhiteSpace(opis))
        {
            return DomenskiRezultat.Neuspeh("zapis.opis_prazan", "Opis ne sme biti prazan.");
        }

        if (string.IsNullOrWhiteSpace(oblast))
        {
            return DomenskiRezultat.Neuspeh("zapis.oblast_prazna", "Oblast ne sme biti prazna.");
        }

        if (trajanjeMin < MinTrajanje || trajanjeMin > MaxTrajanje)
        {
            return DomenskiRezultat.Neuspeh("zapis.trajanje_van_opsega",
                $"Procenjeno trajanje mora biti izmedju {MinTrajanje} i {MaxTrajanje} minuta.");
        }

        return DomenskiRezultat.Ok();
    }
}
