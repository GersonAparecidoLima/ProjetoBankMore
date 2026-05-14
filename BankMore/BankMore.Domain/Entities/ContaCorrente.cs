using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Domain.Entities;

public class ContaCorrente
{
    //public Guid IdContaCorrente { get; private set; }
    public Guid IdContaCorrente { get; set; } // Ajuste para o nome correto

    public int Numero { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public bool Ativo { get; private set; }

    public string Senha { get; private set; } = string.Empty;

    public string Salt { get; private set; } = string.Empty;

    public ContaCorrente(
        int numero,
        string nome,
        string senhaHash,
        string salt)
    {
        IdContaCorrente = Guid.NewGuid();

        Numero = numero;
        Nome = nome;

        Ativo = true;

        Senha = senhaHash;
        Salt = salt;
    }

    // Necessário para Dapper
    protected ContaCorrente()
    {
    }
}
