using BankMore.Application.Accounts.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaCommand command)
        {
            try
            {
                var resultado = await _mediator.Send(command);
                return Ok(new { id = resultado, mensagem = "Conta criada com sucesso!" });
            }
            catch (ArgumentException ex) when (ex.ParamName == "INVALID_DOCUMENT")
            {
                return BadRequest(new { erro = ex.Message, codigo = "INVALID_DOCUMENT" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}