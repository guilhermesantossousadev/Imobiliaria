using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Entidades.Interfaces.Servicos;
using Microsoft.AspNetCore.Http;

namespace Core.Armazenamento;

public class ArmazenamentoCloudinary : IArmazenamentoImagens
{
    private readonly Cloudinary _cloudinary;

    public ArmazenamentoCloudinary(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string?> Salvar(IFormFile imagem)
    {
        if (!ValidadorImagem.EhValida(imagem)) return null;

        await using var stream = imagem.OpenReadStream();
        var resultado = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(imagem.FileName, stream)
        });

        return resultado.StatusCode == System.Net.HttpStatusCode.OK
            ? resultado.SecureUrl.ToString()
            : null;
    }
}
