namespace Entidades.DTOs.Imoveis;

public class PrecificadorImovelDTO
{
    public string Endereco { get; set; } = string.Empty;
    public string VendaOuAluguel { get; set; } = string.Empty;
    public bool Novo { get; set; }
    public string TipoImovel { get; set; } = string.Empty;
    public int Quartos { get; set; }
    public int Banheiros { get; set; }
    public int Vagas { get; set; }
    public double AreaM2 { get; set; }
    public List<string> AreasComuns { get; set; } = new();
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}
