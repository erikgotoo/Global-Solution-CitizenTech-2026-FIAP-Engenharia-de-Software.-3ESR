using Microsoft.AspNetCore.Mvc;
using Zeladoria.Core.Domain;
using Zeladoria.Core.Dtos;
using Zeladoria.Core.Services;

namespace Zeladoria.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
public class UsuariosController(IUsuarioService service) : ControllerBase
{
    /// <summary>Cadastra um usuário (perfil Cidadao ou Agente).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Criar([FromBody] UsuarioRequest request, CancellationToken ct)
    {
        var criado = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    /// <summary>Identifica o usuário pelo e-mail e devolve seus dados e perfil.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioResponse>> Login([FromBody] LoginRequest request, CancellationToken ct) =>
        Ok(await service.LoginAsync(request, ct));

    /// <summary>Lista os usuários, com filtro opcional por perfil.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<UsuarioResponse>>> Listar([FromQuery] PerfilUsuario? perfil, CancellationToken ct) =>
        Ok(await service.ListarAsync(perfil, ct));

    /// <summary>Obtém um usuário pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> Obter(int id, CancellationToken ct) =>
        Ok(await service.ObterAsync(id, ct));

    /// <summary>Atualiza nome, e-mail e perfil de um usuário.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Atualizar(int id, [FromBody] UsuarioRequest request, CancellationToken ct) =>
        Ok(await service.AtualizarAsync(id, request, ct));

    /// <summary>Remove um usuário que não possui histórico de status.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await service.RemoverAsync(id, ct);
        return NoContent();
    }
}
