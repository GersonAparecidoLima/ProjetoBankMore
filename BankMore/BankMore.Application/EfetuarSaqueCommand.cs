using MediatR;
using System;

namespace BankMore.Application.Accounts.Commands
{
    public record EfetuarSaqueCommand(
        Guid IdConta,
        decimal Valor,
        string ChaveIdempotencia
    ) : IRequest<Unit>;
}