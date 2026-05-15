using BankMore.Domain.Entities;
using BankMore.Infrastructure.Data;
using Dapper;
using System;
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

        // 1. Método de busca corrigido para usar a coluna real: IdContaCorrente
        public async Task<ContaCorrente> ObterPorIdAsync(Guid idContaCorrente)
        {
            var sql = "SELECT * FROM ContaCorrente WHERE IdContaCorrente = @IdContaCorrente";
            return await _session.Connection.QueryFirstOrDefaultAsync<ContaCorrente>(sql, new { IdContaCorrente = idContaCorrente });
        }

        // 2. O seu método impecável de criação de contas
        public async Task<Guid> CriarContaBlanchAsync(string nome, string senhaPura)
        {
            // 1. Geramos o ID que vai mapear para [IdContaCorrente]
            var novoId = Guid.NewGuid();

            // 2. Geramos um número de conta aleatório de 5 dígitos para [Numero]
            var numeroConta = new Random().Next(10000, 99999);

            // 3. Geramos o Salt e o Hash da senha para [Salt] e [Senha]
            var salt = Guid.NewGuid().ToString().Substring(0, 8);
            var senhaHash = CalcularHashSHA256(senhaPura, salt);

            // 4. O SQL perfeitamente alinhado com o seu SELECT do SQL Server
            var sql = @"INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt) 
                        VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo, @Senha, @Salt);";

            // 5. Parâmetros que o Dapper vai injetar nas variáveis com @
            var parametros = new
            {
                IdContaCorrente = novoId,
                Numero = numeroConta,
                Nome = nome,
                Ativo = true,
                Senha = senhaHash,
                Salt = salt
            };

            // 6. Executa o comando no banco de dados
            await _session.Connection.ExecuteAsync(sql, parametros);

            // 7. Retorna o ID gerado
            return novoId;
        }

        // Método auxiliar para gerar o hash da senha dentro do repositório
        // Método auxiliar corrigido (trocado 'senate' por 'senha')
        private string CalcularHashSHA256(string senha, string salt)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var combinado = senha + salt; // Corrigido aqui!
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(combinado));
            return Convert.ToBase64String(bytes);
        }
    }
}