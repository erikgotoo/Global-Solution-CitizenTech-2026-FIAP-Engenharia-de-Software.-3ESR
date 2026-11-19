namespace Zeladoria.Core.Domain;

/// <summary>
/// Status da ocorrência (vocabulário comum do projeto).
/// A ordem numérica representa o andamento: só é permitido avançar.
/// </summary>
public enum StatusOcorrencia
{
    Aberta = 1,
    EmAnalise = 2,
    EmAndamento = 3,
    Resolvida = 4
}
