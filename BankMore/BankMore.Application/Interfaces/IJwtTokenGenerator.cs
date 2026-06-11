namespace BankMore.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GerarToken(Guid idConta, string numero);
}
