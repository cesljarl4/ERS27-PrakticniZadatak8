using Application.Interfaces.Repositories;
using Domain.Models;
using Shared;

namespace Database.Repositories;

// Cuva zapise u memoriji; podaci nestaju kada se aplikacija ugasi.
public sealed class InMemoryZapisiRepozitorijum : IZapisiRepozitorijum
{
    private readonly Dictionary<Guid, Zapis> _zapisi = new();

    public Result Sacuvaj(Zapis zapis)
    {
        if (_zapisi.ContainsKey(zapis.Id))
        {
            return Result.Failure("repo.vec_postoji", "Zapis sa tim identifikatorom vec postoji.");
        }

        _zapisi[zapis.Id] = zapis;
        return Result.Success();
    }

    public Result Azuriraj(Zapis zapis)
    {
        if (!_zapisi.ContainsKey(zapis.Id))
        {
            return Result.Failure("repo.nije_pronadjen", "Zapis sa tim identifikatorom ne postoji.");
        }

        _zapisi[zapis.Id] = zapis;
        return Result.Success();
    }

    public Result<Zapis> Pronadji(Guid id)
    {
        if (_zapisi.TryGetValue(id, out Zapis? zapis))
        {
            return Result<Zapis>.Success(zapis);
        }

        return Result<Zapis>.Failure("repo.nije_pronadjen", "Zapis sa tim identifikatorom ne postoji.");
    }

    public Result<IReadOnlyList<Zapis>> Svi() =>
        Result<IReadOnlyList<Zapis>>.Success(_zapisi.Values.OrderBy(z => z.KreiranoUtc).ToList());
}
