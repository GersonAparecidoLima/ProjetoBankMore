using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BankMore.Application.Accounts.Commands
{
    public class EfetuarLoginCommand : IRequest<string> // Retorna o Token JWT
    {
        [Required]
        public string Identificador { get; set; } // Aceita Número ou CPF

        [Required]
        public string Senha { get; set; }
    }
}