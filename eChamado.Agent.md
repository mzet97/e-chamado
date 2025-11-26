````markdown
# Prompt – Criação do Projeto **eChamado.Agent** (Core + Host + UI MAUI)

> Este prompt é para ser usado por **outra IA** (por exemplo, um code assistant como Claude Code, ChatGPT Code, etc.) para **projetar e implementar** o novo componente **eChamado.Agent** dentro de uma solução que já possui:
> - `EChamado.Server`
> - `EChamado.Client`

---

## 1. Seu papel

Você é um **arquiteto e desenvolvedor sênior .NET / C#** especializado em:

- Aplicações corporativas com **.NET 8**
- **Windows Service / Worker Service**
- **.NET MAUI** (UI cross-platform)
- APIs REST, Clean Architecture leve, boas práticas de logging, configuração e observabilidade.

Seu objetivo é **criar o design e o código inicial** do novo componente **eChamado.Agent**, alinhado com o contexto descrito abaixo.

---

## 2. Contexto do sistema eChamado

Já existem:

- `EChamado.Server` – API/Backend principal do sistema de chamados.
- `EChamado.Client` – Frontend web do sistema (provavelmente SPA).

Agora será introduzido o **eChamado.Agent**, que é o **agente instalado na máquina do usuário**, responsável por:

- Comunicar-se com `EChamado.Server` para:
  - enviar heartbeat/telemetria da máquina;
  - buscar tarefas de automação vinculadas a chamados (tickets);
  - reportar resultados de execução.
- Executar **tarefas locais** com privilégios elevados (admin), como:
  - instalação/remoção/atualização de software;
  - diagnósticos de sistema (CPU, Disco, Rede, etc.);
  - correções automatizadas;
  - coleta de logs.
- Oferecer **UI local para o usuário** (via .NET MAUI), incluindo:
  - status do agente;
  - execução manual de diagnósticos;
  - aceitação/recusa de sessões remotas;
  - eventualmente, ajustes de configurações locais do Agent.
- No futuro, suportar outros sistemas operacionais (não apenas Windows).

**Muito importante:**  
O eChamado.Agent **não conversa diretamente com o provedor de IA** (Gemini, etc.). Ele conversa apenas com `EChamado.Server`, que por sua vez chama um **Recurso de IA** já definido (AI Service) para gerar planos de automação.

---

## 3. Objetivo desta tarefa

Você deve:

1. **Propor e criar** a estrutura de projetos para o **eChamado.Agent** dentro da solução existente.
2. Implementar o **esqueleto funcional** (projeto inicial) com:
   - Core de domínio e aplicação;
   - Host/serviço que roda em background (Windows Service / Worker Service .NET 8) expondo uma API local;
   - UI .NET MAUI que se comunica com essa API local;
   - Integração básica com `EChamado.Server` prevista em código (endpoints, clientes HTTP, modelos).
3. Garantir que tudo esteja organizado de forma **limpa, extensível e sustentável** no longo prazo.

Você deve **desenhar e gerar código**, não apenas descrever em texto.

---

## 4. Tecnologias, versões e padrões

Use:

- **.NET 8** (C# 12).
- `Worker Service` para o host do Agent que irá rodar como **Windows Service** (pensando em futuro suporte a Linux / systemd).
- **Minimal APIs** para a API local do Agent.
- **.NET MAUI** para a UI do Agent (`EChamado.Agent.UI`).
- `HttpClient` para comunicação com `EChamado.Server`.
- Injeção de dependência padrão do `Microsoft.Extensions.DependencyInjection`.
- Logging com `Microsoft.Extensions.Logging`.

Padrões gerais:

- Separar bem **Domínio / Aplicação / Infraestrutura / Host / UI**.
- Manter o código **clean, testável, com responsabilidade clara**.
- Não aplicar uma Clean Architecture “extremamente burocrática”, mas sim algo leve e pragmático.

---

## 5. Estrutura de solução desejada

Considere que já existe uma solution como, por exemplo:

- `EChamado.sln`  
  contendo `EChamado.Server` e `EChamado.Client`.

Você deve adicionar os seguintes projetos na pasta `src/`:

```text
src/
  EChamado.Agent.Core/            (Class Library .NET 8)
  EChamado.Agent.Host/            (Worker Service .NET 8 + Minimal APIs locais)
  EChamado.Agent.UI/              (App .NET MAUI)
````

### 5.1. `EChamado.Agent.Core`

**Tipo:** Class Library (.NET 8)

Responsabilidades:

* Domínio do Agent:

  * Entidades como `AgentTask`, `AgentStatus`, `AgentMachineContext`, etc.
* Regras de aplicação:

  * Interface e implementação de `IAgentTaskExecutor`.
  * Serviços para execução de tarefas (instalação de software, diagnóstico, etc.).
* Contratos de integração com `EChamado.Server`:

  * DTOs básicos para heartbeat, tarefas pendentes, envio de resultados.

Organização sugerida:

```text
EChamado.Agent.Core/
  Domain/
    AgentTask.cs
    AgentTaskType.cs
    AgentStatus.cs
    AgentMachineContext.cs
  Application/
    IAgentTaskExecutor.cs
    AgentTaskExecutor.cs
  Infrastructure/
    EChamadoApiClient.cs        // HttpClient p/ EChamado.Server
    MachineInfoProvider.cs      // coleta de informações da máquina local
```

### 5.2. `EChamado.Agent.Host`

**Tipo:** Projeto console/worker (SDK: Worker Service .NET 8)
**Função:** Host do Agent, rodando como serviço + API local.

Responsabilidades:

* Inicializar o Agent (bootstrap de DI, logging, config).
* Rodar um `BackgroundService` (ex.: `AgentWorker`) que:

  * envia heartbeat para `EChamado.Server`;
  * busca tarefas pendentes;
  * delega execução ao `IAgentTaskExecutor` do `EChamado.Agent.Core`.
* Expor uma API HTTP local (Minimal API) em `localhost` para:

  * `/local/health` – status do Agent;
  * `/local/run-diagnostic` – acionar diagnóstico local;
  * endpoints futuros (sessão remota, logs, config).
* Estar pronto para ser instalado como **Windows Service** (e, futuramente, daemon em Linux).

Organização sugerida:

```text
EChamado.Agent.Host/
  Program.cs
  AgentRuntimeState.cs
  AgentWorker.cs
  LocalEndpoints.cs              // se quiser extrair o mapeamento de endpoints
```

### 5.3. `EChamado.Agent.UI` (MAUI)

**Tipo:** App .NET MAUI

Responsabilidades:

* Fornecer uma UI amigável para o usuário na máquina local:

  * Mostrar status do Agent (online, última comunicação, SO, hostname).
  * Permitir gatilhos manuais: “Executar diagnóstico agora”.
  * Mostrar aviso quando houver sessão remota ativa / convite para sessão remota.
* Comunicar-se com a API local do Host em `http://127.0.0.1:<porta>`.

Organização sugerida:

```text
EChamado.Agent.UI/
  App.xaml
  App.xaml.cs
  AppShell.xaml
  AppShell.xaml.cs
  Pages/
    HomePage.xaml
    HomePage.xaml.cs
    DiagnosticsPage.xaml
    SettingsPage.xaml   // opcional
  Services/
    LocalAgentClient.cs
```

---

## 6. Modelos e classes que você deve criar

Abaixo estão **diretrizes de classes** que você deve implementar no código, respeitando nomes e responsabilidades (pode enriquecer conforme necessário).

### 6.1. Domínio (EChamado.Agent.Core/Domain)

#### `AgentTaskType`

Enum representando tipos de tarefas:

* `InstallSoftware`
* `UninstallSoftware`
* `RunDiagnostic`
* `CollectLogs`
* `RemoteSessionControl`
* Outros que julgar úteis.

#### `AgentTask`

```csharp
public class AgentTask
{
    public string TaskId { get; init; } = default!;
    public string TicketId { get; init; } = default!;
    public AgentTaskType Type { get; init; }
    public string? PayloadJson { get; init; }  // detalhes específicos da tarefa
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ExecutedAt { get; set; }
}
```

#### `AgentStatus`

```csharp
public class AgentStatus
{
    public string AgentId { get; init; } = default!;
    public string Hostname { get; init; } = default!;
    public string OsDescription { get; init; } = default!;
    public bool IsOnline { get; set; }
    public bool HasPendingTasks { get; set; }
    public string? LastError { get; set; }
    public DateTimeOffset LastHeartbeat { get; set; }
}
```

#### `AgentMachineContext`

Classe com informações da máquina local (SO, hardware, rede). Use de forma simples no início (pode ser expandida depois).

---

### 6.2. Aplicação (EChamado.Agent.Core/Application)

#### `IAgentTaskExecutor`

Interface principal para execução de tarefas:

```csharp
public interface IAgentTaskExecutor
{
    Task ExecuteAsync(AgentTask task, CancellationToken ct = default);
}
```

#### `AgentTaskExecutor`

Implementação padrão:

* Implementar `ExecuteAsync` com `switch` em `AgentTask.Type`.
* Para cada tipo, chamar métodos privados:

  * `ExecuteInstallSoftwareAsync`
  * `ExecuteRunDiagnosticAsync`
  * etc.
* Por enquanto, você pode **simular** as ações com logs (`ILogger`), deixando comentários `// TODO` onde futuramente serão chamadas para PowerShell, winget, etc.

---

### 6.3. Infraestrutura (EChamado.Agent.Core/Infrastructure)

#### `EChamadoApiClient`

Cliente HTTP para `EChamado.Server`.

Responsabilidades:

* `GetPendingTasksAsync(agentId)`
* `SendHeartbeatAsync(AgentStatus status)`
* `SendTaskResultAsync(...)` (pode ser stub inicial)

Use `HttpClient` com `BaseAddress` configurável.

#### `MachineInfoProvider`

Serviço que coleta informações da máquina:

* Hostname
* OSDescription
* Informação básica de hardware (RAM, disco)
* Essas informações alimentam `AgentStatus` e `AgentMachineContext`.

---

## 7. Host do Agent (EChamado.Agent.Host)

### 7.1. `AgentRuntimeState`

Classe singleton contendo estado atual do Agent:

* `AgentStatus` (com `AgentId`, `Hostname`, etc.)
* Pode inicializar `AgentId` como `Guid.NewGuid().ToString("N")` (no futuro pode ser persistido).

### 7.2. `AgentWorker` (BackgroundService)

Implementar um `BackgroundService` assim:

* Loop principal (`ExecuteAsync`):

  1. Atualiza `LastHeartbeat`.
  2. Chama `EChamadoApiClient.SendHeartbeatAsync`.
  3. Busca tarefas pendentes com `GetPendingTasksAsync`.
  4. Para cada tarefa:

     * Executa com `IAgentTaskExecutor`.
     * Registra resultados (por enquanto, log + TODO para enviar ao servidor).
  5. Espera alguns segundos (ex.: 15s) e repete.

Tratar exceptions com `try/catch`, logar erro em `_logger` e preencher `AgentStatus.LastError`.

### 7.3. `Program.cs`

Criar Host com:

* `builder.Host.UseWindowsService();` (para rodar como serviço no Windows).
* Registro de serviços:

  * `EChamadoApiClient` com `AddHttpClient`.
  * `AgentRuntimeState` como singleton.
  * `IAgentTaskExecutor` como `Scoped` ou `Singleton` (pode ser `Scoped` com `CreateScope` no worker).
  * `AgentWorker` como `AddHostedService<AgentWorker>()`.
* Minimal APIs locais:

  * `GET /local/health`
  * `POST /local/run-diagnostic`

Exemplo de `GET /local/health`:

* Retornar um JSON com:

  * `status` (“ok”, “error”, etc.)
  * `AgentStatus` atual.

---

## 8. UI MAUI (EChamado.Agent.UI)

### 8.1. `LocalAgentClient`

Criar classe para consumir a API local do Host:

* `GetHealthAsync()` → chama `GET /local/health`.
* `RunDiagnosticAsync()` → chama `POST /local/run-diagnostic`.

Usar `HttpClient` com `BaseAddress` apontando para `http://127.0.0.1:<porta>` (defina uma porta padrão e deixe comentários para tornar configurável).

### 8.2. `HomePage` (exemplo)

Criar `HomePage.xaml` e `HomePage.xaml.cs` com:

* Labels para:

  * Status do Agent
  * Hostname
  * SO
* Botão “Executar diagnóstico local”.
* Ao abrir a página, chamar `LocalAgentClient.GetHealthAsync()` e atualizar UI.
* Ao clicar no botão, chamar `RunDiagnosticAsync()` e exibir resultado em `DisplayAlert`.

---

## 9. Integração com EChamado.Server

Você **não deve reescrever** ou refatorar completamente o `EChamado.Server`, mas:

* Proponha (em código ou comentários) **novos endpoints REST** no `EChamado.Server` para:

  * `POST /api/agent/heartbeat`
  * `GET /api/agent/{agentId}/tasks/pending`
  * `POST /api/agent/{agentId}/tasks/{taskId}/result`
* Para fins desta tarefa, você pode:

  * criar DTOs necessários;
  * esboçar controllers ou minimal APIs;
  * usar comentários `// TODO` onde for dependente de infraestrutura do servidor já existente (banco de dados, auth, etc.).

O foco é **ajustar o Agent** e mostrar claramente como ele fala com o servidor.

---

## 10. Qual deve ser o formato da sua resposta

Ao responder a este prompt, você deve:

1. **Descrever rapidamente** a solução (em poucas linhas) só para contextualizar.

2. Em seguida, gerar **o código completo necessário** para o esqueleto dos projetos novos:

   * `EChamado.Agent.Core` – com classes mínimas necessárias.
   * `EChamado.Agent.Host` – com `Program.cs`, `AgentRuntimeState`, `AgentWorker` e Minimal APIs.
   * `EChamado.Agent.UI` – com estrutura MAUI básica (App, AppShell, HomePage, LocalAgentClient).

3. Mostrar os arquivos em blocos de código separados, com caminhos comentados, por exemplo:

   ```csharp
   // src/EChamado.Agent.Core/Domain/AgentTask.cs
   ...
   ```

4. Evitar texto longo entre blocos de código — seja objetivo e focado em código.

5. Sempre garantir que os projetos **compilariam** (ao menos em teoria) com o .NET 8, mesmo que algumas partes sejam `TODO`.

---

## 11. Regras finais

* **Não toque** em `EChamado.Client`.
* Só proponha mudanças em `EChamado.Server` quando necessário para integração, em arquivos novos ou bem localizados (controllers, endpoints, DTOs).
* Não invente frameworks extras desnecessários (como MediatR, Dapper, etc.) sem justificar.
* Priorize simplicidade e clareza do código.
* Escreva tudo em **português do Brasil** nos comentários e explicações, mas siga padrões de nomenclatura C# em inglês (classes, métodos, propriedades).

Quando estiver pronto, gere a solução completa seguindo essas instruções.

```
::contentReference[oaicite:0]{index=0}
```
