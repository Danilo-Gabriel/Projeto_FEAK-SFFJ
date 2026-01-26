using Application.services;
using Keycloak.AuthServices.Authentication;
using KellermanSoftware.CompareNetObjects;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddControllers();



builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "FEAK", Version = "v1" });
//     var security = new OpenApiSecurityScheme
//     {
//         Name = "Keycloak",
//         In = ParameterLocation.Header,
//         Type = SecuritySchemeType.OpenIdConnect,
//         OpenIdConnectUrl =
//             new Uri(
//                 $"{builder.Configuration["Keycloak:auth-server-url"]}realms/{builder.Configuration["Keycloak:realm"]}/.well-known/openid-configuration"),
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         Reference = new OpenApiReference
//         {
//             Id = "Bearer",
//             Type = ReferenceType.SecurityScheme
//         }
//     };
//     c.AddSecurityDefinition(security.Reference.Id, security);
//     c.AddSecurityRequirement(new OpenApiSecurityRequirement
//     {
//         {security, Array.Empty<string>()}
//     });
});


Configuration.AddContextsServices(builder.Services, builder.Configuration);
Configuration.AddServices(builder.Services);

var app = builder.Build();

app.UseCors(a => a.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FEAK"));
}

app.Run();