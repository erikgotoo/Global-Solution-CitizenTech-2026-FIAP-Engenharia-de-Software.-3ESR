using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Exceptions;
using Zeladoria.Core.Services;
using Zeladoria.Tests.Fakes;

namespace Zeladoria.Tests;

/// <summary>Testes do UsuarioService. Relacionados aos casos TC-01 a TC-08.</summary>
public class UsuarioServiceTests
{
    private readonly FakeUsuarioRepository _usuarios = new();
    private readonly FakeHistoricoStatusRepository _historico = new();
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _service = new UsuarioService(_usuarios, _historico);
    }

    private static UsuarioRequest NovoRequest(string nome = "Maria Souza", string email = "maria@email.com", PerfilUsuario perfil = PerfilUsuario.Cidadao) =>
        new() { Nome = nome, Email = email, Perfil = perfil };

    [Fact]
    public async Task Criar_DadosValidos_RetornaUsuarioComIdEPerfil()
    {
        var resposta = await _service.CriarAsync(NovoRequest());

        Assert.True(resposta.Id > 0);
        Assert.Equal("Maria Souza", resposta.Nome);
        Assert.Equal(PerfilUsuario.Cidadao, resposta.Perfil);
    }

    [Fact]
    public async Task Criar_EmailDuplicado_LancaConflictException()
    {
        await _service.CriarAsync(NovoRequest());

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.CriarAsync(NovoRequest(nome: "Outra Maria", email: "MARIA@email.com")));
    }

    [Fact]
    public async Task Criar_SemPerfil_LancaDomainValidationException()
    {
        var request = new UsuarioRequest { Nome = "Maria Souza", Email = "maria@email.com", Perfil = null };

        await Assert.ThrowsAsync<DomainValidationException>(() => _service.CriarAsync(request));
    }

    [Fact]
    public async Task Obter_UsuarioInexistente_LancaNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterAsync(999));
    }

    [Fact]
    public async Task Listar_FiltraPorPerfil()
    {
        await _service.CriarAsync(NovoRequest("Maria Souza", "maria@email.com", PerfilUsuario.Cidadao));
        await _service.CriarAsync(NovoRequest("Carlos Lima", "carlos@prefeitura.test", PerfilUsuario.Agente));

        var agentes = await _service.ListarAsync(PerfilUsuario.Agente);
        var todos = await _service.ListarAsync(null);

        Assert.Single(agentes);
        Assert.Equal("Carlos Lima", agentes[0].Nome);
        Assert.Equal(2, todos.Count);
    }

    [Fact]
    public async Task Atualizar_EmailDeOutroUsuario_LancaConflictException()
    {
        await _service.CriarAsync(NovoRequest("Maria Souza", "maria@email.com"));
        var segundo = await _service.CriarAsync(NovoRequest("Carlos Lima", "carlos@prefeitura.test", PerfilUsuario.Agente));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.AtualizarAsync(segundo.Id, NovoRequest("Carlos Lima", "maria@email.com", PerfilUsuario.Agente)));
    }

    [Fact]
    public async Task Login_EmailCadastrado_RetornaUsuario()
    {
        await _service.CriarAsync(NovoRequest());

        var resposta = await _service.LoginAsync(new LoginRequest { Email = "  MARIA@email.com " });

        Assert.Equal("maria@email.com", resposta.Email);
        Assert.Equal(PerfilUsuario.Cidadao, resposta.Perfil);
    }

    [Fact]
    public async Task Login_EmailDesconhecido_LancaAutenticacaoException()
    {
        await Assert.ThrowsAsync<AutenticacaoException>(() =>
            _service.LoginAsync(new LoginRequest { Email = "ninguem@email.com" }));
    }

    [Fact]
    public async Task Remover_UsuarioSemHistorico_Remove()
    {
        var criado = await _service.CriarAsync(NovoRequest());

        await _service.RemoverAsync(criado.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterAsync(criado.Id));
    }

    [Fact]
    public async Task Remover_AgenteComHistorico_LancaConflictException()
    {
        var agente = await _service.CriarAsync(NovoRequest("Carlos Lima", "carlos@prefeitura.test", PerfilUsuario.Agente));
        await _historico.AdicionarAsync(new HistoricoStatus(1, StatusOcorrencia.Aberta, StatusOcorrencia.EmAnalise, agente.Id, null));

        await Assert.ThrowsAsync<ConflictException>(() => _service.RemoverAsync(agente.Id));
    }
}
