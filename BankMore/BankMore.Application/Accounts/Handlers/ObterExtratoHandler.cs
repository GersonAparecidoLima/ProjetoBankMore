using BankMore.Infrastructure.Data;
using Dapper;
using MediatR;

namespace BankMore.Application.Handlers
{
    public class ObterExtratoHandler : IRequestHandler<ObterExtratoQuery, ExtratoResponse>
    {
        private readonly DbSession _session;

        public ObterExtratoHandler(DbSession session) => _session = session;

        public async Task<ExtratoResponse> Handle(ObterExtratoQuery request, CancellationToken cancellationToken)
        {
            // 1. Query para o Saldo e Nome
            var sqlDados = @"
                SELECT c.IdContaCorrente as IdConta, c.Nome,
                ISNULL(SUM(CASE WHEN m.TipoMovimento = 'C' THEN m.Valor ELSE -m.Valor END), 0) as SaldoAtual
                FROM ContaCorrente c
                LEFT JOIN Movimento m ON c.IdContaCorrente = m.IdContaCorrente
                WHERE c.IdContaCorrente = @IdConta
                GROUP BY c.IdContaCorrente, c.Nome";

            // 2. Query para a Lista de Movimentações (ordenada pela mais recente)
            var sqlMovimentos = @"
                SELECT IdMovimento, DataMovimento, TipoMovimento as Tipo, Valor 
                FROM Movimento 
                WHERE IdContaCorrente = @IdConta 
                ORDER BY DataMovimento DESC";

            var dadosConta = await _session.Connection.QueryFirstOrDefaultAsync<dynamic>(sqlDados, new { request.IdConta });

            if (dadosConta == null) return null;

            var movimentos = await _session.Connection.QueryAsync<MovimentacaoDto>(sqlMovimentos, new { request.IdConta });

            return new ExtratoResponse(
                (Guid)dadosConta.IdConta,
                (string)dadosConta.Nome,
                (decimal)dadosConta.SaldoAtual,
                movimentos.ToList()
            );
        }
    }
}