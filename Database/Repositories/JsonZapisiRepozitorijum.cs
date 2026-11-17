using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// TODO (grana 2): implementirati trajno cuvanje u JSON datoteci (polimorfni modeli).
public sealed class JsonZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly string _putanja;

    public JsonZapisiRepozitorijum(string putanja)
    {
        _putanja = putanja;
    }

    public Result Sacuvaj(Zapis zapis) =>
        Result.Failure("repo.nije_implementirano", "Implementirati JSON upis.");

    public Result Azuriraj(Zapis zapis) =>
        Result.Failure("repo.nije_implementirano", "Implementirati JSON azuriranje.");

    public Result<Zapis> Pronadji(Guid id) =>
        Result<Zapis>.Failure("repo.nije_implementirano", "Implementirati JSON citanje.");

    public Result<IReadOnlyList<Zapis>> Svi() =>
        Result<IReadOnlyList<Zapis>>.Failure("repo.nije_implementirano", "Implementirati pregled JSON zapisa.");
}
