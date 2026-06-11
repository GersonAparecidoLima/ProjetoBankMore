using BankMore.Application.Accounts.Commands;
using BankMore.Application.Interfaces;
using BankMore.Infrastructure.Data;
using Dapper;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.Application.Accounts.Handlers;

public class EfetuarLoginCommandHandler(DbSession dbSession, IJwtTokenGenerator jwtTokenGenerator)
    : IRequestHandler<EfetuarLoginCommand, string>
{
    public async Task<string> Handle(EfetuarLoginCommand request, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT IdContaCorrente, Numero, Senha, Salt
            FROM ContaCorrente
            WHERE Numero = @Identificador";

        var conta = await dbSession.Connection.QueryFirstOrDefaultAsync<dynamic>(
            query, new { Identificador = request.Identificador });

        if (conta == null)
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

        string hashDigitado = GerarHashSHA256(request.Senha, (string)conta.Salt);

        if (hashDigitado != (string)conta.Senha)
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

        return jwtTokenGenerator.GerarToken(
            (Guid)conta.IdContaCorrente,
            Convert.ToString(conta.Numero)!
        );
    }

    private static string GerarHashSHA256(string senha, string salt)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senha + salt));
        return Convert.ToBase64String(bytes);
    }
}
