namespace Entidades.Usuarios;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UnidadeFederativa { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Identificacao { get; set; } = string.Empty; // CPF, CNPJ, CNH, CTPS, RG, etc...
    public string Username { get; set; } = string.Empty;

    /// <summary>Hash BCrypt da senha. Nunca é devolvido pela API.</summary>
    public string Password { get; set; } = string.Empty;

    public string Tipo { get; set; } = "Usuario"; // Administrador ou Usuario
    public string FotoDePerfilURL { get; set; } = string.Empty;
}
