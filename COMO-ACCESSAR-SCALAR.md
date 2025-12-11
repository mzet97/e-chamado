# 🚀 Como Acessar a Documentação Scalar - EChamado

## ⚠️ IMPORTANTE: O Servidor Deve Estar Rodando

A documentação do Scalar **APENAS será visível** quando o servidor EChamado.Server estiver executando na porta 7296.

---

## 📋 Passo a Passo Completo

### **1️⃣ Iniciar o Servidor**

Abra o terminal/prompt de comando e execute:

```bash
# Navegue até o diretório do servidor
cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server

# Inicie o servidor
dotnet run
```

**✅ Sucesso:** Você verá mensagens como:
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7296
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### **2️⃣ Acessar a Documentação**

Após o servidor estar rodando, abra seu navegador em **UMA DAS URLS**:

#### **🔗 URLs Principais (Escolha uma):**

| URL | Descrição |
|-----|-----------|
| **https://localhost:7296/api-docs/v1** | ⭐ **RECOMENDADA** - Interface Scalar |
| **https://localhost:7296** | Redireciona para /api-docs/v1 |
| **https://localhost:7296/scalar** | Redireciona para /api-docs/v1 |
| **https://localhost:7296/docs** | Redireciona para /api-docs/v1 |
| **https://localhost:7296/swagger** | Redireciona para /api-docs/v1 |

#### **📄 URLs Alternativas:**

| URL | Descrição |
|-----|-----------|
| **https://localhost:7296/openapi/v1.json** | JSON bruto da especificação OpenAPI |
| **https://localhost:7296/health** | Status do servidor (deve retornar `{"status":"ok"}`) |

---

## 🧪 Teste Rápido

### **Teste 1: Verificar se o servidor está rodando**

Abra no navegador: **https://localhost:7296/health**

**✅ Sucesso:**
```json
{
  "status": "ok",
  "service": "EChamado.Server",
  "timestamp": "2025-11-28T10:00:00Z"
}
```

**❌ Erro:** Se receber erro de conexão, o servidor não está rodando.

### **Teste 2: Verificar a documentação**

Abra no navegador: **https://localhost:7296/api-docs/v1**

**✅ Sucesso:** Você verá a interface do Scalar com todos os endpoints da API.

**❌ Erro:** "Esta página não pode ser exibida" = servidor não está rodando.

---

## 🔧 Solução de Problemas

### **❌ "Esta página não pode ser exibida" / "Can't connect"**

**Causa:** Servidor não está executando na porta 7296.

**Solução:**
1. ✅ Certifique-se que está no diretório: `src/EChamado/Server/EChamado.Server`
2. ✅ Execute: `dotnet run`
3. ✅ Aguarde a mensagem: `Now listening on: https://localhost:7296`
4. ✅ Abra: **https://localhost:7296/api-docs/v1**

---

### **❌ "Failed to load API specification"**

**Causa:** Problema na geração do OpenAPI JSON.

**Solução:**
1. ✅ Execute `dotnet build` para verificar erros
2. ✅ Confirme que não há erros de compilação
3. ✅ Verifique se o arquivo XML está sendo gerado:
   - Localização: `bin/Debug/net9.0/EChamado.Server.xml`
4. ✅ Reinicie o servidor: `dotnet run`

---

### **❌ "Unauthorized" nos endpoints**

**Causa:** Token de autenticação não fornecido.

**Solução:**
1. ✅ Na interface do Scalar, clique em **"Authorize"**
2. ✅ Obtenha um token usando cURL:
   ```bash
   curl -X POST https://localhost:7133/connect/token \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "grant_type=password" \
     -d "username=admin@admin.com" \
     -d "password=Admin@123" \
     -d "client_id=mobile-client" \
     -d "scope=openid profile email roles api chamados"
   ```
3. ✅ Copie o `access_token` da resposta
4. ✅ Cole no campo "Token" do Scalar (sem "Bearer")
5. ✅ Clique em "Authorize"

---

### **❌ "Your connection is not private" (HTTPS)**

**Causa:** Certificado de desenvolvimento auto-assinado.

**Solução:**
1. ✅ Clique em "Advanced" ou "Avançado"
2. ✅ Clique em "Proceed to localhost (unsafe)" ou "Prosseguir para localhost (não seguro)"
3. ✅ Ou configure um certificado válido (apenas para produção)

---

## 🎯 URLs de Referência Rápida

### **Serviços do EChamado:**

| Serviço | Porta | URL Base | Documentação |
|---------|-------|----------|--------------|
| **Auth Server** | 7133 | https://localhost:7133 | https://localhost:7133/ |
| **API Server** | 7296 | https://localhost:7296 | **https://localhost:7296/api-docs/v1** |
| **Client App** | 7274 | https://localhost:7274 | https://localhost:7274/ |

### **Credenciais de Teste:**

- **Admin:** admin@admin.com / Admin@123
- **User:** user@echamado.com / User@123

---

## ✨ Funcionalidades do Scalar

Uma vez acessando **https://localhost:7296/api-docs/v1**, você terá:

- ✅ **Lista completa** de todos os endpoints
- ✅ **Busca rápida** (pressione `k`)
- ✅ **Autenticação OAuth 2.0** integrada
- ✅ **Exemplos** de request/response
- ✅ **Geração de cliente** (C#, JavaScript, etc.)
- ✅ **Modelos de dados** visualizáveis
- ✅ **Teste de endpoints** diretamente na interface
- ✅ **Tema escuro** e interface moderna

---

## 📝 Exemplos de Endpoints

### **Gridify (Filtros Dinâmicos):**
```
GET /v1/orders/gridify?Filter=Title @= "Suporte"&Page=1&PageSize=20
```

### **OData (Consultas Avançadas):**
```
GET /odata/Orders?$filter=StatusName eq 'Aberto'&$orderby=CreatedAt desc
```

### **Criar Chamado:**
```
POST /v1/orders
Content-Type: application/json

{
  "title": "Problema no computador",
  "description": "Computador não liga",
  "typeId": "guid-do-tipo",
  "statusId": "guid-do-status",
  "departmentId": "guid-do-departamento"
}
```

---

## 🔄 Resumo de Ações

### **Para acessar rapidamente:**

1. ✅ **Abra o terminal**
2. ✅ **Execute:** `cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server`
3. ✅ **Execute:** `dotnet run`
4. ✅ **Aguarde:** `Now listening on: https://localhost:7296`
5. ✅ **Abra:** **https://localhost:7296/api-docs/v1**

### **Para testar a API:**

1. ✅ **Clique em "Authorize"** no Scalar
2. ✅ **Obtenha o token** via cURL (comando acima)
3. ✅ **Cole o token** no Scalar
4. ✅ **Teste os endpoints** diretamente na interface

---

## 📞 Suporte

Se ainda não conseguir acessar:

1. ✅ **Verifique se o servidor está rodando:** https://localhost:7296/health
2. ✅ **Verifique se o build foi bem-sucedido:** `dotnet build`
3. ✅ **Verifique os logs** no terminal onde executou `dotnet run`
4. ✅ **Confirme a porta:** deve ser 7296 (não 7274, não 7133)

**URLs Corretas:**
- ✅ **CORRETO:** https://localhost:7296/api-docs/v1
- ❌ **ERRADO:** https://localhost:7274/api-docs/v1 (Client App)
- ❌ **ERRADO:** https://localhost:7133/api-docs/v1 (Auth Server)

---

**🎯 Lembre-se: O Scalar só fica visível quando o servidor EChamado.Server está executando na porta 7296!**
