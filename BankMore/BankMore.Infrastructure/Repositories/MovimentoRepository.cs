using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Dapper;
using System.Data;

namespace BankMore.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DbSession _session;

        public MovimentoRepository(DbSession session) => _session = session;

        public async Task<decimal> ObterSaldoAsync(Guid idConta)
        {
            const string sql = @"
                SELECT COALESCE(SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 0)
                FROM Movimento
                WHERE IdContaCorrente = @Id";

            return await _session.Connection.QueryFirstOrDefaultAsync<decimal>(sql, new { Id = idConta });
        }

        public async Task InserirMovimentoAsync(Guid idConta, decimal valor, char tipo)
        {
            const string sql = @"
                INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                VALUES (@IdMovimento, @IdContaCorrente, @Data, @Tipo, @Valor)";

            await _session.Connection.ExecuteAsync(sql, new
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = idConta,
                Data = DateTime.Now,
                Tipo = tipo,
                Valor = valor
            });
        }

        public async Task<IEnumerable<Movimento>> ObterMovimentosAsync(Guid idConta)
        {
            const string sql = @"
                SELECT IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor
                FROM Movimento
                WHERE IdContaCorrente = @Id
                ORDER BY DataMovimento DESC";

            return await _session.Connection.QueryAsync<Movimento>(sql, new { Id = idConta });
        }

        public async Task EfetuarTransferenciaAsync(Guid idOrigem, Guid idDestino, decimal valor, string chaveIdempotencia)
        {
            using var transaction = _session.Connection.BeginTransaction();
            try
            {
                var saldo = await ObterSaldoNaTransacaoAsync(idOrigem, transaction);
                if (saldo < valor)
                    throw new Exception("Saldo insuficiente para realizar a transferência.");

                const string sqlDebito = @"
                    INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                    VALUES (@Id, @IdConta, @Data, 'D', @Valor)";

                await _session.Connection.ExecuteAsync(sqlDebito, new
                {
                    Id = Guid.NewGuid(), IdConta = idOrigem, Data = DateTime.Now, Valor = valor
                }, transaction);

                const string sqlCredito = @"
                    INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor)
                    VALUES (@Id, @IdConta, @Data, 'C', @Valor)";

                await _session.Connection.ExecuteAsync(sqlCredito, new
                {
                    Id = Guid.NewGuid(), IdConta = idDestino, Data = DateTime.Now, Valor = valor
                }, transaction);

                const string sqlTransf = @"
                    INSERT INTO Transferencia (IdContaCorrenteOrigem, IdContaCorrenteDestino, Valor, ChaveIdempotencia)
                    VALUES (@IdOrigem, @IdDestino, @Valor, @Chave)";

                await _session.Connection.ExecuteAsync(sqlTransf, new
                {
                    IdOrigem = idOrigem, IdDestino = idDestino, Valor = valor, Chave = chaveIdempotencia
                }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<decimal> ObterSaldoNaTransacaoAsync(Guid idConta, IDbTransaction transaction)
        {
            const string sql = @"
                SELECT ISNULL(SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 0)
                FROM Movimento
                WHERE IdContaCorrente = @Id";

            return await _session.Connection.QueryFirstOrDefaultAsync<decimal>(sql, new { Id = idConta }, transaction);
        }
    }
}
