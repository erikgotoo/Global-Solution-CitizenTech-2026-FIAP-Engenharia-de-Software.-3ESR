using Zeladoria.Core.Exceptions;

namespace Zeladoria.Core.Domain;

/// <summary>Registro de uma mudança de status de uma ocorrência feita por um agente.</summary>
public class HistoricoStatus
{
    public const int ObservacaoMaxima = 500;

    public int Id { get; private set; }
    public int OcorrenciaId { get; private set; }
    public StatusOcorrencia StatusAnterior { get; private set; }
    public StatusOcorrencia StatusNovo { get; private set; }
    public int AgenteId { get; private set; }
    public Usuario? Agente { get; private set; }
    public string? Observacao { get; private set; }
    public DateTime AlteradoEm { get; private set; }

    // Usado pelo EF Core.
    private HistoricoStatus() { }

    public HistoricoStatus(int ocorrenciaId, StatusOcorrencia statusAnterior, StatusOcorrencia statusNovo, int agenteId, string? observacao)
    {
        if (ocorrenciaId <= 0)
            throw new DomainValidationException("O id da ocorrência deve ser maior que zero.");
        if (agenteId <= 0)
            throw new DomainValidationException("O id do agente deve ser maior que zero.");
        if (!Enum.IsDefined(statusAnterior) || !Enum.IsDefined(statusNovo))
            throw new DomainValidationException("Status inválido.");

        var observacaoLimpa = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
        if (observacaoLimpa is { Length: > ObservacaoMaxima })
            throw new DomainValidationException($"A observação deve ter no máximo {ObservacaoMaxima} caracteres.");

        if (!TransicaoValida(statusAnterior, statusNovo))
            throw new ConflictException($"Transição de status inválida: de {statusAnterior} para {statusNovo}. O status só pode avançar.");

        OcorrenciaId = ocorrenciaId;
        StatusAnterior = statusAnterior;
        StatusNovo = statusNovo;
        AgenteId = agenteId;
        Observacao = observacaoLimpa;
        AlteradoEm = DateTime.UtcNow;
    }

    /// <summary>Regra de negócio: o status só pode avançar (Aberta, Em análise, Em andamento, Resolvida).</summary>
    public static bool TransicaoValida(StatusOcorrencia anterior, StatusOcorrencia novo) => novo > anterior;
}
