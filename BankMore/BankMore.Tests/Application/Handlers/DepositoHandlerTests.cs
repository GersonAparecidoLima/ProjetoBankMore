using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using BankMore.Application.Accounts.Commands;
using BankMore.Application.Accounts.Handlers;
using BankMore.Domain.Interfaces;
using MediatR;

namespace BankMore.Tests.Application.Handlers
{
    public class DepositoHandlerTests
    {
        [Fact]
        public async Task Deve_Realizar_O_Deposito_Com_Sucesso()
        {
            // 1. Arrange (Configuração do Cenário)
            var idContaFake = Guid.NewGuid();
            var valorDeposito = 300.00m; // Depositando R$ 300

            // AJUSTADO: Usando o nome correto do comando do seu projeto
            var command = new EfetuarDepositoCommand(idContaFake, valorDeposito);

            var mockRepository = new Mock<IMovimentoRepository>();

            var handler = new DepositoHandler(mockRepository.Object);

            // 2. Act (Execução)
            // AJUSTADO: Passando o comando correto para o Handler
            var resultado = await handler.Handle(command, CancellationToken.None);

            // 3. Assert (Validação)
            Assert.Equal(Unit.Value, resultado);

            // Verifica se o Handler REALMENTE mandou o banco salvar um Crédito ('C') de R$ 300
            mockRepository.Verify(repo =>
                repo.InserirMovimentoAsync(idContaFake, valorDeposito, 'C'),
                Times.Once);
        }
    }
}