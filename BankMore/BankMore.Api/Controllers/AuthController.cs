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
                var numeroConta = await _mediator.Send(command);
                return Ok(new { NumeroConta = numeroConta });
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