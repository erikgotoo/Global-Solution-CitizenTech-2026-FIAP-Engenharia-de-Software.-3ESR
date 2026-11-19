using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Exceptions;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Core.Services;

public class HistoricoService(IHistoricoStatusRepository historico, IUsuarioRepository usuarios) : IHistoricoService
{
    public async Task<HistoricoResponse> RegistrarAsync(int ocorrenciaId, RegistrarStatusRequest request, CancellationToken ct = default)
    {
        ValidarOcorrencia(ocorrenciaId);
        var novoStatus = request.NovoStatus ?? throw new DomainValidationException("O novo status é obrigatório.");

        var agente = await usuarios.ObterPorIdAsync(request.AgenteId, ct)
            ?? throw new NotFoundException($"Agente {request.AgenteId} não encontrado.");

        if (!agente.EhAgente)
            throw new ForbiddenException("Somente usuários com perfil Agente podem atualizar o status de uma ocorrência.");

        var ultimo = await historico.ObterUltimoPorOcorrenciaAsync(ocorrenciaId, ct);
        var statusAnterior = ultimo?.StatusNovo ?? StatusOcorrencia.Aberta;

        // A regra "o status só avança" é aplicada pela própria entidade.
        var registro = new HistoricoStatus(ocorrenciaId, statusAnterior, novoStatus, agente.Id, request.Observacao);
        await historico.AdicionarAsync(registro, ct);

        return registro.ToResponse(agente.Nome);
    }

    public async Task<IReadOnlyList<HistoricoResponse>> ListarPorOcorrenciaAsync(int ocorrenciaId, CancellationToken ct = default)
    {
        ValidarOcorrencia(ocorrenciaId);
        var lista = await historico.ListarPorOcorrenciaAsync(ocorrenciaId, ct);
        return lista.Select(h => h.ToResponse()).ToList();
    }

    public async Task<StatusAtualResponse> ObterStatusAtualAsync(int ocorrenciaId, CancellationToken ct = default)
    {
        ValidarOcorrencia(ocorrenciaId);
        var ultimo = await historico.ObterUltimoPorOcorrenciaAsync(ocorrenciaId, ct);
        return new StatusAtualResponse(ocorrenciaId, ultimo?.StatusNovo ?? StatusOcorrencia.Aberta, ultimo?.AlteradoEm);
    }

    public async Task<IReadOnlyList<HistoricoResponse>> ListarAsync(int? agenteId, StatusOcorrencia? statusNovo, CancellationToken ct = default)
    {
        var lista = await historico.ListarAsync(agenteId, statusNovo, ct);
        return lista.Select(h => h.ToResponse()).ToList();
    }

    private static void ValidarOcorrencia(int ocorrenciaId)
    {
        if (ocorrenciaId <= 0)
            throw new DomainValidationException("O id da ocorrência deve ser maior que zero.");
    }
}
