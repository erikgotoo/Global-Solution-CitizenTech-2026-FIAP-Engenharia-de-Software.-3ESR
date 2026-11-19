using System.ComponentModel.DataAnnotations;
using Zeladoria.Core.Domain;

namespace Zeladoria.Core.Dtos;

public class RegistrarStatusRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "O agenteId deve ser maior que zero.")]
    public int AgenteId { get; set; }

    [Required(ErrorMessage = "O novo status é obrigatório.")]
    [EnumDataType(typeof(StatusOcorrencia), ErrorMessage = "Status inválido. Use Aberta, EmAnalise, EmAndamento ou Resolvida.")]
    public StatusOcorrencia? NovoStatus { get; set; }

    [StringLength(HistoricoStatus.ObservacaoMaxima, ErrorMessage = "A observação deve ter no máximo {1} caracteres.")]
    public string? Observacao { get; set; }
}

public record HistoricoResponse(
    int Id,
    int OcorrenciaId,
    StatusOcorrencia StatusAnterior,
    StatusOcorrencia StatusNovo,
    int AgenteId,
    string AgenteNome,
    string? Observacao,
    DateTime AlteradoEm);

public record StatusAtualResponse(int OcorrenciaId, StatusOcorrencia StatusAtual, DateTime? AtualizadoEm);
