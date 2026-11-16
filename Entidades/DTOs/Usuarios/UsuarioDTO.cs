using Entidades.Usuarios;

namespace Entidades.DTOs.Usuarios;

/// <summary>Usuário como exposto pela API: nunca inclui a senha.</summary>
public class UsuarioDTO
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
    public string Identificacao { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string FotoDePerfilURL { get; set; } = string.Empty;

    public static UsuarioDTO De(Usuario u) => new()
    {
        Id = u.Id, Nome = u.Nome, Idade = u.Idade, Genero = u.Genero, CEP = u.CEP,
        Rua = u.Rua, Numero = u.Numero, Bairro = u.Bairro, Cidade = u.Cidade,
        UnidadeFederativa = u.UnidadeFederativa, Endereco = u.Endereco,
        Identificacao = u.Identificacao, Username = u.Username, Tipo = u.Tipo,
        FotoDePerfilURL = u.FotoDePerfilURL
    };
}
