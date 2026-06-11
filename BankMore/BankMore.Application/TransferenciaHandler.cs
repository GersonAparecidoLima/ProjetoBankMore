using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers
{
    public class TransferenciaHandler : IRequestHandler<EfetuarTransferenciaCommand, Unit>
    {
        private readonly IMovimentoRepository _movimentoRepository;

        public TransferenciaHandler(IMovimentoRepository movimentoRepository) => _movimentoRepository = movimentoRepository;

        public async Task<Unit> Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            await _movimentoRepository.EfetuarTransferenciaAsync(
                request.IdOrigem,
                request.IdDestino,
                request.Valor,
                request.ChaveIdempotencia);

            return Unit.Value;
        }
    }
}
