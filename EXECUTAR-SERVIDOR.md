# 🚀 EXECUTAR E ACESSAR O SCALAR - PASSO A PASSO

## ⚠️ IMPORTANTE: O SERVIDOR DEVE ESTAR RODANDO

A documentação do Scalar **NÃO VAI APARECER** se o servidor não estiver executando.

---

## 📋 PASSO A PASSO SIMPLES

### **1️⃣ ABRA O TERMINAL/PROMPT DE COMANDO**

**Windows:**
- Pressione `Win + R`
- Digite `cmd` e pressione Enter

**OU**

- Clique com o botão direito na pasta `e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server`
- Selecione "Open in Terminal" ou "Abrir no Terminal"

---

### **2️⃣ NAVEGUE ATÉ O DIRETÓRIO DO SERVIDOR**

```bash
cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
```

---

### **3️⃣ EXECUTE O SERVIDOR**

```bash
dotnet run
```

---

### **4️⃣ AGUARDE A MENSAGEM DE SUCESSO**

**Você verá algo assim:**
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7296
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**⚠️ IMPORTANTE:** A mensagem "Now listening on: https://localhost:7296" DEVE aparecer!

---

### **5️⃣ ABRA O NAVEGADOR**

**Acesse uma das URLs:**

| URL | Descrição |
|-----|-----------|
| **https://localhost:7296** | ⭐ **MAIS SIMPLES** - Página inicial que redireciona |
| **https://localhost:7296/api-docs/v1** | Interface do Scalar |
| **https://localhost:7296/health** | Verificar se o servidor está funcionando |

---

## 🧪 TESTE SE O SERVIDOR ESTÁ RODANDO

### **Teste 1: Health Check**

1. Com o servidor rodando, abra uma nova aba do navegador
2. Digite: `https://localhost:7296/health`
3. **Resultado esperado:**
   ```json
   {
     "status": "ok",
     "service": "EChamado.Server",
     "timestamp": "2025-11-28T..."
   }
   ```

### **Teste 2: Documentação**

1. Digite: `https://localhost:7296`
2. **Resultado esperado:** Interface do Scalar com todos os endpoints da API

---

## 🔧 SOLUÇÃO DE PROBLEMAS

### **❌ "Can't connect" / "Esta página não pode ser exibida"**

**Causa:** Servidor não está rodando na porta 7296.

**Solução:**
1. Verifique se você está no diretório correto: `e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server`
2. Execute: `dotnet run`
3. Aguarde a mensagem: "Now listening on: https://localhost:7296"
4. Abra: `https://localhost:7296`

---

### **❌ "Não é possível localizar o projeto"**

**Causa:** Você não está no diretório correto do projeto.

**Solução:**
1. Navegue até o diretório: `cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server`
2. Execute: `dotnet run`

---

### **❌ "Your connection is not private" (Erro de SSL)**

**Causa:** Certificado de desenvolvimento auto-assinado.

**Solução:**
1. Clique em "Advanced" (Avançado)
2. Clique em "Proceed to localhost (unsafe)" (Prosseguir para localhost - não seguro)

---

### **❌ "Failed to load API specification"**

**Causa:** Erro de build ou compilação.

**Solução:**
1. Pare o servidor (Ctrl + C)
2. Execute: `dotnet build`
3. Se houver erros, corrija-os
4. Execute novamente: `dotnet run`

---

## 🎯 RESUMO RÁPIDO

### **Para executar o servidor:**

1. **Abra o terminal**
2. **Execute:**
   ```bash
   cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
   dotnet run
   ```
3. **Aguarde:** `Now listening on: https://localhost:7296`
4. **Acesse:** `https://localhost:7296`

### **Para acessar a documentação:**

- **Página inicial:** `https://localhost:7296`
- **Interface Scalar:** `https://localhost:7296/api-docs/v1`
- **Health check:** `https://localhost:7296/health`

---

## 📞 SE AINDA NÃO FUNCIONAR

### **Verifique se o servidor está rodando:**

1. **Abra o navegador**
2. **Digite:** `https://localhost:7296/health`
3. **Se retornar JSON:** O servidor está rodando
4. **Se der erro:** O servidor não está rodando

### **Se o servidor não estiver rodando:**

1. **Pare o terminal** (Ctrl + C)
2. **Execute novamente:**
   ```bash
   cd e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server
   dotnet run
   ```
3. **Aguarde** a mensagem "Now listening on: https://localhost:7296"
4. **Acesse:** `https://localhost:7296`

---

## 🔑 CREDENCIAIS DE TESTE

- **Admin:** admin@admin.com / Admin@123
- **User:** user@echamado.com / User@123

---

## ✨ FUNCIONALIDADES DO SCALAR

Uma vez acessando `https://localhost:7296`, você terá:

- ✅ **Lista completa** de endpoints
- ✅ **Busca rápida** (pressione `k`)
- ✅ **Autenticação** integrada (botão "Authorize")
- ✅ **Exemplos** de request/response
- ✅ **Geração de clientes** (C#, JavaScript, etc.)
- ✅ **Modelos de dados**
- ✅ **Teste de endpoints** diretamente na interface

---

**🎯 IMPORTANTE: O SERVIDOR DEVE ESTAR RODANDO PARA ACESSAR A DOCUMENTAÇÃO!**

**Para executar:** `dotnet run` em `e:\TI\git\e-chamado\src\EChamado\Server\EChamado.Server`

**Para acessar:** `https://localhost:7296`
