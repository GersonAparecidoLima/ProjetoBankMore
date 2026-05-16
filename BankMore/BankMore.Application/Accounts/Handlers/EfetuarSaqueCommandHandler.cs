using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using BankMore.Infrastructure.Data;
using BankMore.Application.Accounts.Commands;

namespace BankMore.Application.Accounts.Handlers
{
    public class EfetuarSaqueCommandHandler : IRequestHandler<EfetuarSaqueCommand, Unit>
    {
        private readonly DbSession _session;

        public EfetuarSaqueCommandHandler(DbSession session)
        {
            _session = session;
        }

        public async Task<Unit> Handle(EfetuarSaqueCommand request, CancellationToken cancellationToken)
        {
            // 1. Calcula o saldo real somando os Créditos ('C') e subtraindo os Débitos ('D') da tabela Movimento
            var sqlSaldo = @"
        SELECT COALESCE(
            SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 
            0
        ) 
        FROM Movimento 
        WHERE IdContaCorrente = @Id";

            var saldoReal = await _session.Connection.QueryFirstOrDefaultAsync<decimal>(sqlSaldo, new { Id = request.IdConta });

            // 2. REGRA DE NEGÓCIO: Validar se o saldo calculado é suficiente
            if (saldoReal < request.Valor)
            {
                throw new Exception($"Saldo insuficiente para realizar o saque. Saldo atual: {saldoReal:C}");
            }

            // 3. Como o seu saldo é baseado em movimentos, NÃO precisamos dar UPDATE em tabela nenhuma!
            // Apenas inserimos o registro de Débito ('D') e o saldo se atualiza automaticamente na próxima consulta.
            var sqlMovimento = @"INSERT INTO Movimento (IdMovimento, IdContaCorrente, DataMovimento, TipoMovimento, Valor) 
                         VALUES (@IdMovimento, @IdContaCorrente, @Data, 'D', @Valor);";

            await _session.Connection.ExecuteAsync(sqlMovimento, new
            {
                IdMovimento = Guid.NewGuid(),
                IdContaCorrente = request.IdConta,
                Data = DateTime.Now,
                Valor = request.Valor
            });

            return Unit.Value;
        }

    }
}