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
    public class EfetuarSaqueCommandHandlerTests
    {
        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Saldo_For_Insuficiente()
        {
            // 1. Arrange (Configuração do Cenário)
            var idContaFake = Guid.NewGuid();
            var valorSaque = 500.00m;
            string observacaoFake = "Saque via Teste Automatizado";

            var command = new EfetuarSaqueCommand(idContaFake, valorSaque, observacaoFake);

            var mockRepository = new Mock<IMovimentoRepository>();

            mockRepository
                .Setup(repo => repo.ObterSaldoAsync(idContaFake))
                .ReturnsAsync(100.00m); // Saldo de apenas R$ 100

            var handler = new EfetuarSaqueCommandHandler(mockRepository.Object);

            // 2. Act & 3. Assert (Ação e Validação conjugados)
            // Aqui testamos se o Handler lança a exceção esperada ao tentar sacar R$ 500 tendo R$ 100
            var excecao = await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(command, CancellationToken.None)
            );

            // Valida se a mensagem de erro contém o texto que você escreveu no Handler
            Assert.Contains("Saldo insuficiente", excecao.Message);

            // Garantia Sênior: O repositório NUNCA deve ter chamado o método de inserir débito
            mockRepository.Verify(repo =>
                repo.InserirMovimentoAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<char>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_Realizar_O_Saque_Com_Sucesso_Quando_Saldo_For_Suficiente()
        {
            // 1. Arrange (Configuração do Cenário)
            var idContaFake = Guid.NewGuid();
            var valorSaque = 150.00m; // Tentando sacar R$ 150
            string observacaoFake = "Saque de teste com sucesso";

            var command = new EfetuarSaqueCommand(idContaFake, valorSaque, observacaoFake);

            var mockRepository = new Mock<IMovimentoRepository>();

            // Forçamos o mock a dizer que o cliente TEM R$ 1000,00 de saldo real
            mockRepository
                .Setup(repo => repo.ObterSaldoAsync(idContaFake))
                .ReturnsAsync(1000.00m);

            var handler = new EfetuarSaqueCommandHandler(mockRepository.Object);

            // 2. Act (Ação)
            // Chamamos o Handler. Como ele tem saldo, não deve estourar nenhuma exceção
            var resultado = await handler.Handle(command, CancellationToken.None);

            // 3. Assert (Validação e Garantia Sênior)
            // Validamos que o MediatR retornou a unidade padrão (Unit.Value) indicando sucesso
            Assert.Equal(Unit.Value, resultado);

            // A MÁGICA DO MOQ: Verificamos se o Handler REALMENTE chamou o banco 
            // para salvar o movimento de Débito ('D') com o valor exato de R$ 150
            mockRepository.Verify(repo =>
                repo.InserirMovimentoAsync(idContaFake, valorSaque, 'D'),
                Times.Once); // Deve ser chamado exatamente UMA vez!
        }


    }
}