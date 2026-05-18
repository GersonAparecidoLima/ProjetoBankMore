using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BankMore.Application.Accounts.Commands
{
    // AJUSTADO: Agora indica que o MediatR vai retornar o objeto CadastrarContaResult
    public class CadastrarContaCommand : IRequest<CadastrarContaResult>
    {
        [Required]
        public string Cpf { get; set; }

        [Required]
        public string Senha { get; set; }
    }

    // NOVA CLASSE: Esse é o espelho do JSON que vai aparecer lindo no seu Swagger!
    public class CadastrarContaResult
    {
        public string IdConta { get; set; }
        public string NumeroConta { get; set; }
    }
}