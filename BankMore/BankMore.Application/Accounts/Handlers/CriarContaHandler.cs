using BankMore.Application.Accounts.Commands;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Guid>
    {
        private readonly IContaCorrenteRepository _repository;

        public CriarContaHandler(IContaCorrenteRepository repository) => _repository = repository;

        public async Task<Guid> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
                throw new ArgumentException("O nome é obrigatório para a criação da conta.");

            return await _repository.CriarContaAsync(request.Nome, request.Senha);
        }
    }
}
