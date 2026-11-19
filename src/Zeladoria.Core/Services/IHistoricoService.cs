using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;

namespace Zeladoria.Core.Services;

public interface IHistoricoService
{
    Task<HistoricoResponse> RegistrarAsync(int ocorrenciaId, RegistrarStatusRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<HistoricoResponse>> ListarPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default);
    Task<StatusAtualResponse> ObterStatusAtualAsync(int ocorrenciaId, CancellationToken ct = default);
    Task<IReadOnlyList<HistoricoResponse>> ListarAsync(int? agenteId, StatusOcorrencia? statusNovo, CancellationToken ct = default);
}
