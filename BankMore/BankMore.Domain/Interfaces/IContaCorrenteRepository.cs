using BankMore.Domain.Entities;

namespace BankMore.Domain.Interfaces
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente?> ObterPorIdAsync(Guid id);
        Task<ContaCorrente?> ObterPorNumeroAsync(string numero);
        Task<Guid> CriarContaAsync(string nome, string senha);
        Task<(Guid id, string numero)> CadastrarContaAsync(string cpf, string senha);
    }
}
