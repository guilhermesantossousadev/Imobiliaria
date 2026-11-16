using Entidades.DTOs.Imoveis;
using Entidades.Imoveis;
using Microsoft.AspNetCore.Http;

namespace Entidades.Interfaces.Imoveis;

public interface IImovelService
{
    Task<ReturnImovelIdDTO> Adicionar(Imovel imovel);
    Task<string?> UploadImage(IFormFile imagem, string imovelId);
    Task<List<Imovel>> Listar();
    Task<Imovel?> BuscarImovelPorId(string id);
    Task<List<Imovel>> BuscarImoveisPorTipo(string tipo);
    Task<List<Imovel>> BuscarImoveisPorTipoServico(string tipoServico);
    Task<List<string>?> BuscarURLsImagensPorId(string id);
    Task<bool> Editar(Imovel imovel);
    Task<bool> Remover(string id);
    ReturnPrecificadorImovelDTO PrecificarImovel(PrecificadorImovelDTO imovel);
}
