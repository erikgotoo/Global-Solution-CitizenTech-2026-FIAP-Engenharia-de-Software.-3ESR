using Zeladoria.Core.Domain;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Tests.Fakes;

internal sealed class FakeHistoricoStatusRepository : IHistoricoStatusRepository
{
    private readonly List<HistoricoStatus> _itens = [];
    private int _proximoId = 1;

    public Task AdicionarAsync(HistoricoStatus historico, CancellationToken ct = default)
    {
        FakeUsuarioRepository.DefinirId(historico, _proximoId++);
        _itens.Add(historico);
        return Task.CompletedTask;
    }

    public Task<HistoricoStatus?> ObterUltimoPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default) =>
        Task.FromResult(_itens
            .Where(h => h.OcorrenciaId == ocorrenciaId)
            .OrderByDescending(h => h.AlteradoEm)
            .ThenByDescending(h => h.Id)
            .FirstOrDefault());

    public Task<IReadOnlyList<HistoricoStatus>> ListarPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default)
    {
        IReadOnlyList<HistoricoStatus> lista = _itens
            .Where(h => h.OcorrenciaId == ocorrenciaId)
            .OrderBy(h => h.AlteradoEm)
            .ThenBy(h => h.Id)
            .ToList();
        return Task.FromResult(lista);
    }

    public Task<IReadOnlyList<HistoricoStatus>> ListarAsync(int? agenteId, StatusOcorrencia? statusNovo, CancellationToken ct = default)
    {
        IReadOnlyList<HistoricoStatus> lista = _itens
            .Where(h => !agenteId.HasValue || h.AgenteId == agenteId.Value)
            .Where(h => !statusNovo.HasValue || h.StatusNovo == statusNovo.Value)
            .OrderByDescending(h => h.Id)
            .ToList();
        return Task.FromResult(lista);
    }

    public Task<bool> UsuarioPossuiHistoricoAsync(int usuarioId, CancellationToken ct = default) =>
        Task.FromResult(_itens.Any(h => h.AgenteId == usuarioId));
}
