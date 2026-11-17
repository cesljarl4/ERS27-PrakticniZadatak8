using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

public interface IZapisiCitanje
{
    Result<Zapis> Pronadji(Guid id);
    Result<IReadOnlyList<Zapis>> Svi();
}
