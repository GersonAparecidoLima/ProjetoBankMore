using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankMore.Domain.Interfaces;
using BankMore.Application.Accounts.Commands;

namespace BankMore.Application.Accounts.Handlers
{
    public class EfetuarSaqueCommandHandler : IRequestHandler<EfetuarSaqueCommand, Unit>
    {
        private readonly IMovimentoRepository _repository;

        // O segredo está aqui: Injetamos a Interface!
        public EfetuarSaqueCommandHandler(IMovimentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(EfetuarSaqueCommand request, CancellationToken cancellationToken)
        {
            // 1. Puxa o saldo pela interface de forma abstrata
            var saldoReal = await _repository.ObterSaldoAsync(request.IdConta);

            // 2. REGRA DE NEGÓCIO: Valida se o saldo calculado é suficiente
            if (saldoReal < request.Valor)
            {
                throw new Exception($"Saldo insuficiente para realizar o saque. Saldo atual: {saldoReal:C}");
            }

            // 3. Registra o Débito ('D') pela interface
            await _repository.InserirMovimentoAsync(request.IdConta, request.Valor, 'D');

            return Unit.Value;
        }
    }
}