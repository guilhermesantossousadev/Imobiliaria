using Microsoft.AspNetCore.Http;

namespace Entidades.Interfaces.Servicos;

public interface IArmazenamentoImagens
{
    /// <summary>Salva a imagem e devolve a URL pública. Retorna null se o arquivo for inválido.</summary>
    Task<string?> Salvar(IFormFile imagem);
}
