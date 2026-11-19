using Zeladoria.Core.Domain;
using Zeladoria.Core.Exceptions;

namespace Zeladoria.Tests;

/// <summary>Testes da regra "o status só avança". Relacionados aos casos TC-18 a TC-24.</summary>
public class HistoricoStatusTests
{
    [Theory]
    [InlineData(StatusOcorrencia.Aberta, StatusOcorrencia.EmAnalise, true)]
    [InlineData(StatusOcorrencia.EmAnalise, StatusOcorrencia.EmAndamento, true)]
    [InlineData(StatusOcorrencia.EmAndamento, StatusOcorrencia.Resolvida, true)]
    [InlineData(StatusOcorrencia.Aberta, StatusOcorrencia.Resolvida, true)]
    [InlineData(StatusOcorrencia.Resolvida, StatusOcorrencia.Aberta, false)]
    [InlineData(StatusOcorrencia.EmAndamento, StatusOcorrencia.EmAnalise, false)]
    [InlineData(StatusOcorrencia.EmAnalise, StatusOcorrencia.EmAnalise, false)]
    public void TransicaoValida_SoPermiteAvancar(StatusOcorrencia anterior, StatusOcorrencia novo, bool esperado)
    {
        Assert.Equal(esperado, HistoricoStatus.TransicaoValida(anterior, novo));
    }

    [Fact]
    public void Construtor_TransicaoValida_CriaRegistro()
    {
        var registro = new HistoricoStatus(10, StatusOcorrencia.Aberta, StatusOcorrencia.EmAnalise, 1, "  Equipe a caminho  ");

        Assert.Equal(10, registro.OcorrenciaId);
        Assert.Equal(StatusOcorrencia.EmAnalise, registro.StatusNovo);
        Assert.Equal("Equipe a caminho", registro.Observacao);
    }

    [Fact]
    public void Construtor_StatusRetrocedendo_LancaConflictException()
    {
        Assert.Throws<ConflictException>(() =>
            new HistoricoStatus(10, StatusOcorrencia.Resolvida, StatusOcorrencia.Aberta, 1, null));
    }

    [Fact]
    public void Construtor_ObservacaoComMaisDe500Caracteres_LancaDomainValidationException()
    {
        var observacao = new string('x', HistoricoStatus.ObservacaoMaxima + 1);

        Assert.Throws<DomainValidationException>(() =>
            new HistoricoStatus(10, StatusOcorrencia.Aberta, StatusOcorrencia.EmAnalise, 1, observacao));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(10, 0)]
    public void Construtor_IdsInvalidos_LancaDomainValidationException(int ocorrenciaId, int agenteId)
    {
        Assert.Throws<DomainValidationException>(() =>
            new HistoricoStatus(ocorrenciaId, StatusOcorrencia.Aberta, StatusOcorrencia.EmAnalise, agenteId, null));
    }
}
