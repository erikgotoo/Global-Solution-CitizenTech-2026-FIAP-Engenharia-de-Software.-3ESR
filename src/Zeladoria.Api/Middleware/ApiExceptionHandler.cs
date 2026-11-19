using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zeladoria.Core.Exceptions;

namespace Zeladoria.Api.Middleware;

/// <summary>Converte as exceções de negócio em respostas HTTP (ProblemDetails) com o status correto.</summary>
public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            DomainValidationException => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            AutenticacaoException => (StatusCodes.Status401Unauthorized, "Não autenticado"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Acesso negado"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false; // erro inesperado: segue para o tratamento padrão (500)

        logger.LogInformation("Regra de negócio rejeitou a requisição: {Mensagem}", exception.Message);

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);
        return true;
    }
}
