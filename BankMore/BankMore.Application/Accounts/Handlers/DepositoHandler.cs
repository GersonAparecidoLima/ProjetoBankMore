using BankMore.Application.Accounts.Commands;
using BankMore.Infrastructure.Data;
using Dapper;
using MediatR;
using System.Data;

namespace BankMore.Application.Handlers
{
    public class DepositoHandler : IRequestHandler<EfetuarDepositoCommand, Unit>
    {
        private readonly DbSession _session;

        public DepositoHandler(DbSession session)
        {
            _session = session;
        }

        public async Task<Unit> Handle(EfetuarDepositoCommand request, CancellationToken cancellationToken)
        {
            // Garante que a conexão está aberta
            if (_session.Connection.State == ConnectionState.Closed)
                _session.Connection.Open();

            // Como é apenas um insert simples, não é obrigatório usar transaction, 
            // mas é boa prática manter o padrão caso queira adicionar logs depois.
            using var transaction = _session.Connection.BeginTransaction();

            try
            {
                // Registro de CRÉDITO ('C') na conta informada
                var sql = @"INSERT INTO Movimento (IdContaCorrente, TipoMovimento, Valor, DataMovimento) 
                           VALUES (@IdConta, 'C', @Valor, GETDATE())";

                await _session.Connection.ExecuteAsync(sql, new
                {
                    request.IdConta,
                    request.Valor
                }, transaction);

                transaction.Commit();
                return Unit.Value;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}