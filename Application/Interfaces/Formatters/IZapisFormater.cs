using Domain.Models;

namespace Application.Interfaces.Formatters;

public interface IZapisFormater
{
    string Naziv { get; }
    string Formatiraj(Zapis zapis);
}
