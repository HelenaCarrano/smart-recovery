using System.Reflection;
using Microsoft.OpenApi.Models;
using SmartRecovery.API.Extensions;
using SmartRecovery.API.Middlewares;
using SmartRecovery.Application;
using SmartRecovery.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Camadas da aplicação ─────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializa enums como strings (ex: "APPROVED" em vez de 0)
        // Facilita muito a leitura da API e do Swagger
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Smart Recovery API",
        Version = "v1",
        Description = """
            Plataforma inteligente de recuperação de pagamentos recusados.

            Quando uma cobrança falha, o Smart Recovery analisa o histórico do cliente,
            o motivo da recusa e calcula um **Recovery Score** para determinar a melhor
            estratégia de recuperação automaticamente.

            Desenvolvida em C# / .NET 8 como projeto de portfólio.
            """,
        Contact = new OpenApiContact
        {
            Name = "Smart Recovery",
            Email = "smartrecovery@example.com"
        }
    });

    // Inclui os comentários XML dos controllers no Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── CORS ──────────────────────────────────────────────────────────────────────
// Permite que o frontend React (em desenvolvimento) acesse a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // porta padrão do Vite
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middlewares ───────────────────────────────────────────────────────────────
// Ordem importa: o ExceptionHandlingMiddleware deve vir antes dos demais
// para capturar erros que acontecerem em qualquer parte do pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Recovery API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:5000
        options.DocumentTitle = "Smart Recovery API";
    });
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.MigrateAndSeedDatabaseAsync();

app.Run();

// Necessário para que o projeto de testes de integração possa referenciar o Program
public partial class Program { }
