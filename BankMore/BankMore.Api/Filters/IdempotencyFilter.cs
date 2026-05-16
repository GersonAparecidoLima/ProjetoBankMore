using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using BankMore.Infrastructure.Data;

namespace BankMore.Api.Filters
{
    public class IdempotencyFilter : IAsyncActionFilter
    {
        private readonly DbSession _session;

        public IdempotencyFilter(DbSession session)
        {
            _session = session;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Procuramos a chave dentro do Command
            string? chaveIdempotencia = null;
            var command = context.ActionArguments.Values.FirstOrDefault();
            if (command != null)
            {
                var propriedadeChave = command.GetType().GetProperty("ChaveIdempotencia");
                chaveIdempotencia = propriedadeChave?.GetValue(command)?.ToString();
            }

            if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            {
                await next();
                return;
            }

            // 2. Verifica se essa chave já existe na tabela [Idempotencia]
            var sqlCheck = "SELECT Resultado FROM Idempotencia WHERE ChaveIdempotencia = @Chave";
            var resultadoExistente = await _session.Connection.QueryFirstOrDefaultAsync<string>(sqlCheck, new { Chave = chaveIdempotencia });

            if (resultadoExistente != null)
            {
                // PARA CORTAR O FLUXO DEFINITIVAMENTE:
                // Atribuir o resultado e dar return SEM chamar o 'await next()' 
                // força o ASP.NET Core a fazer o curto-circuito (short-circuit) da requisição.
                context.Result = new ContentResult
                {
                    Content = resultadoExistente,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return; // Retorna direto daqui, impedindo o pipeline de continuar!
            }

            // 3. Se for inédita, chama o próximo passo do pipeline (Controller / Handler)
            var executedContext = await next();

            // 4. Se deu tudo certo, salva na tabela de Idempotência
            if (executedContext.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var respostaComCache = new
                {
                    dados = objectResult.Value,
                    idempotencia = "Requisição já processada anteriormente (Retornado via cache)"
                };

                var resultadoJson = JsonSerializer.Serialize(respostaComCache);
                var jsonRequisicao = JsonSerializer.Serialize(command);

                var sqlInsert = @"INSERT INTO Idempotencia (ChaveIdempotencia, Requisicao, Resultado, DataCriacao)
                          VALUES (@Chave, @Req, @Res, @Data);";

                await _session.Connection.ExecuteAsync(sqlInsert, new
                {
                    Chave = chaveIdempotencia,
                    Req = jsonRequisicao,
                    Res = resultadoJson,
                    Data = DateTime.Now
                });
            }
        }

    }
}