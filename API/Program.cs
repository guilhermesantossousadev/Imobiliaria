using Core.Armazenamento;
using Core.Dados;
using Core.Repositorios;
using Core.Services;
using Entidades.Interfaces.Imoveis;
using Entidades.Interfaces.Mensagens;
using Entidades.Interfaces.Servicos;
using Entidades.Interfaces.Usuarios;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// --- Banco SQLite (arquivo em API/App_Data por padrão) ---
var pastaDados = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(pastaDados);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    connectionString = $"Data Source={Path.Combine(pastaDados, "imobiliaria.db")}";

builder.Services.AddSingleton(new ConexaoSqlite(connectionString));
builder.Services.AddSingleton<InicializadorDb>();

// --- Armazenamento de imagens: pasta local ---
var pastaUploads = Path.Combine(pastaDados, "uploads");
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IArmazenamentoImagens>(sp =>
    new ArmazenamentoLocal(pastaUploads, sp.GetRequiredService<IHttpContextAccessor>()));

// --- Aplicação ---
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IImovelRepository, ImovelRepository>();
builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IImovelService, ImovelService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var origens = builder.Configuration.GetSection("Cors:Origens").Get<string[]>()
              ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("Front", policy =>
        policy.WithOrigins(origens).AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// --- Inicialização do banco e dados de exemplo ---
app.Services.GetRequiredService<InicializadorDb>().Inicializar();

if (app.Configuration.GetValue("Seed:ImoveisExemplo", true))
{
    using var scope = app.Services.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<IImovelRepository>();

    if ((await repo.Listar()).Count == 0)
    {
        foreach (var imovel in ImoveisExemplo.Obter())
            await repo.Adicionar(imovel);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

Directory.CreateDirectory(pastaUploads);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pastaUploads),
    RequestPath = "/uploads"
});

app.UseCors("Front");
app.UseAuthorization();
app.MapControllers();

app.Run();
