using MediatR;
using System;

namespace BankMore.Application.Accounts.Commands
{
    // O record é perfeito aqui pois é um objeto de transferência de dados imutável
    public record EfetuarDepositoCommand(Guid IdConta, decimal Valor) : IRequest<Unit>;
}