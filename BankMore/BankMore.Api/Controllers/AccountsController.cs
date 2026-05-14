using BankMore.Application; // Namespace onde está o seu EfetuarTransferenciaCommand
using BankMore.Application.Accounts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Endpoint de Criação de Conta 
        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(CriarConta), new { id }, new { id, mensagem = "Conta criada com sucesso!" });
        }
                
        [HttpPost("transferir")]
        public async Task<IActionResult> Transferir([FromBody] EfetuarTransferenciaCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { mensagem = "Transferência realizada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost("deposito")]
        public async Task<IActionResult> Deposito([FromBody] EfetuarDepositoCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok(new { mensagem = "Depósito realizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpGet("{id}/saldo")]
        public async Task<IActionResult> ObterSaldo(Guid id)
        {
            var resultado = await _mediator.Send(new ObterSaldoQuery(id));

            if (resultado == null)
                return NotFound(new { mensagem = "Conta não encontrada." });

            return Ok(resultado);
        }

    }
}