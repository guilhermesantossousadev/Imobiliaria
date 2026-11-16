using Microsoft.AspNetCore.Http;

namespace Core.Armazenamento;

public static class ValidadorImagem
{
    public const long TamanhoMaximoBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> Extensoes = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    public static bool EhValida(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0 || arquivo.Length > TamanhoMaximoBytes) return false;
        return Extensoes.Contains(Path.GetExtension(arquivo.FileName));
    }
}
