using BankMore.Application.Accounts.Commands;
using MediatR;
using BankMore.Domain.Entities; // Onde deve estar a classe ContaCorrente
using BankMore.Infrastructure.Repositories; // Onde deve estar o Repository
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            // 1. Validação de Negócio: Verificar se o número da conta já existe
            var contaExistente = await _repository.ObterPorNumeroAsync(request.Numero);
            if (contaExistente != null)
            {
                throw new Exception("Este número de conta já está em uso.");
            }

            // 2. Validação de Documento (Requisito do teste: INVALID_DOCUMENT)
            if (!ValidarCPF(request.CPF))
            {
                throw new ArgumentException("CPF inválido", "INVALID_DOCUMENT");
            }

            // 3. Criar a Entidade
            // O construtor da ContaCorrente já gera o Salt e o Hash internamente agora!
            var novaConta = new ContaCorrente(
                request.Numero,
                request.Nome,
                request.Senha);

            // 4. Persistir no Banco via Repositório (Dapper)
            await _repository.AdicionarAsync(novaConta);

            // 5. RETORNO OBRIGATÓRIO: Você precisa retornar o ID da conta criada
            return novaConta.IdContaCorrente;
        }

        private bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            // Lógica simplificada: apenas garante que tem 11 dígitos
            var apenasNumeros = new string(cpf.Where(char.IsDigit).ToArray());
            return apenasNumeros.Length == 11;
        }
    }
}
