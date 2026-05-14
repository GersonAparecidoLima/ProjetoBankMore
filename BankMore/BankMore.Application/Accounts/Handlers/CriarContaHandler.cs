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
                // Dica: No futuro, usaremos um custom exception para o Middleware pegar o 'INVALID_DOCUMENT'
                throw new ArgumentException("CPF inválido", "INVALID_DOCUMENT");
            }

            // --- MOVA A GERAÇÃO PARA CÁ ---
            // Assim, cada execução do Handle gera um Salt novo e exclusivo
            string novoSalt = Guid.NewGuid().ToString().Substring(0, 8);

            // 3. Criar a Entidade
            // O construtor da Entidade deve ser responsável por gerar o Salt e o ID único
            var novaConta = new ContaCorrente(
                request.Numero,
                request.Nome,
                request.Senha, // Aqui você passaria a senha (idealmente já hasheada)
                novoSalt       // O argumento que estava faltando!
            );
            // 4. Persistir no Banco via Repositório (Dapper)
            return await _repository.AdicionarAsync(novaConta);
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
