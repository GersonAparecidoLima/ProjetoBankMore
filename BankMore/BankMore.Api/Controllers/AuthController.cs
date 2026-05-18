using BankMore.Application.Accounts.Commands;
using BankMore.Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BankMore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastrarContaCommand command)
        {
            try
            {
                // CORRIGIDO: O retorno agora é o objeto 'resultado' completo (GUID + Número)
                var resultado = await _mediator.Send(command);

                // CORRIGIDO: Retorna o resultado direto, sem precisar envelopar em objeto anônimo
                return Ok(resultado);
            }
            catch (ArgumentException ex) when (ex.ParamName == "INVALID_DOCUMENT")
            {
                return BadRequest(new ErrorResponse(ex.Message, "INVALID_DOCUMENT"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EfetuarLoginCommand command)
        {
            try
            {
                var token = await _mediator.Send(command);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ErrorResponse("Usuário ou senha incorretos.", "USER_UNAUTHORIZED"));
            }
        }
    }
}