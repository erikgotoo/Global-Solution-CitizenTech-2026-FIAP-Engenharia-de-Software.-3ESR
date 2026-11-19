using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Zeladoria.Api.Middleware;
using Zeladoria.Core.Services;
using Zeladoria.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Enums como texto (ex.: "Agente", "EmAnalise") também no OpenAPI/Scalar e nas respostas de erro.
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=zeladoria.db";
builder.Services.AddInfrastructure(connectionString);

// Injeção de dependência: serviços de aplicação.
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IHistoricoService, HistoricoService>();

var app = builder.Build();

// Aplica automaticamente as migrations versionadas em src/Zeladoria.Infrastructure/Migrations.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ZeladoriaDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.MapControllers();

app.Run();
