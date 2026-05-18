using BankMore.Application.Accounts.Commands;
using BankMore.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BankMore.Application.Handlers
{
    public class DepositoHandler : IRequestHandler<EfetuarDepositoCommand, Unit>
    {
        private readonly IMovimentoRepository _repository;

        // 1. Construtor único injetando apenas a nossa Interface de Movimentos
        public DepositoHandler(IMovimentoRepository repository)
        {
            _repository = repository;
        }

        // 2. Método Handle único e limpo
        public async Task<Unit> Handle(EfetuarDepositoCommand request, CancellationToken cancellationToken)
        {
            // Toda a parte de Dapper, conexão e transação agora acontece de forma segura
            // dentro da implementação do repositório. O Handler só dispara o comando!
            await _repository.InserirMovimentoAsync(request.IdConta, request.Valor, 'C');

            return Unit.Value;
        }
    }
}