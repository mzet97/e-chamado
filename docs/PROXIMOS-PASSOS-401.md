# Próximos Passos - Resolver 401 Token

## 🎯 Situação

Você está recebendo **401 Unauthorized** ao enviar Bearer token para a API. A correção já foi aplicada no código, mas o servidor precisa ser reconstruído.

## ✅ O Que Já Foi Feito

1. ✅ **IdentityConfig.cs corrigido** - DefaultChallengeScheme agora usa OpenIddict Validation
2. ✅ **Scripts criados** - rebuild-windows.ps1 e test-api-with-token.ps1
3. ✅ **Documentação completa** - GUIA-RESOLVER-401-TOKEN.md

## 🚀 Próximos Passos (Você Deve Executar)

### Passo 1: Parar o EChamado.Server

No terminal onde o EChamado.Server está rodando, pressione **Ctrl+C**.

### Passo 2: Rebuild no Windows PowerShell

**Abra o PowerShell como Administrador** e execute:

```powershell
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
.\rebuild-windows.ps1
```

O script irá:
- Parar processos dotnet
- Limpar o projeto completamente
- Remover bin/obj/staticwebassets
- Limpar cache NuGet
- Restaurar e reconstruir
- Perguntar se deseja iniciar o servidor (responda **S**)

### Passo 3: Testar com o Script

**Em um novo PowerShell**, execute:

```powershell
cd E:\TI\git\e-chamado
.\test-api-with-token.ps1
```

## ✅ Resultado Esperado

Se tudo funcionar corretamente:

```
==========================================
Teste Completo: Autenticação + API
==========================================

[1/4] Obtendo token do Auth Server (porta 7132)...
✅ Token obtido com sucesso

[2/4] Verificando token (primeiros 50 caracteres)...
Token: eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZDQkMtSFM1MTIi...
Expires in: 3600 segundos

[3/4] Testando API sem autenticação (deve retornar 401)...
✅ Retornou 401 Unauthorized (correto)

[4/4] Testando API COM token Bearer...
HTTP Status: 201
Response Body:
{
  "data": {
    "id": "...",
    "name": "Teste Com Token",
    "description": "Teste de autenticação"
  },
  "success": true,
  "message": "Category created successfully"
}

==========================================
✅ SUCESSO! API funcionando com token
==========================================
```

## ❌ Se Ainda Receber 401

Se o problema persistir após o rebuild, verifique:

1. **Ambos os servidores estão rodando?**
   ```powershell
   netstat -ano | findstr "7132"  # Echamado.Auth
   netstat -ano | findstr "7296"  # EChamado.Server
   ```

2. **Token não expirou?**
   - Tokens expiram em 1 hora
   - Gere um novo token se necessário

3. **Logs do servidor mostram erros?**
   - Verifique o console do EChamado.Server
   - Procure por: "Bearer token validation failed", "Signature validation failed", "Issuer validation failed"

## 📁 Arquivos Disponíveis

1. **rebuild-windows.ps1**
   - Localização: `src/EChamado/Server/EChamado.Server/rebuild-windows.ps1`
   - Usa: Rebuild completo do EChamado.Server

2. **test-api-with-token.ps1**
   - Localização: `test-api-with-token.ps1` (raiz)
   - Usa: Teste completo de autenticação + API

3. **GUIA-RESOLVER-401-TOKEN.md**
   - Localização: `GUIA-RESOLVER-401-TOKEN.md` (raiz)
   - Usa: Guia detalhado com troubleshooting

## 🔧 Comandos Rápidos

**Rebuild Completo:**
```powershell
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
dotnet clean
dotnet build
dotnet run --launch-profile https
```

**Obter Token (curl):**
```bash
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"
```

**Testar API (curl):**
```bash
curl -k -X POST https://localhost:7296/v1/category \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <SEU_TOKEN>" \
  -d '{"name": "Teste 1", "description": "Teste 1"}'
```

## 💡 Importante

- ⚠️ **Use APENAS Windows para build e execução** (não misture WSL e Windows)
- ⚠️ **Execute como Administrador** para garantir permissões adequadas
- ⚠️ **Pare completamente o servidor** antes de fazer rebuild
- ⚠️ **Aguarde o servidor inicializar** antes de testar (veja "Now listening on:")

## 📊 Status das Correções

| Problema | Status | Arquivo |
|----------|--------|---------|
| Compilation Error (CS0117) | ✅ RESOLVIDO | OpenIddictWorker.cs |
| invalid_scope | ✅ RESOLVIDO | Program.cs + OpenIddictWorker.cs |
| IOpenIddictService missing | ✅ RESOLVIDO | Program.cs + Echamado.Auth.csproj |
| Missing dependencies | ✅ RESOLVIDO | Program.cs |
| wwwroot path conflict | ✅ RESOLVIDO | rebuild-windows.ps1 |
| API returning HTML | ✅ RESOLVIDO | IdentityConfig.cs |
| **401 with valid token** | ⏳ **PENDENTE REBUILD** | IdentityConfig.cs |

---

**🎯 AÇÃO NECESSÁRIA: Execute o rebuild-windows.ps1 agora!**
