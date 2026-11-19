
Global Solution CitizenTech 2026 — FIAP, Engenharia de Software.

## Integrantes

Augusto Rocha Silva - RM556316
Erik Yuuta Goto - RM558076
Guilherme Vieira Augusto - RM557264
Wendell dos Santos Silva - RM558859

## Ficha da CitizenTech

| Item | Descrição |
|------|-----------|
| Nome | Zeladoria do Bairro |
| Cidade e bairro | São Paulo, bairro a definir |
| Cidadão beneficiado | Morador que convive com o problema (e o agente que atende as ocorrências) |
| Problema, em uma frase | O morador não tem um jeito simples de registrar problemas do espaço público do bairro e não sabe se o chamado foi atendido. |
| Como funciona | Registrar, acompanhar e atualizar ocorrências, com alerta automático de poste apagado (IoT). |
| Indicador de impacto | Tempo médio de resolução das ocorrências e % de ocorrências resolvidas. |
| ODS conectados | ODS 11 (cidades e comunidades sustentáveis) e ODS 9 (indústria, inovação e infraestrutura). |
| Dados | SP156 (chamados reais da cidade) para o modelo de ML; dados simulados nas demais partes. |

## Descrição desta parte

O morador registra um problema do espaço público (buraco, lixo, poste apagado) e acompanha o andamento. Esta API cuida de dois assuntos da jornada:

1. **Usuários**: cadastro com perfil **Cidadao** ou **Agente**, identificação por e-mail (login simplificado, sem senha, conforme o vocabulário comum do projeto) e consulta.
2. **Acompanhamento**: histórico de mudanças de status das ocorrências. Somente um **Agente** registra a mudança, e o status **só pode avançar**: `Aberta` → `EmAnalise` → `EmAndamento` → `Resolvida`.

As ocorrências em si (CRUD) ficam no serviço de SOA. Aqui o `ocorrenciaId` é apenas uma referência, sem chave estrangeira entre os serviços. Uma ocorrência sem nenhum registro de histórico está `Aberta`.

Nomes do vocabulário comum usados: `Usuario` (id, nome, perfil), perfis `Cidadao`/`Agente` e status `Aberta`, `EmAnalise` (Em análise), `EmAndamento` (Em andamento), `Resolvida`.

## Tecnologias

- .NET 10 e ASP.NET Core Web API (controllers)
- Entity Framework Core 10 com SQLite (migrations versionadas em `src/Zeladoria.Infrastructure/Migrations`)
- Documentação interativa com **Scalar** (OpenAPI gerado pela própria API)
- xUnit para testes unitários

## Arquitetura em camadas

```
src/
  Zeladoria.Core            Domínio e regras: entidades (Usuario, HistoricoStatus), interfaces
                            de repositório (IUsuarioRepository, IHistoricoStatusRepository),
                            DTOs, serviços de aplicação e exceções de negócio
  Zeladoria.Infrastructure  EF Core: DbContext, mapeamento, repositórios (async + LINQ), migrations
  Zeladoria.Api             Controllers REST, tratamento de erros (ProblemDetails), Program.cs (DI)
tests/
  Zeladoria.Tests           Testes unitários com repositórios em memória (fakes)
```

- **POO**: entidades com encapsulamento (setters privados, validação no construtor), interfaces para repositórios e serviços, injeção de dependência registrada em `Program.cs` e `AddInfrastructure`.
- **Web API**: códigos de status corretos (200, 201, 204, 400, 401, 403, 404, 409) devolvidos como `ProblemDetails`, com validação por DataAnnotations nos DTOs.
- **EF Core + LINQ**: filtros por perfil, agente e status; ordenação cronológica; `Include` do agente; migrations.
- **Async/Await**: todos os métodos de repositório, serviço e controller são assíncronos e recebem `CancellationToken`.

## Como executar

Pré-requisito: [.NET SDK 10](https://dotnet.microsoft.com/download).

Atalho: no Windows, dê dois cliques em `1-verificar.bat` (Mac/Linux: `bash 1-verificar.sh`). Ele restaura as ferramentas, compila, confere a migration, roda os testes e lista o que ainda falta. Para subir a API, use `2-executar-api.bat`.

Ou, manualmente, na pasta raiz do projeto (onde está o arquivo `Zeladoria.sln`):

```bash
dotnet tool restore
dotnet build
dotnet test
dotnet run --project src/Zeladoria.Api
```

A API sobe em **http://localhost:5080** e a documentação interativa (Scalar) abre em **http://localhost:5080/scalar/v1**. O OpenAPI fica em `/openapi/v1.json`. O banco SQLite (`zeladoria.db`) é criado e as migrations são aplicadas automaticamente na primeira execução.

A migration inicial foi gerada com:

```bash
dotnet ef migrations add InitialCreate --project src/Zeladoria.Infrastructure --startup-project src/Zeladoria.Api --output-dir Migrations
```

## Endpoints

| Método | Rota | Descrição | Status |
|--------|------|-----------|--------|
| POST | `/api/usuarios` | Cadastra usuário (`nome`, `email`, `perfil`) | 201, 400, 409 |
| POST | `/api/usuarios/login` | Identifica pelo e-mail | 200, 400, 401 |
| GET | `/api/usuarios?perfil=Agente` | Lista, com filtro opcional por perfil | 200, 400 |
| GET | `/api/usuarios/{id}` | Obtém um usuário | 200, 404 |
| PUT | `/api/usuarios/{id}` | Atualiza um usuário | 200, 400, 404, 409 |
| DELETE | `/api/usuarios/{id}` | Remove (se não tiver histórico) | 204, 404, 409 |
| POST | `/api/ocorrencias/{ocorrenciaId}/historico` | Agente registra mudança de status | 201, 400, 403, 404, 409 |
| GET | `/api/ocorrencias/{ocorrenciaId}/historico` | Histórico da ocorrência (ordem cronológica) | 200, 400 |
| GET | `/api/ocorrencias/{ocorrenciaId}/status-atual` | Status atual (`Aberta` se não houver histórico) | 200, 400 |
| GET | `/api/historico?agenteId=1&status=Resolvida` | Lista com filtros por agente e status | 200, 400 |

Exemplo de cadastro de agente e mudança de status:

```json
POST /api/usuarios
{ "nome": "Carlos Lima", "email": "carlos@prefeitura.test", "perfil": "Agente" }

POST /api/ocorrencias/10/historico
{ "agenteId": 1, "novoStatus": "EmAnalise", "observacao": "Equipe vistoriando o local" }
```

Os valores de enum são aceitos como texto: perfil `Cidadao` | `Agente`; status `Aberta` | `EmAnalise` | `EmAndamento` | `Resolvida`.

Somente dados fictícios são usados nos exemplos e testes (nenhum dado pessoal real).

## Prompts de IA

Os prompts de IA usados no desenvolvimento estão registrados em [`prompts.md`](prompts.md).
