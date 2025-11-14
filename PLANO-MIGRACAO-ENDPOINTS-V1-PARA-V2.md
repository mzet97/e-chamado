# Plano de Migração - Endpoints V1 para V2

## 📋 Análise dos Endpoints V2 (Auth)

### ✅ **Padrão Identificado nos Endpoints V2/Auth**

#### 1. **DTOs Otimizados**
```csharp
// LoginRequestDto.cs - Apenas campos essenciais
public class LoginRequestDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Password { get; set; } = string.Empty;
}
```

#### 2. **Extension Methods de Mapeamento**
```csharp
// AuthDTOSExtensions.cs
public static LoginUserCommand ToCommand(this LoginRequestDto requestDto)
{
    return new LoginUserCommand(requestDto.Email, requestDto.Password);
}
```

#### 3. **Endpoints V2 com Validações Avançadas**
```csharp
// LoginUserEndpointV2.cs
public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/login", HandleAsync)
        .WithName("Login: login in application (V2)")
        .WithSummary("Faz o login (versão otimizada)")
        .WithOrder(2); // Prioridade menor que V1

private static async Task<IResult> HandleAsync(
    [FromServices] IAmACommandProcessor commandProcessor,
    [FromBody] LoginRequestDto request)
{
    try
    {
        // Validação manual customizada
        if (!IsValidEmail(request.Email))
        {
            return TypedResults.BadRequest(new BaseResult<LoginResponseViewModel>(
                null, success: false, message: "Email inválido"));
        }

        // Mapeamento e execução
        var command = request.ToCommand();
        await commandProcessor.SendAsync(command);
        var result = command.Result;

        if (result?.Success == true)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result ?? 
            new BaseResult<LoginResponseViewModel>(null, success: false, 
                message: "Usuário ou senha inválidos"));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro no login: {ex.Message}");
        return TypedResults.Problem(
            detail: "Erro interno durante o processo de login",
            statusCode: 500);
    }
}
```

## 📊 **Inventário Completo - 49 Endpoints V1 para Migrar**

### **Por Categoria:**

#### 🔐 **Auth (2 endpoints)**
- [x] LoginUserEndpoint.cs ✓ **MIGRADO PARA V2**
- [x] RegisterUserEndpoint.cs ✓ **MIGRADO PARA V2**

#### 📂 **Categories (5 endpoints)**
- [ ] CreateCategoryEndpoint.cs
- [ ] DeleteCategoryEndpoint.cs  
- [ ] GetCategoryByIdEndpoint.cs
- [ ] SearchCategoriesEndpoint.cs
- [ ] UpdateCategoryEndpoint.cs

#### 💬 **Comments (3 endpoints)**
- [ ] CreateCommentEndpoint.cs
- [ ] DeleteCommentEndpoint.cs
- [ ] GetCommentsByOrderIdEndpoint.cs

#### 🏢 **Departments (6 endpoints)**
- [ ] CreateDepartmentEndpoint.cs
- [ ] DeletesDepartmentEndpoint.cs
- [ ] DisableDepartmentEndpoint.cs
- [ ] GetByIdDepartmentEndpoint.cs
- [ ] SearchDepartmentEndpoint.cs
- [ ] UpdateDepartmentEndpoint.cs
- [ ] UpdateStatusDepartmentEndpoint.cs

#### 📋 **OrderTypes (5 endpoints)**
- [ ] CreateOrderTypeEndpoint.cs
- [ ] DeleteOrderTypeEndpoint.cs
- [ ] GetOrderTypeByIdEndpoint.cs
- [ ] SearchOrderTypesEndpoint.cs
- [ ] UpdateOrderTypeEndpoint.cs

#### 🎫 **Orders (7 endpoints)**
- [ ] AssignOrderEndpoint.cs
- [ ] ChangeStatusOrderEndpoint.cs
- [ ] CloseOrderEndpoint.cs
- [ ] CreateOrderEndpoint.cs
- [ ] GetOrderByIdEndpoint.cs
- [ ] SearchOrdersEndpoint.cs
- [ ] UpdateOrderEndpoint.cs

#### 👥 **Roles (6 endpoints)**
- [ ] CreateRoleEndpoint.cs
- [ ] DeleteRoleEndpoint.cs
- [ ] GetAllRolesEndpoint.cs
- [ ] GetRoleByIdEndpoint.cs
- [ ] GetRoleByNameEndpoint.cs
- [ ] UpdateRoleEndpoint.cs

#### 🔄 **StatusTypes (5 endpoints)**
- [ ] CreateStatusTypeEndpoint.cs
- [ ] DeleteStatusTypeEndpoint.cs
- [ ] GetStatusTypeByIdEndpoint.cs
- [ ] SearchStatusTypesEndpoint.cs
- [ ] UpdateStatusTypeEndpoint.cs

#### 📚 **SubCategories (5 endpoints)**
- [ ] CreateSubCategoryEndpoint.cs
- [ ] DeleteSubCategoryEndpoint.cs
- [ ] GetSubCategoryByIdEndpoint.cs
- [ ] SearchSubCategoriesEndpoint.cs
- [ ] UpdateSubCategoryEndpoint.cs

#### 👤 **Users (3 endpoints)**
- [ ] GetAllUsersEndpoint.cs
- [ ] GetByEmailUserEndpoint.cs
- [ ] GetByIdUserEndpoint.cs

## 🚀 **Estratégia de Migração**

### **Fase 1: Estrutura Base**
1. Criar pastas de DTOs por categoria
2. Criar extension methods globais
3. Definir padrões de validação

### **Fase 2: Migração por Categoria** 
1. **Categories** (5 endpoints)
2. **Comments** (3 endpoints)  
3. **Departments** (6 endpoints)
4. **OrderTypes** (5 endpoints)
5. **Orders** (7 endpoints)
6. **Roles** (6 endpoints)
7. **StatusTypes** (5 endpoints)
8. **SubCategories** (5 endpoints)
9. **Users** (3 endpoints)

### **Fase 3: Validação e Testes**
1. Testar compilação de todos os endpoints
2. Validar funcionamento dos DTOs
3. Documentar mudanças

## 📋 **Checklist de Implementação**

### **Para cada endpoint V1 → V2:**
- [ ] 1. Analisar campos necessários do Command original
- [ ] 2. Criar DTO otimizado com validações
- [ ] 3. Criar extension method de mapeamento  
- [ ] 4. Implementar endpoint V2 com validações
- [ ] 5. Adicionar tratamento de erros
- [ ] 6. Testar compilação
- [ ] 7. Preservar endpoint V1 original

### **Exemplo de Estrutura por Categoria:**
```
/Endpoints/[Category]/DTOs/
├── [Category]RequestDto.cs
├── [Category]ResponseDto.cs  
└── [Category]DTOExtensions.cs

/Endpoints/[Category]/[Category]EndpointV2.cs
```

---
**Status**: ✅ Análise Completa  
**Próximo Passo**: Iniciar migração Fase 1 - Estrutura Base