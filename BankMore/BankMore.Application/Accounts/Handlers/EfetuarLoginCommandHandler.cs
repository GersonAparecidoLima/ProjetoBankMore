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
    public class EfetuarLoginCommandHandler : IRequestHandler<EfetuarLoginCommand, string>
    {
        private readonly DbSession _dbSession;

        public EfetuarLoginCommandHandler(DbSession dbSession)
        {
            _dbSession = dbSession;
        }

        public async Task<string> Handle(EfetuarLoginCommand request, CancellationToken cancellationToken)
        {
            // Busca dinâmica: pode ser por Número ou por outra identificação cadastrada
            var query = @"
                SELECT IdContaCorrente, Numero, Senha, Salt 
                FROM ContaCorrente 
                WHERE Numero = @Identificador";

            var conta = await _dbSession.Connection.QueryFirstOrDefaultAsync<dynamic>(query, new { Identificador = request.Identificador });

            if (conta == null)
            {
                throw new UnauthorizedAccessException("USER_UNAUTHORIZED");
            }

            // Verifica o Hash recriando-o com o Salt recuperado do banco
            string saltDoBanco = conta.Salt;
            string hashSenhaDigitada = GerarHashSHA256(request.Senha, saltDoBanco);

            if (hashSenhaDigitada != conta.Senha)
            {
                throw new UnauthorizedAccessException("USER_UNAUTHORIZED");
            }

            // Aqui você chamará a sua geração real de JWT (passando o IdContaCorrente nas Claims)
            string tokenJWT = "TOKEN_JWT_VALIDO_GERADO_PARA_CONTA_" + conta.Numero;
            return tokenJWT;
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