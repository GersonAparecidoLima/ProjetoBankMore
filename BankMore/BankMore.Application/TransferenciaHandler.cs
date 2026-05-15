using BankMore.Infrastructure.Data;
using MediatR;
using Dapper; // Necessário para o ExecuteAsync
using System.Data; // Necessário para IDbTransaction

namespace BankMore.Application.Handlers
{
    // 1. Tornar pública e herdar da interface do MediatR
    public class TransferenciaHandler : IRequestHandler<EfetuarTransferenciaCommand, Unit>
    {
        private readonly DbSession _session;

        // 2. Construtor para receber a sessão do banco
        public TransferenciaHandler(DbSession session)
        {
            _session = session;
        }

        public async Task<Unit> Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            if (_session.Connection.State == ConnectionState.Closed)
                _session.Connection.Open();

            using var transaction = _session.Connection.BeginTransaction();

            try
            {
                // --- ADICIONE ESTA VALIDAÇÃO AQUI ---
                var saldoDisponivel = await ObterSaldoAtual(request.IdOrigem, transaction);

                if (saldoDisponivel < request.Valor)
                {
                    throw new Exception("Saldo insuficiente para realizar a transferência.");
                }
                // ------------------------------------

                // 1. Registro de DÉBITO na conta de origem
                var sqlDebito = @"INSERT INTO Movimento (IdContaCorrente, TipoMovimento, Valor) 
                         VALUES (@IdOrigem, 'D', @Valor)";
                await _session.Connection.ExecuteAsync(sqlDebito, request, transaction);

                // 2. Registro de CRÉDITO na conta de destino
                var sqlCredito = @"INSERT INTO Movimento (IdContaCorrente, TipoMovimento, Valor) 
                          VALUES (@IdDestino, 'C', @Valor)";
                await _session.Connection.ExecuteAsync(sqlCredito, request, transaction);

                // 3. Registro na tabela de Transferencia
                //var sqlTransf = @"INSERT INTO Transferencia (IdContaCorrenteOrigem, IdContaCorrenteDestino, Valor) 
                //          VALUES (@IdOrigem, @IdDestino, @Valor)";
                //await _session.Connection.ExecuteAsync(sqlTransf, request, transaction);

                // 5. Registro na tabela de Transferencia (Certifique-se de passar a chave aqui!)
                var sqlTransf = @"INSERT INTO Transferencia (IdContaCorrenteOrigem, IdContaCorrenteDestino, Valor, ChaveIdempotencia) 
                  VALUES (@IdOrigem, @IdDestino, @Valor, @ChaveIdempotencia)";

                // O 'request' precisa ter a propriedade ChaveIdempotencia preenchida
                await _session.Connection.ExecuteAsync(sqlTransf, request, transaction);



                transaction.Commit();
                return Unit.Value;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<decimal> ObterSaldoAtual(Guid idConta, IDbTransaction transaction)
        {
            var sql = @"SELECT 
                ISNULL(SUM(CASE WHEN TipoMovimento = 'C' THEN Valor ELSE -Valor END), 0) 
                FROM Movimento 
                WHERE IdContaCorrente = @idConta";

            return await _session.Connection.QueryFirstOrDefaultAsync<decimal>(sql, new { idConta }, transaction);
        }

    }
}