using BankMore.Infrastructure.Data;
using Dapper;
using MediatR;
using System.Data;

namespace BankMore.Application.Handlers
{
    public class ObterSaldoHandler : IRequestHandler<ObterSaldoQuery, SaldoResponse>
    {
        private readonly DbSession _session;

        public ObterSaldoHandler(DbSession session)
        {
            _session = session;
        }

        public async Task<SaldoResponse> Handle(ObterSaldoQuery request, CancellationToken cancellationToken)
        {
            // Garante que a conexão esteja aberta para leitura
            if (_session.Connection.State == ConnectionState.Closed)
                _session.Connection.Open();

            var sql = @"
                SELECT 
                    c.IdContaCorrente as IdConta, 
                    c.Nome,
                    ISNULL(SUM(CASE WHEN m.TipoMovimento = 'C' THEN m.Valor ELSE -m.Valor END), 0) as Saldo
                FROM ContaCorrente c
                LEFT JOIN Movimento m ON c.IdContaCorrente = m.IdContaCorrente
                WHERE c.IdContaCorrente = @IdConta
                GROUP BY c.IdContaCorrente, c.Nome";

            return await _session.Connection.QueryFirstOrDefaultAsync<SaldoResponse>(sql, new { request.IdConta });
        }
    }
}