using BankMore.Application.Accounts.Commands;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers
{
    public class CadastrarContaCommandHandler : IRequestHandler<CadastrarContaCommand, CadastrarContaResult>
    {
        private readonly IContaCorrenteRepository _repository;

        public CadastrarContaCommandHandler(IContaCorrenteRepository repository) => _repository = repository;

        public async Task<CadastrarContaResult> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
        {
            // Regra de negócio: validação de CPF fica na camada Application
            if (string.IsNullOrWhiteSpace(request.Cpf) || request.Cpf.Length != 11)
                throw new ArgumentException("O CPF fornecido é inválido.", "INVALID_DOCUMENT");

            var (id, numero) = await _repository.CadastrarContaAsync(request.Cpf, request.Senha);

            return new CadastrarContaResult
            {
                IdConta = id.ToString().ToUpper(),
                NumeroConta = numero
            };
        }
    }
}
