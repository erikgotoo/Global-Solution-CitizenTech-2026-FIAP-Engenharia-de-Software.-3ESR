using Zeladoria.Core.Domain;
using Zeladoria.Core.Interfaces;

namespace Zeladoria.Tests.Fakes;

/// <summary>Repositório em memória usado nos testes unitários (sem banco de dados).</summary>
internal sealed class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly List<Usuario> _itens = [];
    private int _proximoId = 1;

    public Task<Usuario?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_itens.FirstOrDefault(u => u.Id == id));

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_itens.FirstOrDefault(u => u.Email == email));

    public Task<IReadOnlyList<Usuario>> ListarAsync(PerfilUsuario? perfil, CancellationToken ct = default)
    {
        IReadOnlyList<Usuario> lista = _itens
            .Where(u => !perfil.HasValue || u.Perfil == perfil.Value)
            .OrderBy(u => u.Nome)
            .ToList();
        return Task.FromResult(lista);
    }

    public Task<bool> EmailEmUsoAsync(string email, int? ignorarUsuarioId, CancellationToken ct = default) =>
        Task.FromResult(_itens.Any(u => u.Email == email && (ignorarUsuarioId == null || u.Id != ignorarUsuarioId)));

    public Task AdicionarAsync(Usuario usuario, CancellationToken ct = default)
    {
        DefinirId(usuario, _proximoId++);
        _itens.Add(usuario);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Usuario usuario, CancellationToken ct = default) => Task.CompletedTask;

    public Task RemoverAsync(Usuario usuario, CancellationToken ct = default)
    {
        _itens.Remove(usuario);
        return Task.CompletedTask;
    }

    // O Id tem setter privado (como no EF Core); aqui simulamos o banco gerando o valor.
    internal static void DefinirId(object entidade, int id) =>
        entidade.GetType().GetProperty("Id")!.SetValue(entidade, id);
}
