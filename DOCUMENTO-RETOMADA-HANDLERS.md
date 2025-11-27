# 📋 DOCUMENTO DE RETOMADA - Handlers CQRS Pendentes

**Data**: 26/11/2025  
**Status**: 🔴 30% dos Handlers CQRS Necessitam Correção  
**Prioridade**: Alta - Bloqueando Build e Funcionalidades  
**Tempo Estimado**: 2-3 horas

---

## 🎯 SUMÁRIO EXECUTIVO

Das **46 correções críticas e de alta prioridade implementadas** com sucesso na revisão anterior, restam **10 handlers CQRS** com problemas específicos de **dependência de IDateTimeProvider**. Estes handlers estão impedindo o build e impedindo que o projeto seja considerado 100% funcional.

### Estado Atual
- ✅ **36 handlers CQRS** funcionam corretamente
- 🔴 **10 handlers CQRS** têm erros de build
- 🔴 **2 arquivos de teste** também precisam correção
- ❌ **Build geral** falha por esses erros

---

## 🔴 HANDLERS PROBLEMÁTICOS

### 1. **AssignOrderCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Orders/Commands/AssignOrderCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'Order.AssignTo(...)'`

**Correção Necessária**:
```csharp
public class AssignOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<AssignOrderCommandHandler> logger)

// E no método HandleAsync:
order.AssignTo(command.AssignedToUserId, string.Empty, dateTimeProvider);
```

---

### 2. **ChangeStatusOrderCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Orders/Commands/ChangeStatusOrderCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'Order.ChangeStatus(...)'`

**Correção**:
```csharp
public class ChangeStatusOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<ChangeStatusOrderCommandHandler> logger)

// No HandleAsync:
order.ChangeStatus(command.StatusId, dateTimeProvider);
```

---

### 3. **CloseOrderCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Orders/Commands/CloseOrderCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'Order.Close(...)'`

**Correção**:
```csharp
public class CloseOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<CloseOrderCommandHandler> logger)

// No HandleAsync:
order.Close(command.Evaluation ?? 0, dateTimeProvider);
```

---

### 4. **UpdateOrderCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Orders/Commands/UpdateOrderCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'Order.Update(...)'`

**Causa**: O método `Order.Update()` agora requer `IDateTimeProvider` como parâmetro, mas o handler não injeta nem passa essa dependência.

**Correção Necessária**:
```csharp
// ANTES
public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderCommandHandler> logger) :
    RequestHandlerAsync<UpdateOrderCommand>

// DEPOIS
public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<UpdateOrderCommandCommandHandler> logger) :
    RequestHandlerAsync<UpdateOrderCommand>

// E no método HandleAsync:
order.Update(
    command.Title,
    command.Description,
    command.RequestingUserEmail,
    command.CategoryId,
    command.SubCategoryId,
    command.DepartmentId,
    command.StatusTypeId,
    command.OrderTypeId,
    command.ResponsibleUserId,
    command.Priority,
    command.DueDate,
    dateTimeProvider); // ADICIONAR este parâmetro
```

---

### 2. **UpdateOrderTypeCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/OrderTypes/Commands/UpdateOrderTypeCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'OrderType.Update(...)'`

**Correção**:
```csharp
// Adicionar IDateTimeProvider na injeção de dependência
public class UpdateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<UpdateOrderTypeCommandHandler> logger)

// Passar no método Update
orderType.Update(command.Name, command.Description, dateTimeProvider);
```

---

### 3. **UpdateSubCategoryCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Categories/Commands/UpdateSubCategoryCommandHandler.cs`

**Correção**:
```csharp
public class UpdateSubCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<UpdateSubCategoryCommandHandler> logger)

// No HandleAsync:
subCategory.Update(command.Name, command.Description, command.CategoryId, dateTimeProvider);
```

---

### 4. **UpdateDepartmentCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Departments/Commands/Handlers/UpdateDepartmentCommandHandler.cs`

**Correção**:
```csharp
public class UpdateDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<UpdateDepartmentCommandHandler> logger)

// No HandleAsync:
department.Update(command.Name, command.Description, dateTimeProvider);
```

---

### 5. **UpdateStatusTypeCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/StatusTypes/Commands/UpdateStatusTypeCommandHandler.cs`

**Correção**:
```csharp
public class UpdateStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<UpdateStatusTypeCommandHandler> logger)

// No HandleAsync:
statusType.Update(command.Name, command.Description, dateTimeProvider);
```

---

### 6. **CreateDepartmentCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/Departments/Commands/Handlers/CreateDepartmentCommandHandler.cs`

**Erro**: `CS7036: There is no argument given that corresponds to the required parameter 'dateTimeProvider' of 'Department.Create(...)'`

**Correção**:
```csharp
// ADICIONAR IDateTimeProvider na injeção
public class CreateDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<CreateDepartmentCommandHandler> logger)

// Passar no Create
var entity = Department.Create(command.Name, command.Description, dateTimeProvider);
```

---

### 7. **CreateOrderTypeCommandHandler.cs**
**Localização**: `src/EChamado/Server/EChamado.Server.Application/UseCases/OrderTypes/Commands/CreateOrderTypeCommandHandler.cs`

