using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers
{
    public class ObterSaldoHandler : IRequestHandler<ObterSaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ObterSaldoHandler(IContaCorrenteRepository contaRepository, IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<SaldoResponse> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null) return null;

            var saldo = await _movimentoRepository.ObterSaldoAsync(request.IdConta);
            return new SaldoResponse(conta.IdConta, conta.Nome, saldo);
        }
    }
}
