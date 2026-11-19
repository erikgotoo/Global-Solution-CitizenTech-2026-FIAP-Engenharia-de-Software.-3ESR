using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Exceptions;
using Zeladoria.Core.Services;
using Zeladoria.Tests.Fakes;

namespace Zeladoria.Tests;

/// <summary>Testes do HistoricoService. Relacionados aos casos TC-14 a TC-28.</summary>
public class HistoricoServiceTests
{
    private readonly FakeUsuarioRepository _usuarios = new();
    private readonly FakeHistoricoStatusRepository _historico = new();
    private readonly HistoricoService _service;

    public HistoricoServiceTests()
    {
        _service = new HistoricoService(_historico, _usuarios);
    }

    private async Task<int> CriarUsuarioAsync(PerfilUsuario perfil, string nome, string email)
    {
        var usuario = new Usuario(nome, email, perfil);
        await _usuarios.AdicionarAsync(usuario);
        return usuario.Id;
    }

    private Task<int> CriarAgenteAsync() => CriarUsuarioAsync(PerfilUsuario.Agente, "Carlos Lima", "carlos@prefeitura.test");

    private static RegistrarStatusRequest Mudanca(int agenteId, StatusOcorrencia novo, string? observacao = null) =>
        new() { AgenteId = agenteId, NovoStatus = novo, Observacao = observacao };

    [Fact]
    public async Task Registrar_PrimeiraMudanca_ParteDeAberta()
    {
        var agenteId = await CriarAgenteAsync();

        var resposta = await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise));

        Assert.Equal(StatusOcorrencia.Aberta, resposta.StatusAnterior);
        Assert.Equal(StatusOcorrencia.EmAnalise, resposta.StatusNovo);
        Assert.Equal("Carlos Lima", resposta.AgenteNome);
    }

    [Fact]
    public async Task Registrar_SegundaMudanca_UsaUltimoStatusComoAnterior()
    {
        var agenteId = await CriarAgenteAsync();
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise));

        var resposta = await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAndamento, "Equipe enviada"));

        Assert.Equal(StatusOcorrencia.EmAnalise, resposta.StatusAnterior);
        Assert.Equal("Equipe enviada", resposta.Observacao);
    }

    [Fact]
    public async Task Registrar_UsuarioCidadao_LancaForbiddenException()
    {
        var cidadaoId = await CriarUsuarioAsync(PerfilUsuario.Cidadao, "Maria Souza", "maria@email.com");

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.RegistrarAsync(10, Mudanca(cidadaoId, StatusOcorrencia.EmAnalise)));
    }

    [Fact]
    public async Task Registrar_AgenteInexistente_LancaNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.RegistrarAsync(10, Mudanca(999, StatusOcorrencia.EmAnalise)));
    }

    [Fact]
    public async Task Registrar_StatusRetrocedendo_LancaConflictException()
    {
        var agenteId = await CriarAgenteAsync();
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.Resolvida));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.Aberta)));
    }

    [Fact]
    public async Task Registrar_MesmoStatus_LancaConflictException()
    {
        var agenteId = await CriarAgenteAsync();
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise)));
    }

    [Fact]
    public async Task Registrar_ObservacaoMuitoLonga_LancaDomainValidationException()
    {
        var agenteId = await CriarAgenteAsync();
        var observacao = new string('x', 501);

        await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise, observacao)));
    }

    [Fact]
    public async Task Registrar_OcorrenciaIdZero_LancaDomainValidationException()
    {
        var agenteId = await CriarAgenteAsync();

        await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.RegistrarAsync(0, Mudanca(agenteId, StatusOcorrencia.EmAnalise)));
    }

    [Fact]
    public async Task ObterStatusAtual_SemHistorico_RetornaAberta()
    {
        var resposta = await _service.ObterStatusAtualAsync(10);

        Assert.Equal(StatusOcorrencia.Aberta, resposta.StatusAtual);
        Assert.Null(resposta.AtualizadoEm);
    }

    [Fact]
    public async Task ObterStatusAtual_AposMudancas_RetornaUltimoStatus()
    {
        var agenteId = await CriarAgenteAsync();
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise));
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAndamento));

        var resposta = await _service.ObterStatusAtualAsync(10);

        Assert.Equal(StatusOcorrencia.EmAndamento, resposta.StatusAtual);
        Assert.NotNull(resposta.AtualizadoEm);
    }

    [Fact]
    public async Task ObterStatusAtual_OcorrenciaIdInvalido_LancaDomainValidationException()
    {
        await Assert.ThrowsAsync<DomainValidationException>(() => _service.ObterStatusAtualAsync(0));
    }

    [Fact]
    public async Task ListarPorOcorrencia_RetornaEmOrdemCronologica()
    {
        var agenteId = await CriarAgenteAsync();
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAnalise));
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.EmAndamento));
        await _service.RegistrarAsync(10, Mudanca(agenteId, StatusOcorrencia.Resolvida));
        await _service.RegistrarAsync(11, Mudanca(agenteId, StatusOcorrencia.EmAnalise));

        var lista = await _service.ListarPorOcorrenciaAsync(10);

        var esperado = new[] { StatusOcorrencia.EmAnalise, StatusOcorrencia.EmAndamento, StatusOcorrencia.Resolvida };
        Assert.Equal(esperado, lista.Select(h => h.StatusNovo).ToArray());
    }

    [Fact]
    public async Task ListarPorOcorrencia_SemMudancas_RetornaListaVazia()
    {
        var lista = await _service.ListarPorOcorrenciaAsync(10);

        Assert.Empty(lista);
    }

    [Fact]
    public async Task Listar_FiltraPorAgenteEStatus()
    {
        var carlos = await CriarAgenteAsync();
        var ana = await CriarUsuarioAsync(PerfilUsuario.Agente, "Ana Prado", "ana@prefeitura.test");
        await _service.RegistrarAsync(10, Mudanca(carlos, StatusOcorrencia.EmAnalise));
        await _service.RegistrarAsync(11, Mudanca(ana, StatusOcorrencia.EmAnalise));
        await _service.RegistrarAsync(11, Mudanca(ana, StatusOcorrencia.Resolvida));

        var doCarlos = await _service.ListarAsync(carlos, null);
        var resolvidas = await _service.ListarAsync(null, StatusOcorrencia.Resolvida);

        Assert.Single(doCarlos);
        Assert.Single(resolvidas);
        Assert.Equal(ana, resolvidas[0].AgenteId);
    }
}