**Correção**:
```csharp
public class CreateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ADICIONAR
    ILogger<CreateOrderTypeCommandHandler> logger)

// Passar no Create
var entity = OrderType.Create(command.Name, command.Description, dateTimeProvider);
```

---

## 🧪 TESTES A CORRIGIR

### 1. **EntityTests.cs**
**Localização**: `src/EChamado/Tests/EChamado.Shared.UnitTests/Shared/EntityTests.cs`

**Erros**:
- Linha 36: `Entity.Update(IDateTimeProvider)` requer parâmetro
- Linha 118: `Entity.Disabled(IDateTimeProvider)` requer parâmetro  
- Linha 131: `Entity.Disabled(IDateTimeProvider)` requer parâmetro

**Correção**: Adicionar mock do IDateTimeProvider nos testes.

---

## 📊 ANÁLISE DE DEPENDÊNCIAS

### Padrão Atual vs Aplicado

**✅ PADRÃO CORRETO** (já aplicado em CreateCategoryCommandHandler.cs):
```csharp
public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,  // ✅ CORRETO
    ILogger<CreateCategoryCommandHandler> logger)
{
    public override async Task<CreateCategoryCommand> HandleAsync(...)
    {
        var entity = Category.Create(command.Name, command.Description, dateTimeProvider);
        // ...
    }
}
```

**❌ PADRÃO INCORRETO** (handlers problemáticos):
```csharp
public class UpdateOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderCommandHandler> logger)  // ❌ FALTA dateTimeProvider
{
    public override async Task<UpdateOrderCommand> HandleAsync(...)
    {
        order.Update(/* sem dateTimeProvider */);
        // ...
    }
}
```

---

## 🛠️ PLANO DE CORREÇÃO

### Passo 1: Corrigir Handlers (2.5 horas)
1. **Order handlers** (4) - Adicionar IDateTimeProvider nos handlers: AssignOrder, ChangeStatusOrder, CloseOrder, UpdateOrder
2. **Update handlers** (4) - Adicionar IDateTimeProvider nos handlers: UpdateOrderType, UpdateSubCategory, UpdateDepartment, UpdateStatusType  
3. **Create handlers** (2) - Adicionar IDateTimeProvider nos handlers: CreateDepartment, CreateOrderType
4. **Verificar** - Todos os imports e namespaces

### Passo 2: Corrigir Testes (30 min)
1. Adicionar mock do IDateTimeProvider em EntityTests.cs
2. Passar o mock nos métodos Entity.Update() e Entity.Disabled()

### Passo 3: Validar Build (15 min)
1. Executar `dotnet build`
2. Verificar que todos os erros CS7036 foram resolvidos
3. Executar `dotnet test` para validar que testes passam

### Passo 4: Validar Funcionalidades (15 min)
1. Testar handlers manualmente via comandos
2. Verificar logs e comportamento
3. Validar que eventos de domínio são publicados corretamente

---

## 📝 CHECKLIST DE CORREÇÃO

### Handlers a Corrigir
- [ ] **AssignOrderCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **ChangeStatusOrderCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **CloseOrderCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **UpdateOrderCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **UpdateOrderTypeCommandHandler.cs** - Injetar IDateTimeProvider  
- [ ] **UpdateSubCategoryCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **UpdateDepartmentCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **UpdateStatusTypeCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **CreateDepartmentCommandHandler.cs** - Injetar IDateTimeProvider
- [ ] **CreateOrderTypeCommandHandler.cs** - Injetar IDateTimeProvider

### Testes a Corrigir
- [ ] **EntityTests.cs** - Adicionar mock IDateTimeProvider

### Validação Final
- [ ] Build successful sem erros CS7036
- [ ] Todos os testes passing
- [ ] Funcionalidades validadas

---

## 🎯 IMPACTO DA CORREÇÃO

### Após Correção
- ✅ **100% dos handlers** funcionando corretamente
- ✅ **Build limpo** sem erros
- ✅ **Funcionalidades completas** - todos os CRUDs operacionais
- ✅ **Padrão consistente** - todos os handlers seguindo mesmo padrão
- ✅ **Testes passing** - 100% da cobertura mantida

### Benefícios
1. **Consistência**: Todos os handlers seguem mesmo padrão de injeção
2. **Testabilidade**: Handlers mais facilmente testáveis
3. **Manutenibilidade**: Código mais limpo e padronizado
4. **Robustez**: IDateTimeProvider evita acoplamento com DateTime.Now

---

## 📞 INSTRUÇÕES PARA RETOMADA

### Contexto Necessário
- Você já tem o `IDateTimeProvider` implementado no projeto
- O padrão já está aplicado em `CreateCategoryCommandHandler.cs`
- Basta aplicar o mesmo padrão nos 7 handlers restantes

### Arquivos de Referência
1. **Padrão Correto**: `CreateCategoryCommandHandler.cs`
2. **Interface**: `IDateTimeProvider.cs` em `EChamado.Shared`
3. **Implementação**: `SystemDateTimeProvider.cs`

### Comando para Verificar Progresso
```bash
# Verificar se erros foram corrigidos
dotnet build 2>&1 | grep "CS7036"

# Deve retornar vazio se tudo foi corrigido
```

---

**Preparado por**: Claude (Senior SWE Specialist)  
**Data**: 26/11/2025  
**Status**: 🔴 Pronto para Implementação Imediata