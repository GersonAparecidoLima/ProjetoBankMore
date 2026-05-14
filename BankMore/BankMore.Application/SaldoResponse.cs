using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Application
{
    public record SaldoResponse(Guid IdConta, string Nome, decimal Saldo);
}
