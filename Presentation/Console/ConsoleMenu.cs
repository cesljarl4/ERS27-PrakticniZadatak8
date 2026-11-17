using System.Globalization;
using Application.Interfaces.Formatters;
using Application.Services;
using Domain.Models;
using Presentation.Composition;
using Presentation.Interfaces;
using Shared;

namespace Presentation.Console;

// Konzolni meni za F1-F6. Cita unos, poziva servise i ispisuje rezultat.
// Poslovna pravila su u Domain-u; ovde se tekst samo pretvara u brojeve i datume.
public sealed class ConsoleMenu
{
    private readonly IConsoleIO _io;
    private readonly AppContainer _container;

    public ConsoleMenu(IConsoleIO io, AppContainer container)
    {
        _io = io;
        _container = container;
    }

    public void Run()
    {
        _io.WriteLine("Plan samostalnog ucenja");

        while (true)
        {
            _io.WriteLine();
            _io.WriteLine("1. Dodaj novu temu");
            _io.WriteLine("2. Dodaj ponavljanje");
            _io.WriteLine("3. Pregled svih aktivnosti");
            _io.WriteLine("4. Pretraga po oblasti");
            _io.WriteLine("5. Oznaci aktivnost kao zavrsenu");
            _io.WriteLine("6. Ponovo otvori aktivnost");
            _io.WriteLine("0. Izlaz");

            string izbor = Pitaj("Izbor: ");
            if (izbor == "0" || izbor == "")
            {
                break;
            }

            Obradi(izbor);
        }

        _io.WriteLine("Dovidjenja.");
    }

    private void Obradi(string izbor)
    {
        switch (izbor)
        {
            case "1":
                DodajNovuTemu();
                break;
            case "2":
                DodajPonavljanje();
                break;
            case "3":
                PrikaziPregled();
                break;
            case "4":
                Pretrazi();
                break;
            case "5":
                PromeniStatus(zavrsi: true);
                break;
            case "6":
                PromeniStatus(zavrsi: false);
                break;
            default:
                _io.WriteLine("Nepoznata opcija.");
                break;
        }
    }

    // F1
    private void DodajNovuTemu()
    {
        string naziv = Pitaj("Naziv: ");
        string opis = Pitaj("Opis: ");
        string oblast = Pitaj("Oblast: ");
        string poglavlje = Pitaj("Poglavlje: ");
        if (!ProcitajTrajanje(out int trajanje))
        {
            return;
        }

        PrikaziRezultatDodavanja(_container.Servis.DodajNovuTemu(naziv, opis, oblast, trajanje, poglavlje));
    }

    // F2
    private void DodajPonavljanje()
    {
        string naziv = Pitaj("Naziv: ");
        string opis = Pitaj("Opis: ");
        string oblast = Pitaj("Oblast: ");
        string unosDatuma = Pitaj("Datum prethodnog ucenja (dd.MM.yyyy): ");

        // Prazan unos ide servisu kao null, a servis ga odbija jer je datum obavezan.
        DateOnly? datum = null;
        if (unosDatuma != "")
        {
            if (!DateOnly.TryParseExact(unosDatuma, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly procitan))
            {
                _io.WriteLine("Greska: datum nije u ispravnom formatu (dd.MM.yyyy).");
                return;
            }

            datum = procitan;
        }

        if (!ProcitajTrajanje(out int trajanje))
        {
            return;
        }

        PrikaziRezultatDodavanja(_container.Servis.DodajPonavljanje(naziv, opis, oblast, trajanje, datum));
    }

    // F3
    private void PrikaziPregled()
    {
        Result<PregledRezultat> rezultat = _container.PregledServis.Pregled();
        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error!.Message}");
            return;
        }

        PregledRezultat pregled = rezultat.Value!;
        if (pregled.Zapisi.Count == 0)
        {
            _io.WriteLine("Lista je prazna.");
        }
        else
        {
            IspisiZapise(pregled.Zapisi);
        }

        _io.WriteLine($"Preostalo planirano vreme: {pregled.PreostaloMinuta} min");
    }

    // F4
    private void Pretrazi()
    {
        string oblast = Pitaj("Naziv oblasti: ");
        Result<IReadOnlyList<Zapis>> rezultat = _container.PregledServis.PretraziPoOblasti(oblast);
        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error!.Message}");
            return;
        }

        if (rezultat.Value!.Count == 0)
        {
            _io.WriteLine($"Nema rezultata za oblast '{oblast}'.");
            return;
        }

        IspisiZapise(rezultat.Value);
    }

    // F5 i F6
    private void PromeniStatus(bool zavrsi)
    {
        string unos = Pitaj("Identifikator aktivnosti (iz pregleda): ");
        if (!Guid.TryParse(unos, out Guid id))
        {
            _io.WriteLine("Greska: identifikator nije ispravan.");
            return;
        }

        Result rezultat = zavrsi ? _container.StatusServis.Zavrsi(id) : _container.StatusServis.PonovoOtvori(id);
        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error!.Message}");
            return;
        }

        _io.WriteLine(zavrsi ? "Aktivnost je zavrsena." : "Aktivnost je ponovo otvorena (zapoceta).");
    }

    // Korisnik bira sazeti (1) ili detaljni (2) prikaz; Enter znaci sazeti.
    private void IspisiZapise(IReadOnlyList<Zapis> zapisi)
    {
        string izbor = Pitaj("Prikaz: 1. Sazeti, 2. Detaljni (Enter = 1): ");
        IZapisFormater formater = izbor == "2" ? _container.Formateri[1] : _container.Formateri[0];

        foreach (Zapis zapis in zapisi)
        {
            _io.WriteLine(formater.Formatiraj(zapis));
        }
    }

    private bool ProcitajTrajanje(out int trajanje)
    {
        string unos = Pitaj("Procenjeno trajanje u minutima (5-300): ");
        if (int.TryParse(unos, NumberStyles.Integer, CultureInfo.InvariantCulture, out trajanje))
        {
            return true;
        }

        _io.WriteLine("Greska: trajanje mora biti ceo broj.");
        return false;
    }

    private void PrikaziRezultatDodavanja(Result<Guid> rezultat)
    {
        if (rezultat.IsFailure)
        {
            _io.WriteLine($"Greska: {rezultat.Error!.Message}");
            return;
        }

        _io.WriteLine($"Sacuvano (status: zapoceta). Identifikator: {rezultat.Value}");
    }

    private string Pitaj(string poruka)
    {
        _io.Write(poruka);
        return _io.ReadLine().Trim();
    }
}
