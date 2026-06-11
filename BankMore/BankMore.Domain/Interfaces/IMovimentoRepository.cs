using BankMore.Domain.Entities;

namespace BankMore.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<decimal> ObterSaldoAsync(Guid idConta);
        Task InserirMovimentoAsync(Guid idConta, decimal valor, char tipo);
        Task<IEnumerable<Movimento>> ObterMovimentosAsync(Guid idConta);
        Task EfetuarTransferenciaAsync(Guid idOrigem, Guid idDestino, decimal valor, string chaveIdempotencia);
    }
}