using Core.Dados;
using Dapper;
using Entidades.Interfaces.Usuarios;
using Entidades.Usuarios;

namespace Core.Repositorios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ConexaoSqlite _conexao;

    public UsuarioRepository(ConexaoSqlite conexao)
    {
        _conexao = conexao;
    }

    public int Adicionar(Usuario usuario)
    {
        using var connection = _conexao.Criar();

        return connection.ExecuteScalar<int>(@"
            INSERT INTO Usuarios
                (Nome, Idade, Genero, CEP, Rua, Numero, Bairro, Cidade, UnidadeFederativa,
                 Endereco, Identificacao, Username, Password, Tipo, FotoDePerfilURL)
            VALUES
                (@Nome, @Idade, @Genero, @CEP, @Rua, @Numero, @Bairro, @Cidade, @UnidadeFederativa,
                 @Endereco, @Identificacao, @Username, @Password, @Tipo, @FotoDePerfilURL);
            SELECT last_insert_rowid();", usuario);
    }

    public Usuario? BuscarPorUsername(string username)
    {
        using var connection = _conexao.Criar();

        return connection.QueryFirstOrDefault<Usuario>(
            "SELECT * FROM Usuarios WHERE Username = @username;", new { username });
    }

    public List<Usuario> Listar()
    {
        using var connection = _conexao.Criar();

        return connection.Query<Usuario>("SELECT * FROM Usuarios;").ToList();
    }

    public Usuario? BuscarUsuarioPorId(int id)
    {
        using var connection = _conexao.Criar();

        return connection.QueryFirstOrDefault<Usuario>(
            "SELECT * FROM Usuarios WHERE Id = @id;", new { id });
    }

    /// <summary>Atualiza os dados cadastrais. Não altera senha nem tipo.</summary>
    public bool Editar(Usuario usuario)
    {
        using var connection = _conexao.Criar();

        return connection.Execute(@"
            UPDATE Usuarios SET
                Nome = @Nome, Idade = @Idade, Genero = @Genero, CEP = @CEP, Rua = @Rua,
                Numero = @Numero, Bairro = @Bairro, Cidade = @Cidade,
                UnidadeFederativa = @UnidadeFederativa, Endereco = @Endereco,
                Identificacao = @Identificacao
            WHERE Id = @Id;", usuario) > 0;
    }

    public bool EditarUsername(int id, string username)
    {
        using var connection = _conexao.Criar();

        return connection.Execute(
            "UPDATE Usuarios SET Username = @username WHERE Id = @id;", new { id, username }) > 0;
    }

    public bool EditarSenha(int id, string senhaHash)
    {
        using var connection = _conexao.Criar();

        return connection.Execute(
            "UPDATE Usuarios SET Password = @senhaHash WHERE Id = @id;", new { id, senhaHash }) > 0;
    }

    public bool EditarFoto(int id, string url)
    {
        using var connection = _conexao.Criar();

        return connection.Execute(
            "UPDATE Usuarios SET FotoDePerfilURL = @url WHERE Id = @id;", new { id, url }) > 0;
    }

    public bool Remover(int id)
    {
        using var connection = _conexao.Criar();

        return connection.Execute("DELETE FROM Usuarios WHERE Id = @id;", new { id }) > 0;
    }
}
