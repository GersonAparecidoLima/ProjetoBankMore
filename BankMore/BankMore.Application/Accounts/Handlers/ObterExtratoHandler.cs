using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers
{
    public class ObterExtratoHandler : IRequestHandler<ObterExtratoQuery, ExtratoResponse>
    {
        private readonly IContaCorrenteRepository _contaRepository;
        private readonly IMovimentoRepository _movimentoRepository;

        public ObterExtratoHandler(IContaCorrenteRepository contaRepository, IMovimentoRepository movimentoRepository)
        {
            _contaRepository = contaRepository;
            _movimentoRepository = movimentoRepository;
        }

        public async Task<ExtratoResponse> Handle(ObterExtratoQuery request, CancellationToken cancellationToken)
        {
            var conta = await _contaRepository.ObterPorIdAsync(request.IdConta);
            if (conta == null) return null;

            var saldo = await _movimentoRepository.ObterSaldoAsync(request.IdConta);
            var movimentos = await _movimentoRepository.ObterMovimentosAsync(request.IdConta);

            var transacoes = movimentos
                .Select(m => new MovimentacaoDto(m.IdMovimento, m.DataMovimento, m.TipoMovimento.ToString(), m.Valor))
                .ToList();

            return new ExtratoResponse(conta.IdConta, conta.Nome, saldo, transacoes);
        }
    }
}
