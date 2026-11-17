using Domain.Models;

namespace Application.Services;

public sealed class PregledRezultat
{
    public PregledRezultat(IReadOnlyList<Zapis> zapisi, int preostaloMinuta)
    {
        Zapisi = zapisi;
        PreostaloMinuta = preostaloMinuta;
    }

    public IReadOnlyList<Zapis> Zapisi { get; }
    public int PreostaloMinuta { get; }
}
