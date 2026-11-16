using System.Text.Json.Serialization;

namespace Entidades.Imoveis;

/// <summary>
/// Imóvel anunciado. Uma única entidade cobre todos os tipos
/// (Apartamento, Casa, Comercial, Lote, Rural e Terreno): os campos que não
/// se aplicam a um tipo ficam nulos.
/// </summary>
public class Imovel
{
    public string? Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public double Area { get; set; }
    public double Preco { get; set; }
    public int Anos { get; set; }
    public string TipoServico { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public int? Numero { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UnidadeFederativa { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    [JsonPropertyName("urlsImagens")]
    public List<string> URLsImagens { get; set; } = new();
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public List<string> AreasComuns { get; set; } = new();

    // Campos específicos de alguns tipos
    public int? Quartos { get; set; }
    public int? Suites { get; set; }
    public int? Banheiros { get; set; }
    public int? Vagas { get; set; }
    public int? Andar { get; set; }
    public string? TipoNegocio { get; set; }
}
