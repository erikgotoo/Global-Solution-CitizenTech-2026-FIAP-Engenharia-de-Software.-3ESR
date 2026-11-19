using Zeladoria.Core.Domain;
using Zeladoria.Core.Exceptions;

namespace Zeladoria.Tests;

/// <summary>Testes da entidade Usuario. Relacionados aos casos TC-01 a TC-06 do documento de QA.</summary>
public class UsuarioTests
{
    [Fact]
    public void Construtor_DadosValidos_NormalizaNomeEEmail()
    {
        var usuario = new Usuario("  Maria Souza ", "  MARIA@Email.com ", PerfilUsuario.Cidadao);

        Assert.Equal("Maria Souza", usuario.Nome);
        Assert.Equal("maria@email.com", usuario.Email);
        Assert.Equal(PerfilUsuario.Cidadao, usuario.Perfil);
        Assert.False(usuario.EhAgente);
    }

    [Fact]
    public void Construtor_PerfilAgente_EhAgenteVerdadeiro()
    {
        var usuario = new Usuario("Carlos Lima", "carlos@prefeitura.test", PerfilUsuario.Agente);

        Assert.True(usuario.EhAgente);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Al")]
    public void Construtor_NomeInvalido_LancaDomainValidationException(string nome)
    {
        Assert.Throws<DomainValidationException>(() => new Usuario(nome, "a@b.com", PerfilUsuario.Cidadao));
    }

    [Theory]
    [InlineData("")]
    [InlineData("maria.email.com")]
    [InlineData("maria@")]
    public void Construtor_EmailInvalido_LancaDomainValidationException(string email)
    {
        Assert.Throws<DomainValidationException>(() => new Usuario("Maria Souza", email, PerfilUsuario.Cidadao));
    }

    [Fact]
    public void Construtor_PerfilInexistente_LancaDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() => new Usuario("Maria Souza", "maria@email.com", (PerfilUsuario)99));
    }
}
