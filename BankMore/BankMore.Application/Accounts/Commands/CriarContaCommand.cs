using System;
using MediatR;

namespace BankMore.Application.Accounts.Commands
{
    // A planta perfeita que define as propriedades que o Handler e o Swagger esperam
    public record CriarContaCommand(
        string Nome,
        string Documento,
        decimal SaldoInicial,
        string Senha
    ) : IRequest<Guid>;
}