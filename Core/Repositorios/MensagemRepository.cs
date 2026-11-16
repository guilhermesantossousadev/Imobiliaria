using Core.Dados;
using Dapper;
using Entidades.Interfaces.Mensagens;
using Entidades.Mensagens;

namespace Core.Repositorios;

public class MensagemRepository : IMensagemRepository
{
    private readonly ConexaoSqlite _conexao;

    public MensagemRepository(ConexaoSqlite conexao)
    {
        _conexao = conexao;
    }

    public int Adicionar(Mensagem mensagem)
    {
        mensagem.CriadaEm = DateTime.UtcNow.ToString("o");

        using var connection = _conexao.Criar();
        return connection.ExecuteScalar<int>(@"
            INSERT INTO Mensagens (ImovelId, Nome, Email, Telefone, Texto, Origem, CriadaEm)
            VALUES (@ImovelId, @Nome, @Email, @Telefone, @Texto, @Origem, @CriadaEm);
            SELECT last_insert_rowid();", mensagem);
    }

    public List<Mensagem> Listar()
    {
        using var connection = _conexao.Criar();
        return connection.Query<Mensagem>("SELECT * FROM Mensagens ORDER BY Id DESC;").ToList();
    }
}
