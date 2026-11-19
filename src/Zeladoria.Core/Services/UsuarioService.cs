using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Exceptions;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Core.Services;

public class UsuarioService(IUsuarioRepository usuarios, IHistoricoStatusRepository historico) : IUsuarioService
{
    public async Task<UsuarioResponse> CriarAsync(UsuarioRequest request, CancellationToken ct = default)
    {
        var perfil = request.Perfil ?? throw new DomainValidationException("O perfil é obrigatório (Cidadao ou Agente).");
        var usuario = new Usuario(request.Nome, request.Email, perfil);

        if (await usuarios.EmailEmUsoAsync(usuario.Email, null, ct))
            throw new ConflictException("Já existe um usuário cadastrado com este e-mail.");

        await usuarios.AdicionarAsync(usuario, ct);
        return usuario.ToResponse();
    }

    public async Task<UsuarioResponse> ObterAsync(int id, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");
        return usuario.ToResponse();
    }

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(PerfilUsuario? perfil, CancellationToken ct = default)
    {
        var lista = await usuarios.ListarAsync(perfil, ct);
        return lista.Select(u => u.ToResponse()).ToList();
    }

    public async Task<UsuarioResponse> AtualizarAsync(int id, UsuarioRequest request, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        var perfil = request.Perfil ?? throw new DomainValidationException("O perfil é obrigatório (Cidadao ou Agente).");
        usuario.Atualizar(request.Nome, request.Email, perfil);

        if (await usuarios.EmailEmUsoAsync(usuario.Email, usuario.Id, ct))
            throw new ConflictException("Já existe outro usuário cadastrado com este e-mail.");

        await usuarios.AtualizarAsync(usuario, ct);
        return usuario.ToResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var usuario = await usuarios.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException($"Usuário {id} não encontrado.");

        if (await historico.UsuarioPossuiHistoricoAsync(id, ct))
            throw new ConflictException("O usuário possui histórico de status e não pode ser removido.");

        await usuarios.RemoverAsync(usuario, ct);
    }

    public async Task<UsuarioResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = Usuario.NormalizarEmail(request.Email);
        var usuario = await usuarios.ObterPorEmailAsync(email, ct)
            ?? throw new AutenticacaoException("E-mail não cadastrado.");
        return usuario.ToResponse();
    }
}
