using Zeladoria.Core.Domain;

namespace Zeladoria.Core.Dtos;

public static class Mapeamentos
{
    public static UsuarioResponse ToResponse(this Usuario u) =>
        new(u.Id, u.Nome, u.Email, u.Perfil, u.CriadoEm);

    public static HistoricoResponse ToResponse(this HistoricoStatus h, string? nomeAgente = null) =>
        new(h.Id, h.OcorrenciaId, h.StatusAnterior, h.StatusNovo, h.AgenteId,
            nomeAgente ?? h.Agente?.Nome ?? string.Empty, h.Observacao, h.AlteradoEm);
}
