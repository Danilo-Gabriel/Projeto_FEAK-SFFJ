using Application.services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod
        | HttpLoggingFields.RequestPath
        | HttpLoggingFields.RequestQuery
        | HttpLoggingFields.ResponseStatusCode
        | HttpLoggingFields.Duration;
});

builder.WebHost.UseUrls("http://0.0.0.0:7000");

builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo { Title = "FEAK", Version = "v1" });
     
     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
     {
         Name = "Authorization",
         Type = SecuritySchemeType.Http,
         Scheme = "bearer",
         BearerFormat = "JWT",
         In = ParameterLocation.Header,
         Description = "Informe o token JWT: Bearer {seu token}"
     });

     c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
         {
             new OpenApiSecurityScheme
             {
                 Reference = new OpenApiReference
                 {
                     Type = ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             Array.Empty<string>()
         }
     });

});


Configuration.AddContextsServices(builder.Services, builder.Configuration);
Configuration.AddKeyclokServices(builder.Services, builder.Configuration);
Configuration.AddServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FEAK"));
}

app.UseHttpLogging();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();