using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Dados;

public class InicializadorDb
{
    public const string FotoPadraoUsuario = "/exemplos/usuario.svg";
    public const string FotoPadraoAdmin = "/exemplos/admin.svg";

    private readonly ConexaoSqlite _conexao;
    private readonly IConfiguration _config;
    private readonly ILogger<InicializadorDb> _logger;

    public InicializadorDb(ConexaoSqlite conexao, IConfiguration config, ILogger<InicializadorDb> logger)
    {
        _conexao = conexao;
        _config = config;
        _logger = logger;
    }

    public void Inicializar()
    {
        using var connection = _conexao.Criar();

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS Usuarios(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nome TEXT NOT NULL,
                Idade INTEGER NOT NULL DEFAULT 0,
                Genero TEXT NOT NULL DEFAULT '',
                CEP TEXT,
                Rua TEXT,
                Numero INTEGER NOT NULL DEFAULT 0,
                Bairro TEXT,
                Cidade TEXT,
                UnidadeFederativa TEXT,
                Endereco TEXT,
                Identificacao TEXT NOT NULL DEFAULT '',
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Tipo TEXT NOT NULL,
                FotoDePerfilURL TEXT
            );

            CREATE TABLE IF NOT EXISTS Imoveis(
                Id TEXT PRIMARY KEY,
                Tipo TEXT NOT NULL,
                Area REAL NOT NULL DEFAULT 0,
                Preco REAL NOT NULL DEFAULT 0,
                Anos INTEGER NOT NULL DEFAULT 0,
                TipoServico TEXT NOT NULL,
                CEP TEXT,
                Rua TEXT,
                Numero INTEGER,
                Bairro TEXT,
                Cidade TEXT,
                UnidadeFederativa TEXT,
                Endereco TEXT,
                URLsImagens TEXT NOT NULL DEFAULT '[]',
                Titulo TEXT,
                Descricao TEXT,
                AreasComuns TEXT NOT NULL DEFAULT '[]',
                Quartos INTEGER,
                Suites INTEGER,
                Banheiros INTEGER,
                Vagas INTEGER,
                Andar INTEGER,
                TipoNegocio TEXT
            );

            CREATE TABLE IF NOT EXISTS Mensagens(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ImovelId TEXT,
                Nome TEXT NOT NULL DEFAULT '',
                Email TEXT NOT NULL,
                Telefone TEXT NOT NULL DEFAULT '',
                Texto TEXT NOT NULL DEFAULT '',
                Origem TEXT NOT NULL,
                CriadaEm TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Imoveis_Tipo ON Imoveis(Tipo);
            CREATE INDEX IF NOT EXISTS IX_Imoveis_TipoServico ON Imoveis(TipoServico);");

        CriarAdminSeNecessario(connection);
    }

    private void CriarAdminSeNecessario(Microsoft.Data.Sqlite.SqliteConnection connection)
    {
        var existe = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM Usuarios;");
        if (existe > 0) return;

        var senha = _config["Admin:Senha"];
        if (string.IsNullOrWhiteSpace(senha)) senha = "Admin";

        connection.Execute(@"
            INSERT INTO Usuarios
                (Nome, Idade, Genero, CEP, Rua, Numero, Bairro, Cidade, UnidadeFederativa,
                 Endereco, Identificacao, Username, Password, Tipo, FotoDePerfilURL)
            VALUES
                ('Administrador', 0, 'Masculino', '34505-730', 'Praça Getúlio Vargas', 168,
                 'Centro', 'Sabará', 'MG',
                 'Praça Getúlio Vargas, 168, Centro, Sabará, MG - 34505-730',
                 '00.000.000/0001-00', 'Admin', @Hash, 'Administrador', @Foto);",
            new { Hash = BCrypt.Net.BCrypt.HashPassword(senha), Foto = FotoPadraoAdmin });

        _logger.LogInformation("Usuário administrador criado (username: Admin).");
    }
}
