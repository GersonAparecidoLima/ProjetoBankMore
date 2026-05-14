using System;
using System.Collections.Generic;

namespace BankMore.Application
{
    // DTO para cada linha do extrato
    public record MovimentacaoDto(Guid IdMovimento, DateTime DataMovimento, string Tipo, decimal Valor);

    // DTO principal do Extrato
    public record ExtratoResponse(Guid IdConta, string Nome, decimal SaldoAtual, List<MovimentacaoDto> Transacoes);
}