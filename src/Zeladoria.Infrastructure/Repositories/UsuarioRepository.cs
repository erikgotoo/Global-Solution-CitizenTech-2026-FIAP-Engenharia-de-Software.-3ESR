using Microsoft.EntityFrameworkCore;
using Zeladoria.Core.Domain;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Infrastructure.Repositories;

public class UsuarioRepository(ZeladoriaDbContext db) : IUsuarioRepository
{
    public async Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<IReadOnlyList<Usuario>> ListarAsync(PerfilUsuario? perfil, CancellationToken ct = default)
    {
        var consulta = db.Usuarios.AsNoTracking();

        if (perfil.HasValue)
            consulta = consulta.Where(u => u.Perfil == perfil.Value);

        return await consulta.OrderBy(u => u.Nome).ToListAsync(ct);
    }

    public async Task<bool> EmailEmUsoAsync(string email, int? ignorarUsuarioId, CancellationToken ct = default) =>
        await db.Usuarios.AnyAsync(u => u.Email == email && (ignorarUsuarioId == null || u.Id != ignorarUsuarioId), ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        await db.Usuarios.AddAsync(usuario, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Usuario usuario, CancellationToken ct = default)
    {
        db.Usuarios.Update(usuario);
        await db.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Usuario usuario, CancellationToken ct = default)
    {
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync(ct);
    }
}
