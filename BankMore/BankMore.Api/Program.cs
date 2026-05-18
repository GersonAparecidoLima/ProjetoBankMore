using BankMore.Infrastructure.Data;
using BankMore.Infrastructure.Repositories;
using BankMore.Application.Accounts.Handlers;
using BankMore.Domain.Interfaces; // Referência para onde está um dos seus handlers

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar a Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar o DbSession (Scoped: uma conexão por requisição HTTP)
builder.Services.AddScoped(provider => new DbSession(connectionString));

// 3. Registrar os Repositórios
builder.Services.AddScoped<ContaCorrenteRepository>();

// Adicione junto com os outros registros de serviço:
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();

// 4. Registrar o MediatR 
// Ele vai escanear o projeto 'Application' para achar todos os Handlers automaticamente
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CriarContaHandler).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();