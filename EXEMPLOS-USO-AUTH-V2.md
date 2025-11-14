# 🚀 Exemplos de Uso - Endpoints de Autenticação V2

## 📋 Estrutura dos DTOs

### LoginRequestDto.cs
```csharp
public class LoginRequestDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Password { get; set; } = string.Empty;
}
```

### RegisterRequestDto.cs
```csharp
public class RegisterRequestDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(255, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
```

---

## 🌐 Exemplos de Requests

### POST /v2/auth/login-v2

#### Request (JSON)
```json
{
  "email": "usuario@exemplo.com",
  "password": "senha123"
}
```

#### Response Sucesso (200)
```json
{
  "success": true,
  "message": "",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600,
    "userToken": {
      "id": "12345678-1234-1234-1234-123456789012",
      "email": "usuario@exemplo.com",
      "claims": [
        {
          "value": "user",
          "type": "role"
        }
      ]
    }
  }
}
```

#### Response Erro (400)
```json
{
  "success": false,
  "message": "Usuário ou senha inválidos",
  "data": null
}
```

---

### POST /v2/auth/register-v2

#### Request (JSON)
```json
{
  "email": "novousuario@exemplo.com",
  "password": "senha123456"
}
```

#### Response Sucesso (200)
```json
{
  "success": true,
  "message": "",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 3600,
    "userToken": {
      "id": "87654321-4321-4321-4321-210987654321",
      "email": "novousuario@exemplo.com",
      "claims": [
        {
          "value": "user",
          "type": "role"
        }
      ]
    }
  }
}
```

#### Response Erro - Email Inválido (400)
```json
{
  "success": false,
  "message": "Email inválido",
  "data": null
}
```

#### Response Erro - Senha Fraca (400)
```json
{
  "success": false,
  "message": "A senha deve ter pelo menos 6 caracteres",
  "data": null
}
```

---

## 🧪 Casos de Teste

### Casos Válidos

```json
// ✅ Login válido
{
  "email": "usuario@exemplo.com",
  "password": "minhasenha123"
}

// ✅ Registro válido
{
  "email": "novouser@exemplo.com",
  "password": "senhaforte456"
}
```

### Casos Inválidos

```json
// ❌ Email vazio
{
  "email": "",
  "password": "senha123"
}

// ❌ Email inválido
{
  "email": "email-invalido",
  "password": "senha123"
}

// ❌ Email sem @ 
{
  "email": "usuarioexemplo.com",
  "password": "senha123"
}

// ❌ Email com apenas @
{
  "email": "@exemplo.com",
  "password": "senha123"
}

// ❌ Senha vazia
{
  "email": "usuario@exemplo.com",
  "password": ""
}

// ❌ Senha muito curta
{
  "email": "usuario@exemplo.com", 
  "password": "123"
}

// ❌ Senha muito longa
{
  "email": "usuario@exemplo.com",
  "password": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
}
```

---

## 🔧 Integração com Clientes

### JavaScript/TypeScript

```typescript
interface LoginRequest {
  email: string;
  password: string;
}

interface LoginResponse {
  success: boolean;
  message: string;
  data: {
    accessToken: string;
    expiresIn: number;
    userToken: {
      id: string;
      email: string;
      claims: Array<{value: string, type: string}>;
    };
  } | null;
}

class AuthService {
  private baseURL = '/v2/auth';

  async login(email: string, password: string): Promise<LoginResponse> {
    const response = await fetch(`${this.baseURL}/login-v2`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password })
    });

    return await response.json();
  }

  async register(email: string, password: string): Promise<LoginResponse> {
    const response = await fetch(`${this.baseURL}/register-v2`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password })
    });

    return await response.json();
  }
}

// Uso
const auth = new AuthService();

try {
  const result = await auth.login('usuario@exemplo.com', 'senha123');
  
  if (result.success) {
    // Sucesso - usar result.data.accessToken
    localStorage.setItem('token', result.data!.accessToken);
  } else {
    // Erro - mostrar result.message
    console.error('Erro:', result.message);
  }
} catch (error) {
  console.error('Erro de rede:', error);
}
```

### C# Cliente

