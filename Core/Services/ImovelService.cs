using Entidades.DTOs.Imoveis;
using Entidades.Imoveis;
using Entidades.Interfaces.Imoveis;
using Entidades.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;

namespace Core.Services;

public class ImovelService : IImovelService
{
    private readonly IImovelRepository _repository;
    private readonly IArmazenamentoImagens _armazenamento;

    public ImovelService(IImovelRepository repository, IArmazenamentoImagens armazenamento)
    {
        _repository = repository;
        _armazenamento = armazenamento;
    }

    public async Task<ReturnImovelIdDTO> Adicionar(Imovel imovel)
    {
        imovel.URLsImagens ??= new List<string>();
        imovel.AreasComuns ??= new List<string>();

        var id = await _repository.Adicionar(imovel);
        return new ReturnImovelIdDTO { Id = id };
    }

    public async Task<string?> UploadImage(IFormFile imagem, string imovelId)
    {
        var imovel = await _repository.BuscarImovelPorId(imovelId);
        if (imovel is null) return null;

        var url = await _armazenamento.Salvar(imagem);
        if (url is null) return null;

        imovel.URLsImagens.Add(url);
        await _repository.Editar(imovel);

        return url;
    }

    public Task<List<Imovel>> Listar() => _repository.Listar();

    public Task<Imovel?> BuscarImovelPorId(string id) => _repository.BuscarImovelPorId(id);

    public Task<List<Imovel>> BuscarImoveisPorTipo(string tipo) => _repository.BuscarImoveisPorTipo(tipo);

    public Task<List<Imovel>> BuscarImoveisPorTipoServico(string tipoServico) =>
        _repository.BuscarImoveisPorTipoServico(tipoServico);

    public async Task<List<string>?> BuscarURLsImagensPorId(string id) =>
        (await _repository.BuscarImovelPorId(id))?.URLsImagens;

    public Task<bool> Editar(Imovel imovel) => _repository.Editar(imovel);

    public Task<bool> Remover(string id) => _repository.Remover(id);

    public ReturnPrecificadorImovelDTO PrecificarImovel(PrecificadorImovelDTO imovel)
    {
        const double valorBase = 80000;

        var ehVenda = string.Equals(imovel.VendaOuAluguel, "venda", StringComparison.OrdinalIgnoreCase);
        var ehApartamento = string.Equals(imovel.TipoImovel, "apartamento", StringComparison.OrdinalIgnoreCase);

        double coefVenda = ehVenda ? 1.0 : 0.79;
        double coefNovo = imovel.Novo ? 1.2 : 1.0;
        double coefTipo = ehApartamento ? 1.0 : 1.2;
        double coefQuartos = 1.04101;
        double coefBanheiros = 1.0209;
        double coefVagas = 1.01099;
        double coefArea = 1.1;
        double coefAreasComuns = 1.0108;

        var preco = valorBase *
                    coefVenda *
                    coefNovo *
                    coefTipo *
                    Math.Pow(coefQuartos, imovel.Quartos) *
                    Math.Pow(coefBanheiros, imovel.Banheiros) *
                    Math.Pow(coefVagas, imovel.Vagas) *
                    Math.Pow(coefArea, imovel.AreaM2 / 50) *
                    Math.Pow(coefAreasComuns, imovel.AreasComuns?.Count ?? 0);

        return new ReturnPrecificadorImovelDTO { Preco = Math.Round(preco, 2) };
    }
}
