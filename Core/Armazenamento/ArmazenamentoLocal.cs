using Entidades.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;

namespace Core.Armazenamento;

/// <summary>
/// Guarda as imagens enviadas numa pasta local (padrão quando o Cloudinary não está configurado).
/// Os arquivos são servidos pela API em /uploads.
/// </summary>
public class ArmazenamentoLocal : IArmazenamentoImagens
{
    private readonly string _pasta;
    private readonly IHttpContextAccessor _http;

    public ArmazenamentoLocal(string pasta, IHttpContextAccessor http)
    {
        _pasta = pasta;
        _http = http;
        Directory.CreateDirectory(_pasta);
    }

    public async Task<string?> Salvar(IFormFile imagem)
    {
        if (!ValidadorImagem.EhValida(imagem)) return null;

        var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();
        var nome = $"{Guid.NewGuid():N}{extensao}";

        await using (var stream = File.Create(Path.Combine(_pasta, nome)))
        {
            await imagem.CopyToAsync(stream);
        }

        var request = _http.HttpContext?.Request;
        var baseUrl = request is null ? string.Empty : $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/uploads/{nome}";
    }
}
