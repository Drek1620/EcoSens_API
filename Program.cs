using EcoSens_API.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using EcoSens_API.Services;
using System.Net;



var builder = WebApplication.CreateBuilder(args);

//  CORS con dominio del frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7265") // Ajusta estos
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
Console.WriteLine("Iniciando EcoSens_API...");
// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

//Configurar MongoDB como servicio
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddControllers();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Asegúrate de aplicar la política aquí
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
