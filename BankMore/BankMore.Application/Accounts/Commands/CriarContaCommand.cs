using MediatR; // <--- Adicione esta linha
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Application.Accounts.Commands
{
    // O Command define o que entra e o que sai (neste caso, retorna o Guid da conta criada)
    public record CriarContaCommand(
        int Numero,
        string Nome,
        string Senha,
        string CPF // CPF é usado para validar, mas não salvaremos nesta tabela por segurança
    ) : IRequest<Guid>;
}
