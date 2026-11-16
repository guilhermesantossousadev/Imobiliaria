using Entidades.DTOs.Usuarios;
using Microsoft.AspNetCore.Http;

namespace Entidades.Interfaces.Usuarios;

public interface IUsuarioService
{
    /// <summary>Cria o usuário. Retorna null se o username já existir.</summary>
    ReturnUsuarioIdDTO? Adicionar(CreateUsuarioDTO dto);
    Task<string?> UploadImage(IFormFile imagem, int usuarioId);
    UsuarioDTO? LogarUsuario(LoginUsuarioDTO login);
    List<UsuarioDTO> Listar();
    UsuarioDTO? BuscarUsuarioPorId(int id);
    bool Editar(UsuarioDTO usuario);
    bool EditarUsername(int id, string username);
    bool EditarSenha(int id, string senha);
    bool Remover(int id);
}
