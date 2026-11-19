using System.Net.Mail;
using Zeladoria.Core.Exceptions;

namespace Zeladoria.Core.Domain;

/// <summary>Usuário do sistema: cidadão (registra ocorrências) ou agente (atualiza o status).</summary>
public class Usuario
{
    public const int NomeMinimo = 3;
    public const int NomeMaximo = 100;
    public const int EmailMaximo = 150;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public PerfilUsuario Perfil { get; private set; }
    public DateTime CriadoEm { get; private set; }

    // Usado pelo EF Core.
    private Usuario() { }

    public Usuario(string nome, string email, PerfilUsuario perfil)
    {
        Definir(nome, email, perfil);
        CriadoEm = DateTime.UtcNow;
    }

    public bool EhAgente => Perfil == PerfilUsuario.Agente;

    public void Atualizar(string nome, string email, PerfilUsuario perfil) => Definir(nome, email, perfil);

    public static string NormalizarEmail(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();

    private void Definir(string nome, string email, PerfilUsuario perfil)
    {
        var nomeLimpo = (nome ?? string.Empty).Trim();
        if (nomeLimpo.Length < NomeMinimo || nomeLimpo.Length > NomeMaximo)
            throw new DomainValidationException($"O nome deve ter entre {NomeMinimo} e {NomeMaximo} caracteres.");

        var emailLimpo = NormalizarEmail(email);
        if (!EmailValido(emailLimpo))
            throw new DomainValidationException("E-mail inválido.");

        if (!Enum.IsDefined(perfil))
            throw new DomainValidationException("Perfil inválido. Use Cidadao ou Agente.");

        Nome = nomeLimpo;
        Email = emailLimpo;
        Perfil = perfil;
    }

    private static bool EmailValido(string email)
    {
        if (email.Length == 0 || email.Length > EmailMaximo)
            return false;

        return MailAddress.TryCreate(email, out var endereco) && endereco.Address == email;
    }
}
