using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Dapper;
using System;
using System.Threading.Tasks;

namespace BankMore.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly DbSession _session;

        public MovimentoRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<decimal> ObterSaldoAsync(Guid idConta)
        {
            var sqlSaldo = @"
                SELECT COALESCE(
                    SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 
                    0
                ) 
                FROM Movimento 
                WHERE IdContaCorrente = @Id";

            return await _session.Connection.QueryFirstOrDefaultAsync<decimal>(sqlSaldo, new { Id = idConta });
        }

        public async Task InserirMovimentoAsync(Guid idConta, decimal valor, char tipo)
        {
            var sqlMovimento = @"INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor) 
                                 VALUES (@IdMovimento, @IdContaCorrente, @Data, @Tipo, @Valor);";

            await _session.Connection.ExecuteAsync(sqlMovimento, new
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = idConta,
                Data = DateTime.Now,
                Tipo = tipo,
                Valor = valor
            });
        }
    }
}