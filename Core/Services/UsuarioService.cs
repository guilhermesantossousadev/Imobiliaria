using Entidades.DTOs.Usuarios;
using Entidades.Interfaces.Servicos;
using Entidades.Interfaces.Usuarios;
using Entidades.Usuarios;
using Microsoft.AspNetCore.Http;
using Core.Dados;

namespace Core.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IArmazenamentoImagens _armazenamento;

    public UsuarioService(IUsuarioRepository repository, IArmazenamentoImagens armazenamento)
    {
        _repository = repository;
        _armazenamento = armazenamento;
    }

    public ReturnUsuarioIdDTO? Adicionar(CreateUsuarioDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return null;

        if (_repository.BuscarPorUsername(dto.Username) is not null)
            return null;

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Idade = dto.Idade,
            Genero = dto.Genero,
            CEP = dto.CEP,
            Rua = dto.Rua,
            Numero = dto.Numero,
            Bairro = dto.Bairro,
            Cidade = dto.Cidade,
            UnidadeFederativa = dto.UnidadeFederativa,
            Endereco = $"{dto.Rua}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, {dto.UnidadeFederativa} - {dto.CEP}",
            Identificacao = dto.Identificacao,
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Tipo = "Usuario",
            FotoDePerfilURL = InicializadorDb.FotoPadraoUsuario
        };

        return new ReturnUsuarioIdDTO { Id = _repository.Adicionar(usuario) };
    }

    public async Task<string?> UploadImage(IFormFile imagem, int usuarioId)
    {
        if (_repository.BuscarUsuarioPorId(usuarioId) is null) return null;

        var url = await _armazenamento.Salvar(imagem);
        if (url is null) return null;

        _repository.EditarFoto(usuarioId, url);
        return url;
    }

    public UsuarioDTO? LogarUsuario(LoginUsuarioDTO login)
    {
        var usuario = _repository.BuscarPorUsername(login.Username);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(login.Password, usuario.Password))
            return null;

        return UsuarioDTO.De(usuario);
    }

    public List<UsuarioDTO> Listar() => _repository.Listar().Select(UsuarioDTO.De).ToList();

    public UsuarioDTO? BuscarUsuarioPorId(int id)
    {
        var usuario = _repository.BuscarUsuarioPorId(id);
        return usuario is null ? null : UsuarioDTO.De(usuario);
    }

    public bool Editar(UsuarioDTO dto)
    {
        var usuario = _repository.BuscarUsuarioPorId(dto.Id);
        if (usuario is null) return false;

        usuario.Nome = dto.Nome;
        usuario.Idade = dto.Idade;
        usuario.Genero = dto.Genero;
        usuario.CEP = dto.CEP;
        usuario.Rua = dto.Rua;
        usuario.Numero = dto.Numero;
        usuario.Bairro = dto.Bairro;
        usuario.Cidade = dto.Cidade;
        usuario.UnidadeFederativa = dto.UnidadeFederativa;
        usuario.Endereco = $"{dto.Rua}, {dto.Numero}, {dto.Bairro}, {dto.Cidade}, {dto.UnidadeFederativa} - {dto.CEP}";
        usuario.Identificacao = dto.Identificacao;

        return _repository.Editar(usuario);
    }

    public bool EditarUsername(int id, string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return false;

        var existente = _repository.BuscarPorUsername(username);
        if (existente is not null && existente.Id != id) return false;

        return _repository.EditarUsername(id, username);
    }

    public bool EditarSenha(int id, string senha)
    {
        if (string.IsNullOrWhiteSpace(senha)) return false;

        return _repository.EditarSenha(id, BCrypt.Net.BCrypt.HashPassword(senha));
    }

    public bool Remover(int id) => _repository.Remover(id);
}
