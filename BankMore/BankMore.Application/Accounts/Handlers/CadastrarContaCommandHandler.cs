using BankMore.Application.Accounts.Commands;
using BankMore.Infrastructure.Data;
using Dapper;
using MediatR;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankMore.Application.Accounts.Handlers
{
    // CORRIGIDO: Mudamos o segundo parâmetro de 'string' para 'CadastrarContaResult'
    public class CadastrarContaCommandHandler : IRequestHandler<CadastrarContaCommand, CadastrarContaResult>
    {
        private readonly DbSession _dbSession;

        public CadastrarContaCommandHandler(DbSession dbSession)
        {
            _dbSession = dbSession;
        }

        // CORRIGIDO: Mudamos o retorno da Task para 'CadastrarContaResult'
        public async Task<CadastrarContaResult> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
        {
            // Validação simples de CPF (Apenas tamanho para o exemplo, pode usar sua lógica de dígitos)
            if (string.IsNullOrWhiteSpace(request.Cpf) || request.Cpf.Length != 11)
            {
                throw new ArgumentException("O CPF fornecido é inválido.", "INVALID_DOCUMENT");
            }

            // Gerando Salt e Hash (Igual ao formato que você tem no banco de dados!)
            string salt = Guid.NewGuid().ToString().Substring(0, 8);
            string senhaHasheada = GerarHashSHA256(request.Senha, salt);
            string numeroConta = new Random().Next(10000, 99999).ToString();

            // CORRIGIDO: Isolamos o GUID gerado aqui para usá-lo tanto no INSERT quanto no RETURN
            string idNovo = Guid.NewGuid().ToString().ToUpper();

            var query = @"
                INSERT INTO ContaCorrente (IdContaCorrente, Numero, Nome, Ativo, Senha, Salt)
                VALUES (@Id, @Numero, @Nome, @Ativo, @Senha, @Salt)";

            var parametros = new
            {
                Id = idNovo, // Usa a nossa variável com o GUID gerado
                Numero = numeroConta,
                Nome = "Cliente Novo " + numeroConta,
                Ativo = 1,
                Senha = senhaHasheada,
                Salt = salt
            };

            await _dbSession.Connection.ExecuteAsync(query, parametros);

            // CORRIGIDO: Agora retornamos a classe completa com os dois dados!
            return new CadastrarContaResult
            {
                IdConta = idNovo,
                NumeroConta = numeroConta
            };
        }

        private string GerarHashSHA256(string senha, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string senhaComSalt = senha + salt;
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaComSalt));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}