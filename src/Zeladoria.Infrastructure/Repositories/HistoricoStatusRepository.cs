using Microsoft.EntityFrameworkCore;
using Zeladoria.Core.Domain;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Infrastructure.Repositories;

public class HistoricoStatusRepository(ZeladoriaDbContext db) : IHistoricoStatusRepository
{
    public async Task AdicionarAsync(HistoricoStatus historico, CancellationToken ct = default)
    {
        await db.Historicos.AddAsync(historico, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task<HistoricoStatus?> ObterUltimoPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default) =>
        await db.Historicos
            .AsNoTracking()
            .Where(h => h.OcorrenciaId == ocorrenciaId)
            .OrderByDescending(h => h.AlteradoEm)
            .ThenByDescending(h => h.Id)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<HistoricoStatus>> ListarPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default) =>
        await db.Historicos
            .AsNoTracking()
            .Include(h => h.Agente)
            .Where(h => h.OcorrenciaId == ocorrenciaId)
            .OrderBy(h => h.AlteradoEm)
            .ThenBy(h => h.Id)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<HistoricoStatus>> ListarAsync(int? agenteId, StatusOcorrencia? statusNovo, CancellationToken ct = default)
    {
        var consulta = db.Historicos.AsNoTracking().Include(h => h.Agente).AsQueryable();

        if (agenteId.HasValue)
            consulta = consulta.Where(h => h.AgenteId == agenteId.Value);

        if (statusNovo.HasValue)
            consulta = consulta.Where(h => h.StatusNovo == statusNovo.Value);

        return await consulta.OrderByDescending(h => h.AlteradoEm).ThenByDescending(h => h.Id).ToListAsync(ct);
    }

    public async Task<bool> UsuarioPossuiHistoricoAsync(int usuarioId, CancellationToken ct = default) =>
        await db.Historicos.AnyAsync(h => h.AgenteId == usuarioId, ct);
}
