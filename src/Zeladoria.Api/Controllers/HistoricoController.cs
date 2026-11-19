using Microsoft.AspNetCore.Mvc;
using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Services;

namespace Zeladoria.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class HistoricoController(IHistoricoService service) : ControllerBase
{
    /// <summary>Agente registra uma mudança de status da ocorrência (o status só pode avançar).</summary>
    [HttpPost("api/ocorrencias/{ocorrenciaId:int}/historico")]
    [ProducesResponseType(typeof(HistoricoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HistoricoResponse>> Registrar(int ocorrenciaId, [FromBody] RegistrarStatusRequest request, CancellationToken ct)
    {
        var registro = await service.RegistrarAsync(ocorrenciaId, request, ct);
        return Created($"/api/ocorrencias/{ocorrenciaId}/historico", registro);
    }

    /// <summary>Lista o histórico de status de uma ocorrência, do mais antigo para o mais recente.</summary>
    [HttpGet("api/ocorrencias/{ocorrenciaId:int}/historico")]
    [ProducesResponseType(typeof(IReadOnlyList<HistoricoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<HistoricoResponse>>> ListarPorOcorrencia(int ocorrenciaId, CancellationToken ct) =>
        Ok(await service.ListarPorOcorrenciaAsync(ocorrenciaId, ct));

    /// <summary>Informa o status atual da ocorrência (Aberta quando ainda não há mudanças).</summary>
    [HttpGet("api/ocorrencias/{ocorrenciaId:int}/status-atual")]
    [ProducesResponseType(typeof(StatusAtualResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StatusAtualResponse>> StatusAtual(int ocorrenciaId, CancellationToken ct) =>
        Ok(await service.ObterStatusAtualAsync(ocorrenciaId, ct));

    /// <summary>Lista todas as mudanças de status, com filtros opcionais por agente e por novo status.</summary>
    [HttpGet("api/historico")]
    [ProducesResponseType(typeof(IReadOnlyList<HistoricoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<HistoricoResponse>>> Listar(
        [FromQuery] int? agenteId, [FromQuery] StatusOcorrencia? status, CancellationToken ct) =>
        Ok(await service.ListarAsync(agenteId, status, ct));
}
