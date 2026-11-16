using Entidades.Usuarios;

namespace Entidades.Interfaces.Usuarios;

public interface IUsuarioRepository
{
    int Adicionar(Usuario usuario);
    Usuario? BuscarPorUsername(string username);
    List<Usuario> Listar();
    Usuario? BuscarUsuarioPorId(int id);
    bool Editar(Usuario usuario);
    bool EditarUsername(int id, string username);
    bool EditarSenha(int id, string senhaHash);
    bool EditarFoto(int id, string url);
    bool Remover(int id);
}
