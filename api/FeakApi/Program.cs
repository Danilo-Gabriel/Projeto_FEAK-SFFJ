// Cria e configura o objeto `WebApplicationBuilder`, que é usado para configurar os serviços e o pipeline de solicitação HTTP.
using FeakApi.models.repository;
using FeakApi.models.repository.interfaces;
using FeakApi.models.services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura CORS
// 1. Configurar CORS para liberar seu frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:4200", "https://localhost:7250/swagger/index.html") // frontend
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials(); // se precisar enviar cookies, descomente
    });
});


// Dapper
builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeta o repositório genérico e os específicos
//builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();

// Configura os serviços necessários para a aplicação. 
// `AddControllers` adiciona suporte para controllers MVC, o que permite criar endpoints de API e renderizar views.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Constrói o objeto `WebApplication` com base nas configurações fornecidas e no contêiner de serviços configurado.
var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplica a política CORS no pipeline
app.UseCors("AllowFrontend");

//using var conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
//try
//{
//    conn.Open();
//    Console.WriteLine("Conexão com banco aberta com sucesso.");
//}
//catch (Exception ex)
//{
//    Console.WriteLine("Erro ao abrir conexão: " + ex.Message);
//}


// Mapeia os controllers para os endpoints disponíveis na aplicação.
// Isso faz com que os controllers sejam acessíveis através das rotas definidas nas suas classes de controller.
app.MapControllers();

// Inicia a aplicação e começa a escutar as solicitações HTTP.
app.Run();