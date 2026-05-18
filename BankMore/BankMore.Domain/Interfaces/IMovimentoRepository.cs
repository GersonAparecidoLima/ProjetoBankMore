using System;
using System.Threading.Tasks;

namespace BankMore.Domain.Interfaces
{
    public interface IMovimentoRepository
    {
        Task<decimal> ObterSaldoAsync(Guid idConta);
        Task InserirMovimentoAsync(Guid idConta, decimal valor, char tipo);
    }
}