using System;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.Domain.Entities
{
    public class ContaCorrente
    {
        // 1. Unificado para usar o IdConta do seu banco real de testes
        public Guid IdConta { get; private set; }
        public string Nome { get; private set; }
        public string Documento { get; private set; } // Adicionado!
        public decimal SaldoAtual { get; private set; } // Adicionado!
        public DateTime DataCriacao { get; private set; } // Adicionado!

        // Mantidos os campos do seu modelo de segurança
        public int Numero { get; private set; }
        public bool Ativo { get; private set; }
        public string Senha { get; private set; }
        public string Salt { get; private set; }

        // Construtor principal ajustado para receber os dados do banco real + senha
        public ContaCorrente(string nome, string documento, decimal saldoInicial, string senhaPura)
        {
            IdConta = Guid.NewGuid();
            Nome = nome;
            Documento = documento;
            SaldoAtual = saldoInicial;
            DataCriacao = DateTime.Now;

            Numero = new Random().Next(1000, 9999); // Gera um número de conta aleatório
            Ativo = true;

            // Geramos o Salt e Hash perfeitamente como você codificou
            Salt = Guid.NewGuid().ToString().Substring(0, 8);
            Senha = GerarHash(senhaPura, Salt);
        }

        // Construtor vazio necessário para o Dapper reconstruir o objeto do banco
        protected ContaCorrente() { }

        private string GerarHash(string senha, string salt)
        {
            using var sha256 = SHA256.Create();
            var combinado = senha + salt;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinado));
            return Convert.ToBase64String(bytes);
        }

        public bool ValidarSenha(string senhaPura)
        {
            var hashGerado = GerarHash(senhaPura, this.Salt);
            return this.Senha == hashGerado;
        }

        // Método de negócio essencial para o seu Handler de Transferência funcionar!
        public void Debitar(decimal valor)
        {
            if (valor <= 0) throw new ArgumentException("O valor do débito deve ser maior que zero.");
            if (SaldoAtual < valor) throw new InvalidOperationException("Saldo insuficiente.");

            SaldoAtual -= valor;
        }
    }
}