// Cria e configura o objeto `WebApplicationBuilder`, que � usado para configurar os servi�os e o pipeline de solicita��o HTTP.
// using FeakApi.Domain.Handlers;
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

// Injeta o reposit�rio gen�rico e os espec�ficos
//builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
//builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ProdutoRepository>();
builder.Services.AddScoped<ProdutoService>();

// criar um serviço 

// builder.Services.AddTransient<ICreateCustomerHandler, CreateCustomerHandler>();


// Configura os servi�os necess�rios para a aplica��o. 
// `AddControllers` adiciona suporte para controllers MVC, o que permite criar endpoints de API e renderizar views.
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Constr�i o objeto `WebApplication` com base nas configura��es fornecidas e no cont�iner de servi�os configurado.
var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplica a pol�tica CORS no pipeline
app.UseCors("AllowFrontend");

//using var conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
//try
//{
//    conn.Open();
//    Console.WriteLine("Conex�o com banco aberta com sucesso.");
//}
//catch (Exception ex)
//{
//    Console.WriteLine("Erro ao abrir conex�o: " + ex.Message);
//}


// Mapeia os controllers para os endpoints dispon�veis na aplica��o.
// Isso faz com que os controllers sejam acess�veis atrav�s das rotas definidas nas suas classes de controller.
app.MapControllers();

// Inicia a aplica��o e come�a a escutar as solicita��es HTTP.
app.Run();