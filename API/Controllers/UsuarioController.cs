using Entidades.DTOs.Usuarios;
using Entidades.Interfaces.Usuarios;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuarioController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpPost("AdicionarUsuario")]
    public IActionResult Adicionar(CreateUsuarioDTO usuarioDTO)
    {
        var resultado = _service.Adicionar(usuarioDTO);

        return resultado is null
            ? Conflict(new { mensagem = "Username inválido ou já em uso." })
            : Ok(resultado);
    }

    [HttpPut("UploadImage")]
    public async Task<IActionResult> UploadImage(IFormFile imagem, [FromQuery] int usuarioId)
    {
        var url = await _service.UploadImage(imagem, usuarioId);

        return url is null
            ? BadRequest(new { mensagem = "Usuário não encontrado ou imagem inválida (jpg, png, webp, gif até 5 MB)." })
            : Ok(url);
    }

    [HttpPost("LogarUsuario")]
    public IActionResult LogarUsuario(LoginUsuarioDTO usuarioLogin)
    {
        var usuario = _service.LogarUsuario(usuarioLogin);

        return usuario is null
            ? Unauthorized(new { mensagem = "Usuário ou senha inválidos." })
            : Ok(usuario);
    }

    [HttpGet("ListarUsuarios")]
    public List<UsuarioDTO> Listar() => _service.Listar();

    [HttpGet("ListarUsuarioPorId")]
    public IActionResult BuscarUsuarioPorId(int id)
    {
        var usuario = _service.BuscarUsuarioPorId(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPut("EditarUsuario")]
    public IActionResult Editar(UsuarioDTO usuarioEdit) =>
        _service.Editar(usuarioEdit) ? NoContent() : NotFound();

    [HttpPut("EditarUsername")]
    public IActionResult EditarUsername(UpdateUsuarioDTO usuarioEdit) =>
        _service.EditarUsername(usuarioEdit.Id, usuarioEdit.Valor)
            ? NoContent()
            : BadRequest(new { mensagem = "Username inválido, já em uso ou usuário não encontrado." });

    [HttpPut("EditarSenha")]
    public IActionResult EditarSenha(UpdateUsuarioDTO usuarioEdit) =>
        _service.EditarSenha(usuarioEdit.Id, usuarioEdit.Valor)
            ? NoContent()
            : BadRequest(new { mensagem = "Senha inválida ou usuário não encontrado." });

    [HttpDelete("RemoverUsuario")]
    public IActionResult Remover(int id) => _service.Remover(id) ? NoContent() : NotFound();
}
