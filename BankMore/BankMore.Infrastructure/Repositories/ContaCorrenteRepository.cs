using BankMore.Domain.Entities;
using BankMore.Domain.Interfaces;
using BankMore.Infrastructure.Data;
using Dapper;

namespace BankMore.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly DbSession _session;

        public ContaCorrenteRepository(DbSession session) => _session = session;

        public async Task<ContaCorrente?> ObterPorIdAsync(Guid id)
        {
            const string sql = @"
                SELECT IdContaCorrente as IdConta, Numero, Nome, Ativo, Senha, Salt
                FROM ContaCorrente
                WHERE IdContaCorrente = @Id";

            return await _session.Connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Id = id });
        }

        public async Task<ContaCorrente?> ObterPorNumeroAsync(string numero)
        {
            const string sql = @"
                SELECT IdContaCorrente as IdConta, Numero, Nome, Ativo, Senha, Salt
                FROM ContaCorrente
                WHERE Numero = @Numero";

            return await _session.Connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { Numero = numero });
        }

        public async Task<Guid> CriarContaAsync(string nome, string senha)
        {
            var id = Guid.NewGuid();
            var numeroConta = new Random().Next(10000, 99999);
            var salt = Guid.NewGuid().ToString()[..8];
            var senhaHash = CalcularHashSHA256(senha, salt);

            const string sql = @"
                INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt)
                VALUES (@Id, @Numero, @Nome, @Ativo, @Senha, @Salt)";

            await _session.Connection.ExecuteAsync(sql, new
            {
                Id = id,
                Numero = numeroConta,
                Nome = nome,
                Ativo = true,
                Senha = senhaHash,
                Salt = salt
            });

            return id;
        }

        public async Task<(Guid id, string numero)> CadastrarContaAsync(string cpf, string senha)
        {
            var id = Guid.NewGuid();
            var numeroConta = new Random().Next(10000, 99999).ToString();
            var salt = Guid.NewGuid().ToString()[..8];
            var senhaHash = CalcularHashSHA256(senha, salt);

            const string sql = @"
                INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt)
                VALUES (@Id, @Numero, @Nome, @Ativo, @Senha, @Salt)";

            await _session.Connection.ExecuteAsync(sql, new
            {
                Id = id.ToString().ToUpper(),
                Numero = numeroConta,
                Nome = "Cliente Novo " + numeroConta,
                Ativo = 1,
                Senha = senhaHash,
                Salt = salt
            });

            return (id, numeroConta);
        }

        private static string CalcularHashSHA256(string senha, string salt)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(senha + salt));
            return Convert.ToBase64String(bytes);
        }
    }
}
