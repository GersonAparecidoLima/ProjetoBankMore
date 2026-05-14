using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Domain.Entities
{
    public class Movimento
    {
        public Guid IdMovimento { get; private set; }
        public Guid IdContaCorrente { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public char TipoMovimento { get; private set; } // 'C' ou 'D'
        public decimal Valor { get; private set; }

        public Movimento(Guid idContaCorrente, char tipo, decimal valor)
        {
            IdMovimento = Guid.NewGuid();
            IdContaCorrente = idContaCorrente;
            DataMovimento = DateTime.Now;
            TipoMovimento = tipo;
            Valor = valor;
        }

        // Construtor para o Dapper
        protected Movimento() { }
    }
}
