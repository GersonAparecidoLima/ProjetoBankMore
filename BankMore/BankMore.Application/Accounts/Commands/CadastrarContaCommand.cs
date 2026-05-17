using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BankMore.Application.Accounts.Commands
{
    public class CadastrarContaCommand : IRequest<string> // Retorna o número da conta gerado
    {
        [Required]
        public string Cpf { get; set; }

        [Required]
        public string Senha { get; set; }
    }
}