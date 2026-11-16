namespace Entidades.Mensagens;

/// <summary>Mensagem enviada pelo formulário de contato (geral ou sobre um imóvel) ou pela newsletter.</summary>
public class Mensagem
{
    public int Id { get; set; }
    public string? ImovelId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public string Origem { get; set; } = "contato"; // contato | imovel | newsletter
    public string CriadaEm { get; set; } = string.Empty;
}
