using BankMore.Domain.Entities;
using BankMore.Infrastructure.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Infrastructure.Repositories
{
    public class ContaCorrenteRepository
    {
        private readonly DbSession _session;

        public ContaCorrenteRepository(DbSession session)
        {
            _session = session;
        }

        public async Task<Guid> AdicionarAsync(ContaCorrente conta)
        {
            var sql = @"INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt) 
                VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @Senha, @Salt)";

            await _session.Connection.ExecuteAsync(sql, conta);

            // Antes você estava tentando retornar conta.Id, agora use o nome correto:
            return conta.IdContaCorrente;
        }

        public async Task<ContaCorrente> ObterPorNumeroAsync(int numero)
        {
            var sql = "SELECT * FROM CONTACORRENTE WHERE numero = @numero";
            return await _session.Connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { numero });
        }
    }
}
