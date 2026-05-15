using BankMore.Application.Accounts.Commands;
using MediatR;
using BankMore.Domain.Entities;
using BankMore.Infrastructure.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankMore.Application.Accounts.Handlers
{
    public class CriarContaHandler : IRequestHandler<CriarContaCommand, Guid>
    {
        private readonly ContaCorrenteRepository _repository;

        public CriarContaHandler(ContaCorrenteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CriarContaCommand request, CancellationToken cancellationToken)
        {
            // 1. Validação básica de contrato antes de mandar para o banco
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                throw new ArgumentException("O nome é obrigatório para a criação da conta.");
            }

            // 2. Chamar o método correto do repositório passando os 2 argumentos reais: Nome e Senha
            Guid idContaCriada = await _repository.CriarContaBlanchAsync(
                request.Nome,
                request.Senha
            );

            // 3. Retorna o ID (IdContaCorrente) que o repositório acabou de registrar
            return idContaCriada;
        }
    }
}