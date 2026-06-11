using BankMore.Application;
using BankMore.Application.Accounts.Commands;
using BankMore.Api.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController(IMediator mediator) : ControllerBase
{
    // Público: qualquer pessoa pode abrir uma conta
    [HttpPost]
    public async Task<IActionResult> CriarConta([FromBody] CriarContaCommand command)
    {
        var id = await mediator.Send(command);
        return CreatedAtAction(nameof(CriarConta), new { id }, new { id, mensagem = "Conta criada com sucesso!" });
    }

    // Público: fluxo de cadastro via CPF/senha
    [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarConta([FromBody] CriarContaCommand command)
    {
        var idContaCriada = await mediator.Send(command);
        return CreatedAtAction(nameof(CadastrarConta), new { id = idContaCriada }, new { mensagem = "Conta criada com sucesso!" });
    }

    // Protegido: somente o titular autenticado pode consultar o saldo
    [Authorize]
    [HttpGet("{id}/saldo")]
    public async Task<IActionResult> ObterSaldo(Guid id)
    {
        var resultado = await mediator.Send(new ObterSaldoQuery(id));

        if (resultado == null)
            return NotFound(new { mensagem = "Conta não encontrada." });

        return Ok(resultado);
    }

    // Protegido: somente o titular autenticado pode consultar o extrato
    [Authorize]
    [HttpGet("{id}/extrato")]
    public async Task<IActionResult> ObterExtrato(Guid id)
    {
        var extrato = await mediator.Send(new ObterExtratoQuery(id));

        if (extrato == null)
            return NotFound(new { mensagem = "Conta não encontrada." });

        return Ok(extrato);
    }

    // Protegido: depósito exige autenticação para rastreabilidade
    [Authorize]
    [HttpPost("deposito")]
    public async Task<IActionResult> Deposito([FromBody] EfetuarDepositoCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok(new { mensagem = "Depósito realizado com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // Protegido: saque movimenta saldo — requer token válido
    [Authorize]
    [HttpPost("saque")]
    [TypeFilter(typeof(IdempotencyFilter))]
    public async Task<IActionResult> Saque([FromBody] EfetuarSaqueCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok(new { mensagem = "Saque realizado com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    // Protegido: transferência movimenta saldo entre contas — requer token válido
    [Authorize]
    [HttpPost("transferir")]
    [TypeFilter(typeof(IdempotencyFilter))]
    public async Task<IActionResult> Transferir([FromBody] EfetuarTransferenciaCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok(new { mensagem = "Transferência realizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
