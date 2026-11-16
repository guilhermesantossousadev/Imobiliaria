using Entidades.DTOs.Imoveis;
using Entidades.Imoveis;
using Entidades.Interfaces.Imoveis;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ImovelController : ControllerBase
{
    private readonly IImovelService _service;

    public ImovelController(IImovelService service)
    {
        _service = service;
    }

    [HttpPost("AdicionarImovel")]
    public async Task<ReturnImovelIdDTO> Adicionar(Imovel imovel) => await _service.Adicionar(imovel);

    [HttpPut("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile imagem, [FromQuery] string imovelId)
    {
        var url = await _service.UploadImage(imagem, imovelId);

        return url is null
            ? BadRequest(new { mensagem = "Imóvel não encontrado ou imagem inválida (jpg, png, webp, gif até 5 MB)." })
            : Ok(url);
    }

    [HttpGet("ListarImoveis")]
    public async Task<List<Imovel>> Listar() => await _service.Listar();

    [HttpGet("ListarImovelPorId")]
    public async Task<IActionResult> BuscarImovelPorId(string id)
    {
        var imovel = await _service.BuscarImovelPorId(id);
        return imovel is null ? NotFound() : Ok(imovel);
    }

    [HttpGet("ListarImoveisPorTipo")]
    public async Task<List<Imovel>> BuscarImoveisPorTipo(string tipo) => await _service.BuscarImoveisPorTipo(tipo);

    [HttpGet("ListarImoveisPorTipoServico")]
    public async Task<List<Imovel>> BuscarImoveisPorTipoServico(string tipoServico) =>
        await _service.BuscarImoveisPorTipoServico(tipoServico);

    [HttpGet("ListarURLsImagensPorId")]
    public async Task<IActionResult> BuscarURLsImagensPorId(string id)
    {
        var urls = await _service.BuscarURLsImagensPorId(id);
        return urls is null ? NotFound() : Ok(urls);
    }

    [HttpPut("EditarImovel")]
    public async Task<IActionResult> Editar(Imovel imovelEdit) =>
        await _service.Editar(imovelEdit) ? NoContent() : NotFound();

    [HttpDelete("RemoverImovel")]
    public async Task<IActionResult> Remover(string id) =>
        await _service.Remover(id) ? NoContent() : NotFound();

    [HttpPost("PrecificarImovel")]
    public ReturnPrecificadorImovelDTO PrecificarImovel(PrecificadorImovelDTO imovel) =>
        _service.PrecificarImovel(imovel);
}
