using System.ComponentModel.DataAnnotations;
using Entidades.Interfaces.Mensagens;
using Entidades.Mensagens;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class EnviarMensagemDTO
{
    public string? ImovelId { get; set; }
    public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string Origem { get; set; } = "contato";
}

[ApiController]
[Route("[controller]")]
public class MensagemController : ControllerBase
{
    private static readonly HashSet<string> Origens = new() { "contato", "imovel", "newsletter" };

    private readonly IMensagemRepository _repository;

    public MensagemController(IMensagemRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("EnviarMensagem")]
    public IActionResult Enviar(EnviarMensagemDTO dto)
    {
        var origem = Origens.Contains(dto.Origem) ? dto.Origem : "contato";

        var id = _repository.Adicionar(new Mensagem
        {
            ImovelId = dto.ImovelId,
            Nome = dto.Nome,
            Email = dto.Email,
            Telefone = dto.Telefone,
            Texto = dto.Texto,
            Origem = origem
        });

        return Ok(new { id });
    }

    [HttpGet("ListarMensagens")]
    public List<Mensagem> Listar() => _repository.Listar();
}
