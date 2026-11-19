using Zeladoria.Core.Domain;

namespace Zeladoria.Core.Interfaces;

public interface IHistoricoStatusRepository
{
    Task AdicionarAsync(HistoricoStatus historico, CancellationToken ct = default);
    Task<HistoricoStatus?> ObterUltimoPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default);
    Task<IReadOnlyList<HistoricoStatus>> ListarPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default);
    Task<IReadOnlyList<HistoricoStatus>> ListarAsync(int? agenteId, StatusOcorrencia? statusNovo, CancellationToken ct = default);
    Task<bool> UsuarioPossuiHistoricoAsync(int usuarioId, CancellationToken ct = default);
}
