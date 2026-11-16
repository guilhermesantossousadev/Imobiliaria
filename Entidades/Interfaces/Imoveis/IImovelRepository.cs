using Entidades.Imoveis;

namespace Entidades.Interfaces.Imoveis;

public interface IImovelRepository
{
    Task<string> Adicionar(Imovel imovel);
    Task<List<Imovel>> Listar();
    Task<Imovel?> BuscarImovelPorId(string id);
    Task<List<Imovel>> BuscarImoveisPorTipo(string tipo);
    Task<List<Imovel>> BuscarImoveisPorTipoServico(string tipoServico);
    Task<bool> Editar(Imovel imovel);
    Task<bool> Remover(string id);
}
