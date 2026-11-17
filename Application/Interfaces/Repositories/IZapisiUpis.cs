using Domain.Models;
using Shared;

namespace Application.Interfaces.Repositories;

public interface IZapisiUpis
{
    Result Sacuvaj(Zapis zapis);
    Result Azuriraj(Zapis zapis);
}
