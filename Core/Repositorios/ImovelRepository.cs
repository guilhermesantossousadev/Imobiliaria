using System.Text.Json;
using Core.Dados;
using Dapper;
using Entidades.Imoveis;
using Entidades.Interfaces.Imoveis;
using Microsoft.Data.Sqlite;

namespace Core.Repositorios;

public class ImovelRepository : IImovelRepository
{
    private const string Colunas = @"Id, Tipo, Area, Preco, Anos, TipoServico, CEP, Rua, Numero, Bairro,
        Cidade, UnidadeFederativa, Endereco, URLsImagens, Titulo, Descricao, AreasComuns,
        Quartos, Suites, Banheiros, Vagas, Andar, TipoNegocio";

    private readonly ConexaoSqlite _conexao;

    public ImovelRepository(ConexaoSqlite conexao)
    {
        _conexao = conexao;
    }

    public async Task<string> Adicionar(Imovel imovel)
    {
        imovel.Id = Guid.NewGuid().ToString("N");

        using var connection = _conexao.Criar();
        await connection.ExecuteAsync($@"
            INSERT INTO Imoveis ({Colunas})
            VALUES (@Id, @Tipo, @Area, @Preco, @Anos, @TipoServico, @CEP, @Rua, @Numero, @Bairro,
                    @Cidade, @UnidadeFederativa, @Endereco, @URLsImagens, @Titulo, @Descricao,
                    @AreasComuns, @Quartos, @Suites, @Banheiros, @Vagas, @Andar, @TipoNegocio);",
            ParaLinha(imovel));

        return imovel.Id;
    }

    public async Task<List<Imovel>> Listar()
    {
        using var connection = _conexao.Criar();
        var linhas = await connection.QueryAsync<Linha>($"SELECT {Colunas} FROM Imoveis;");
        return linhas.Select(DeLinha).ToList();
    }

    public async Task<Imovel?> BuscarImovelPorId(string id)
    {
        using var connection = _conexao.Criar();
        var linha = await connection.QueryFirstOrDefaultAsync<Linha>(
            $"SELECT {Colunas} FROM Imoveis WHERE Id = @id;", new { id });
        return linha is null ? null : DeLinha(linha);
    }

    public async Task<List<Imovel>> BuscarImoveisPorTipo(string tipo)
    {
        using var connection = _conexao.Criar();
        var linhas = await connection.QueryAsync<Linha>(
            $"SELECT {Colunas} FROM Imoveis WHERE Tipo = @tipo COLLATE NOCASE;", new { tipo });
        return linhas.Select(DeLinha).ToList();
    }

    public async Task<List<Imovel>> BuscarImoveisPorTipoServico(string tipoServico)
    {
        using var connection = _conexao.Criar();
        var linhas = await connection.QueryAsync<Linha>(
            $"SELECT {Colunas} FROM Imoveis WHERE TipoServico = @tipoServico COLLATE NOCASE;",
            new { tipoServico });
        return linhas.Select(DeLinha).ToList();
    }

    public async Task<bool> Editar(Imovel imovel)
    {
        using var connection = _conexao.Criar();
        var afetadas = await connection.ExecuteAsync(@"
            UPDATE Imoveis SET
                Tipo = @Tipo, Area = @Area, Preco = @Preco, Anos = @Anos, TipoServico = @TipoServico,
                CEP = @CEP, Rua = @Rua, Numero = @Numero, Bairro = @Bairro, Cidade = @Cidade,
                UnidadeFederativa = @UnidadeFederativa, Endereco = @Endereco,
                URLsImagens = @URLsImagens, Titulo = @Titulo, Descricao = @Descricao,
                AreasComuns = @AreasComuns, Quartos = @Quartos, Suites = @Suites,
                Banheiros = @Banheiros, Vagas = @Vagas, Andar = @Andar, TipoNegocio = @TipoNegocio
            WHERE Id = @Id;", ParaLinha(imovel));
        return afetadas > 0;
    }

    public async Task<bool> Remover(string id)
    {
        using var connection = _conexao.Criar();
        return await connection.ExecuteAsync("DELETE FROM Imoveis WHERE Id = @id;", new { id }) > 0;
    }

    // --- Mapeamento: listas são guardadas como JSON em colunas TEXT ---

    private class Linha
    {
        public string Id { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Area { get; set; }
        public double Preco { get; set; }
        public int Anos { get; set; }
        public string TipoServico { get; set; } = string.Empty;
        public string? CEP { get; set; }
        public string? Rua { get; set; }
        public int? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? UnidadeFederativa { get; set; }
        public string? Endereco { get; set; }
        public string URLsImagens { get; set; } = "[]";
        public string? Titulo { get; set; }
        public string? Descricao { get; set; }
        public string AreasComuns { get; set; } = "[]";
        public int? Quartos { get; set; }
        public int? Suites { get; set; }
        public int? Banheiros { get; set; }
        public int? Vagas { get; set; }
        public int? Andar { get; set; }
        public string? TipoNegocio { get; set; }
    }

    private static Linha ParaLinha(Imovel i) => new()
    {
        Id = i.Id ?? string.Empty, Tipo = i.Tipo, Area = i.Area, Preco = i.Preco, Anos = i.Anos,
        TipoServico = i.TipoServico, CEP = i.CEP, Rua = i.Rua, Numero = i.Numero, Bairro = i.Bairro,
        Cidade = i.Cidade, UnidadeFederativa = i.UnidadeFederativa, Endereco = i.Endereco,
        URLsImagens = JsonSerializer.Serialize(i.URLsImagens ?? new()), Titulo = i.Titulo,
        Descricao = i.Descricao, AreasComuns = JsonSerializer.Serialize(i.AreasComuns ?? new()),
        Quartos = i.Quartos, Suites = i.Suites, Banheiros = i.Banheiros, Vagas = i.Vagas,
        Andar = i.Andar, TipoNegocio = i.TipoNegocio
    };

    private static Imovel DeLinha(Linha l) => new()
    {
        Id = l.Id, Tipo = l.Tipo, Area = l.Area, Preco = l.Preco, Anos = l.Anos,
        TipoServico = l.TipoServico, CEP = l.CEP ?? string.Empty, Rua = l.Rua ?? string.Empty,
        Numero = l.Numero, Bairro = l.Bairro ?? string.Empty, Cidade = l.Cidade ?? string.Empty,
        UnidadeFederativa = l.UnidadeFederativa ?? string.Empty, Endereco = l.Endereco ?? string.Empty,
        URLsImagens = JsonSerializer.Deserialize<List<string>>(l.URLsImagens) ?? new(),
        Titulo = l.Titulo ?? string.Empty, Descricao = l.Descricao ?? string.Empty,
        AreasComuns = JsonSerializer.Deserialize<List<string>>(l.AreasComuns) ?? new(),
        Quartos = l.Quartos, Suites = l.Suites, Banheiros = l.Banheiros, Vagas = l.Vagas,
        Andar = l.Andar, TipoNegocio = l.TipoNegocio
    };
}
