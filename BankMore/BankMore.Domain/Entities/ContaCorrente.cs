using System.Security.Cryptography;
using System.Text;

namespace BankMore.Domain.Entities
{
    public class ContaCorrente
    {
        // Usamos IdContaCorrente para bater com o SQL Server
        public Guid IdContaCorrente { get; private set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; }
        public bool Ativo { get; private set; }
        public string Senha { get; private set; } // O Dapper mapeará para a coluna Senha
        public string Salt { get; private set; }

        // Construtor principal usado pelo Handler
        public ContaCorrente(int numero, string nome, string senhaPura)
        {
            IdContaCorrente = Guid.NewGuid();
            Numero = numero;
            Nome = nome;
            Ativo = true;

            // Geramos o Salt internamente para garantir que ele seja único
            Salt = Guid.NewGuid().ToString().Substring(0, 8);

            // Aplicamos o Hash
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

        // Método para validar a senha durante o Login (será útil depois!)
        public bool ValidarSenha(string senhaPura)
        {
            var hashGerado = GerarHash(senhaPura, this.Salt);
            return this.Senha == hashGerado;
        }
    }
}