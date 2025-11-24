# Correção: Chaves de Desenvolvimento OpenIddict

## ❌ Problema

Token é rejeitado com erro:
```
ValidateIdentityModelToken was marked as rejected
The specified token is invalid.
```

Mesmo com o servidor buscando as chaves corretamente do Auth Server.

## 🔍 Causa Raiz

O OpenIddict está usando **chaves de desenvolvimento efêmeras** que são **regeneradas a cada restart**.

Quando você:
1. Inicia Echamado.Auth → Gera chaves A
2. Gera um token → Assinado/encriptado com chaves A
3. Reinicia Echamado.Auth → Gera chaves B (DIFERENTES!)
4. Token antigo (com chaves A) se torna inválido

## ✅ Solução 1: Ordem de Inicialização (Solução Temporária)

**Sempre seguir esta ordem EXATA:**

```powershell
# 1. Parar TUDO
# Ctrl+C em todos os terminais

# 2. Iniciar Auth Server PRIMEIRO
cd E:\TI\git\e-chamado\src\EChamado\Echamado.Auth
dotnet run --launch-profile https

# Aguardar ver:
# ✅ OpenIddict clients and scopes configured successfully
# Now listening on: https://localhost:7132

# 3. Iniciar API Server DEPOIS
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
dotnet run --launch-profile https

# Aguardar ver:
# Now listening on: https://localhost:7296

# 4. Gerar um NOVO token
curl -k -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados"

# 5. Usar o token IMEDIATAMENTE (tokens expiram em 1 hora)
curl -k -X POST https://localhost:7296/v1/category \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <TOKEN_AQUI>" \
  -d '{"name": "Teste", "description": "Teste"}'
```

## ✅ Solução 2: Chaves Persistentes (Solução Definitiva)

Para evitar esse problema, vamos configurar chaves persistentes.

### Opção A: Compartilhar Pasta de Chaves

Ambos os servidores devem usar a **mesma pasta** de Data Protection:

**1. Criar variável de ambiente:**

```powershell
# PowerShell (Windows)
$env:DP_KEYS_PATH = "E:\TI\git\e-chamado\DataProtection-Keys"
```

**2. Reiniciar ambos os servidores**

Agora ambos usarão as mesmas chaves persistentes em disco.

### Opção B: Usar Certificados Fixos (Produção)

Para produção, use certificados reais em vez de chaves de desenvolvimento.

**Echamado.Auth/Program.cs:**

```csharp
// DESENVOLVIMENTO (chaves efêmeras)
.AddDevelopmentEncryptionCertificate()
.AddDevelopmentSigningCertificate();

// PRODUÇÃO (chaves fixas)
// .AddEncryptionCertificate(new X509Certificate2("path/to/encryption.pfx", "password"))
// .AddSigningCertificate(new X509Certificate2("path/to/signing.pfx", "password"));
```

## 🧪 Script de Teste Completo

Use este script para testar tudo de uma vez:

**PowerShell:**
```powershell
cd E:\TI\git\e-chamado
.\test-api-with-token.ps1
```

Este script:
1. ✅ Obtém um novo token
2. ✅ Testa API sem autenticação (deve retornar 401)
3. ✅ Testa API com token (deve retornar 200/201)
4. ✅ Mostra diagnóstico detalhado

## 📊 Verificação das Chaves

Para verificar se as chaves estão sincronizadas:

**1. Verificar chaves do Auth Server:**
```bash
curl -k https://localhost:7132/.well-known/jwks | jq
```

**2. Verificar discovery do Auth Server:**
```bash
curl -k https://localhost:7132/.well-known/openid-configuration | jq
```

**3. Decodificar o token:**

Copie o `access_token` e cole em https://jwt.io

Verifique:
- `iss` (issuer): deve ser `https://localhost:7132/`
- `exp` (expiration): não deve estar expirado
- `aud` (audience): deve incluir o servidor API

## ⚠️ Checklist de Diagnóstico

Se ainda estiver com 401, verifique:

- [ ] **Ordem de inicialização**: Auth Server iniciou ANTES do API Server?
- [ ] **Token novo**: Token foi gerado APÓS ambos os servidores estarem rodando?
- [ ] **Token completo**: Token não foi cortado ao copiar?
- [ ] **Token não expirado**: Token foi gerado há menos de 1 hora?
- [ ] **Chaves sincronizadas**: Ambos os servidores estão usando as mesmas chaves?
- [ ] **Issuer correto**: IdentityConfig.cs tem `SetIssuer(new Uri("https://localhost:7132"))`?

## 🔧 Teste Rápido: Token Válido?

Para verificar se o problema é o token ou a validação:

**1. Obter token e decodificar:**
```bash
# Obter token
TOKEN=$(curl -k -s -X POST https://localhost:7132/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "username=admin@admin.com" \
  -d "password=Admin@123" \
  -d "client_id=mobile-client" \
  -d "scope=openid profile email roles api chamados" \
  | jq -r '.access_token')

# Mostrar token
echo $TOKEN

# Decodificar (se for JWT não encriptado)
# Nota: OpenIddict usa JWE (encriptado), então isso pode não funcionar
echo $TOKEN | cut -d. -f2 | base64 -d 2>/dev/null | jq
```

**2. Se o token é JWE (encriptado):**

Tokens OpenIddict são **JWE** (JSON Web Encryption), não JWT simples. Eles têm 5 partes separadas por ponto:

```
<protected>.<encrypted_key>.<iv>.<ciphertext>.<tag>
```

Isso significa que:
- ❌ Você não pode decodificar com jwt.io
- ✅ Apenas o servidor com a chave de decriptação pode ler

## 🎯 Solução Rápida (para desenvolvimento)

**Execute nesta ordem EXATA:**

```powershell
# Terminal 1: Auth Server
cd E:\TI\git\e-chamado\src\EChamado\Echamado.Auth
dotnet clean
dotnet run --launch-profile https

# Terminal 2: API Server (AGUARDE Auth Server iniciar completamente)
cd E:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
dotnet clean
dotnet run --launch-profile https

# Terminal 3: Teste (AGUARDE ambos iniciarem)
cd E:\TI\git\e-chamado
.\test-api-with-token.ps1
```

## 📝 Nota Importante

**Para produção**, você DEVE usar certificados fixos, não chaves de desenvolvimento efêmeras.

---

**Data:** 23/11/2025
**Status:** 🟡 Diagnosticando
**Causa Provável:** Chaves de desenvolvimento efêmeras incompatíveis
**Solução:** Ordem correta de inicialização + token novo