```csharp
public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public LoginDataDto? Data { get; set; }
}

public class LoginDataDto
{
    public string AccessToken { get; set; } = string.Empty;
    public double ExpiresIn { get; set; }
    public UserTokenViewModel UserToken { get; set; } = new();
}

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:5001/v2/auth/");
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string password)
    {
        var request = new LoginRequestDto { Email = email, Password = password };
        
        var response = await _httpClient.PostAsJsonAsync("login-v2", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        return new LoginResponseDto 
        { 
            Success = false, 
            Message = "Erro ao fazer login" 
        };
    }

    public async Task<LoginResponseDto?> RegisterAsync(string email, string password)
    {
        var request = new LoginRequestDto { Email = email, Password = password };
        
        var response = await _httpClient.PostAsJsonAsync("register-v2", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        return new LoginResponseDto 
        { 
            Success = false, 
            Message = "Erro ao fazer registro" 
        };
    }
}

// Uso
var authService = new AuthService(httpClient);

var loginResult = await authService.LoginAsync("usuario@exemplo.com", "senha123");

if (loginResult?.Success == true)
{
    var token = loginResult.Data?.AccessToken;
    // Usar token para autenticação
}
```

### Python

```python
import requests
from dataclasses import dataclass
from typing import Optional

@dataclass
class LoginRequest:
    email: str
    password: str

@dataclass
class LoginResponse:
    success: bool
    message: str
    data: Optional[dict] = None

class AuthService:
    def __init__(self, base_url: str = "https://localhost:5001/v2/auth"):
        self.base_url = base_url
        
    def login(self, email: str, password: str) -> LoginResponse:
        request_data = {"email": email, "password": password}
        
        try:
            response = requests.post(
                f"{self.base_url}/login-v2",
                json=request_data,
                headers={"Content-Type": "application/json"}
            )
            
            if response.status_code == 200:
                data = response.json()
                return LoginResponse(
                    success=data.get("success", False),
                    message=data.get("message", ""),
                    data=data.get("data")
                )
            else:
                return LoginResponse(
                    success=False,
                    message="Erro ao fazer login"
                )
        except requests.RequestException as e:
            return LoginResponse(
                success=False,
                message=f"Erro de rede: {str(e)}"
            )

    def register(self, email: str, password: str) -> LoginResponse:
        request_data = {"email": email, "password": password}
        
        try:
            response = requests.post(
                f"{self.base_url}/register-v2",
                json=request_data,
                headers={"Content-Type": "application/json"}
            )
            
            if response.status_code == 200:
                data = response.json()
                return LoginResponse(
                    success=data.get("success", False),
                    message=data.get("message", ""),
                    data=data.get("data")
                )
            else:
                return LoginResponse(
                    success=False,
                    message="Erro ao fazer registro"
                )
        except requests.RequestException as e:
            return LoginResponse(
                success=False,
                message=f"Erro de rede: {str(e)}"
            )

# Uso
auth = AuthService()

result = auth.login("usuario@exemplo.com", "senha123")

if result.success:
    token = result.data["accessToken"]
    print(f"Login realizado com sucesso. Token: {token[:20]}...")
else:
    print(f"Erro: {result.message}")
```

---

## 🔍 Validações Implementadas

### LoginRequestDto
- ✅ **Email obrigatório** - não pode estar vazio
- ✅ **Email válido** - deve conter formato de email válido
- ✅ **Senha obrigatória** - não pode estar vazia

### RegisterRequestDto
- ✅ **Email obrigatório** - não pode estar vazio
- ✅ **Email válido** - deve conter formato de email válido
- ✅ **Senha obrigatória** - não pode estar vazia
- ✅ **Senha mínimo 6 caracteres** - validação de força mínima

---

## 📈 Melhorias Implementadas

### Interface Limpa
- ✅ **Apenas campos necessários** no request body
- ✅ **Sem campos técnicos** (id, correlationId, result)
- ✅ **Validações específicas** por contexto

### Desenvolvimento
- ✅ **Tipos seguros** com C# records/interfaces
- ✅ **Documentação XML** em todos os campos
- ✅ **Erros de rede** tratados adequadamente

### Experiência do Usuário
- ✅ **Mensagens de erro** claras e específicas
- ✅ **Status codes** HTTP apropriados (200/400/500)
- ✅ **Response format** consistente

---

*Documento de exemplos criado em: $(Get-Date -Format "dd/MM/yyyy")*
*Especialista C#/.NET - EChamado Team*
