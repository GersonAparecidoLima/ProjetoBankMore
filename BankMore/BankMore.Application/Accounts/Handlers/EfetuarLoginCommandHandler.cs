using BankMore.Application.Accounts.Commands;
using BankMore.Application.Interfaces;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Application.Accounts.Handlers;

public class EfetuarLoginCommandHandler : IRequestHandler<EfetuarLoginCommand, string>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public EfetuarLoginCommandHandler(IContaCorrenteRepository contaRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _contaRepository = contaRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Handle(EfetuarLoginCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorNumeroAsync(request.Identificador);

        if (conta == null)
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

        // Regra de negócio: validação da senha via método do domínio
        if (!conta.ValidarSenha(request.Senha))
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED");

        return _jwtTokenGenerator.GerarToken(conta.IdConta, conta.Numero.ToString());
    }
}
