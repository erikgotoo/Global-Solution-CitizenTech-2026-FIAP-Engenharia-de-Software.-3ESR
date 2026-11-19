using Zeladoria.Core.Domain;

namespace Zeladoria.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<Usuario>> ListarAsync(PerfilUsuario? perfil, CancellationToken ct = default);
    Task<bool> EmailEmUsoAsync(string email, int? ignorarUsuarioId, CancellationToken ct = default);
    Task AdicionarAsync(Usuario usuario, CancellationToken ct = default);
    Task AtualizarAsync(Usuario usuario, CancellationToken ct = default);
    Task RemoverAsync(Usuario usuario, CancellationToken ct = default);
}
