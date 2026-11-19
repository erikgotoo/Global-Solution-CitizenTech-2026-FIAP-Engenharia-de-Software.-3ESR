using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;

namespace Zeladoria.Core.Services;

public interface IUsuarioService
{
    Task<UsuarioResponse> CriarAsync(UsuarioRequest request, CancellationToken ct = default);
    Task<UsuarioResponse> ObterAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(PerfilUsuario? perfil, CancellationToken ct = default);
    Task<UsuarioResponse> AtualizarAsync(int id, UsuarioRequest request, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
    Task<UsuarioResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
