using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// Cuva zapise u JSON datoteci, pa su podaci dostupni i posle ponovnog pokretanja.
// Datoteka se cita pri svakoj operaciji. Neispravan JSON je neuspeh, a datoteka se tada ne prepisuje.
public sealed class JsonZapisiRepozitorijum : IZapisiRepozitorijum
{
    private static readonly JsonSerializerOptions Opcije = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _putanja;

    public JsonZapisiRepozitorijum(string putanja)
    {
        _putanja = putanja;
    }

    public Result Sacuvaj(Zapis zapis)
    {
        Result<List<Zapis>> ucitano = Ucitaj();
        if (ucitano.IsFailure)
        {
            return Result.Failure(ucitano.Error!.Code, ucitano.Error.Message);
        }

        List<Zapis> lista = ucitano.Value!;
        if (lista.Any(z => z.Id == zapis.Id))
        {
            return Result.Failure("repo.vec_postoji", "Zapis sa tim identifikatorom vec postoji.");
        }

        lista.Add(zapis);
        return Snimi(lista);
    }

    public Result Azuriraj(Zapis zapis)
    {
        Result<List<Zapis>> ucitano = Ucitaj();
        if (ucitano.IsFailure)
        {
            return Result.Failure(ucitano.Error!.Code, ucitano.Error.Message);
        }

        List<Zapis> lista = ucitano.Value!;
        int indeks = lista.FindIndex(z => z.Id == zapis.Id);
        if (indeks < 0)
        {
            return Result.Failure("repo.nije_pronadjen", "Zapis sa tim identifikatorom ne postoji.");
        }

        lista[indeks] = zapis;
        return Snimi(lista);
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        Result<List<Zapis>> ucitano = Ucitaj();
        if (ucitano.IsFailure)
        {
            return Result<Zapis>.Failure(ucitano.Error!.Code, ucitano.Error.Message);
        }

        Zapis? zapis = ucitano.Value!.FirstOrDefault(z => z.Id == id);
        if (zapis is null)
        {
            return Result<Zapis>.Failure("repo.nije_pronadjen", "Zapis sa tim identifikatorom ne postoji.");
        }

        return Result<Zapis>.Success(zapis);
    }

    public Result<IReadOnlyList<Zapis>> Svi()
    {
        Result<List<Zapis>> ucitano = Ucitaj();
        if (ucitano.IsFailure)
        {
            return Result<IReadOnlyList<Zapis>>.Failure(ucitano.Error!.Code, ucitano.Error.Message);
        }

        return Result<IReadOnlyList<Zapis>>.Success(ucitano.Value!.OrderBy(z => z.KreiranoUtc).ToList());
    }

    private Result<List<Zapis>> Ucitaj()
    {
        if (!File.Exists(_putanja))
        {
            return Result<List<Zapis>>.Success(new List<Zapis>());
        }

        try
        {
            string json = File.ReadAllText(_putanja);
            List<ZapisDto> dtoLista = string.IsNullOrWhiteSpace(json)
                ? new List<ZapisDto>()
                : JsonSerializer.Deserialize<List<ZapisDto>>(json, Opcije) ?? new List<ZapisDto>();

            var zapisi = new List<Zapis>();
            foreach (ZapisDto dto in dtoLista)
            {
                var zapis = dto.UZapis();
                if (!zapis.Uspeh)
                {
                    return Result<List<Zapis>>.Failure("repo.json_neispravan", $"JSON datoteka sadrzi neispravan zapis: {zapis.Greska!.Poruka}");
                }

                zapisi.Add(zapis.Vrednost!);
            }

            return Result<List<Zapis>>.Success(zapisi);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or JsonException)
        {
            return Result<List<Zapis>>.Failure("repo.citanje_neuspesno", $"Citanje JSON datoteke nije uspelo: {e.Message}");
        }
    }

    private Result Snimi(List<Zapis> lista)
    {
        try
        {
            string json = JsonSerializer.Serialize(lista.Select(ZapisDto.IzZapisa).ToList(), Opcije);
            File.WriteAllText(_putanja, json);
            return Result.Success();
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return Result.Failure("repo.upis_neuspesan", $"Upis u JSON datoteku nije uspeo: {e.Message}");
        }
    }
}
