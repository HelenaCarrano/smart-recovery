using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartRecovery.API.Extensions;
using SmartRecovery.API.Middlewares;
using SmartRecovery.Application;
using SmartRecovery.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Enums como strings ("Approved") em vez de números — muito mais legível no Swagger.
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

// Precisa vir antes de tudo para capturar erros de qualquer parte do pipeline.
app.UseExceptionHandler();

// Habilita Swagger para fácil visualização e teste no portfólio
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Recovery API v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "Smart Recovery API";
});

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

// Em testes de integração o schema é migrado sem o seed completo (80 clientes simulados) — cada
// teste popula só os dados que precisa via DbContext direto, então o seed ali seria puro overhead.
if (app.Environment.IsEnvironment("Testing"))
    await app.MigrateDatabaseAsync();
else
    await app.MigrateAndSeedDatabaseAsync();

app.Run();

// Necessário para que o projeto de testes de integração possa referenciar o Program
public partial class Program { }
