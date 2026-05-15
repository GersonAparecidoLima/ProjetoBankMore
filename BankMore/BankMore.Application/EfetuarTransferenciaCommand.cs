using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Application
{
    public record EfetuarTransferenciaCommand(
      Guid IdOrigem,
      Guid IdDestino,
      decimal Valor,
      //Guid ChaveIdempotencia // Requisito do time de Crédito!
      string ChaveIdempotencia // <-- Certifique-se de que aqui está como string!
  ) : IRequest<Unit>;
}
