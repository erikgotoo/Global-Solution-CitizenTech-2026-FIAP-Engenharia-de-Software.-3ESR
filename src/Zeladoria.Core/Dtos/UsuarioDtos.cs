using System.ComponentModel.DataAnnotations;
using Zeladoria.Core.Domain;

namespace Zeladoria.Core.Dtos;

public class UsuarioRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(Usuario.NomeMaximo, MinimumLength = Usuario.NomeMinimo, ErrorMessage = "O nome deve ter entre {2} e {1} caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(Usuario.EmailMaximo, ErrorMessage = "O e-mail deve ter no máximo {1} caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O perfil é obrigatório (Cidadao ou Agente).")]
    [EnumDataType(typeof(PerfilUsuario), ErrorMessage = "Perfil inválido. Use Cidadao ou Agente.")]
    public PerfilUsuario? Perfil { get; set; }
}

public class LoginRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;
}

public record UsuarioResponse(int Id, string Nome, string Email, PerfilUsuario Perfil, DateTime CriadoEm);
