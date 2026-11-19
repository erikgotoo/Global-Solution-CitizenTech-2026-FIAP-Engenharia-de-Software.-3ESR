namespace Zeladoria.Core.Exceptions;

/// <summary>Dado inválido (a API responde 400).</summary>
public class DomainValidationException(string message) : Exception(message);

/// <summary>Recurso não encontrado (a API responde 404).</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Conflito com o estado atual, como e-mail repetido ou status que retrocede (a API responde 409).</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Usuário sem permissão para a ação (a API responde 403).</summary>
public class ForbiddenException(string message) : Exception(message);

/// <summary>Falha na identificação do usuário (a API responde 401).</summary>
public class AutenticacaoException(string message) : Exception(message);
