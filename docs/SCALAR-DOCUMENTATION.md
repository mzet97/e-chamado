# Scalar API Documentation - EChamado

## 📚 Acessando a Documentação da API

O EChamado.Server utiliza o **Scalar** como ferramenta de documentação de APIs, oferecendo uma interface moderna e interativa para explorar todos os endpoints disponíveis.

### 🌐 URLs de Acesso

#### **Servidor de Desenvolvimento (Local)**

- **API Server (EChamado.Server)**: https://localhost:7296
  - **Scalar UI**: https://localhost:7296/api-docs/v1
  - **OpenAPI JSON**: https://localhost:7296/openapi/v1.json
  - **Swagger Redirect**: https://localhost:7296/swagger

- **Auth Server (Echamado.Auth)**: https://localhost:7133
  - **Página Home**: https://localhost:7133/
  - **Login**: https://localhost:7133/Account/Login
  - **Register**: https://localhost:7133/Account/Register

- **Client App (EChamado.Client)**: https://localhost:7274
  - **Dashboard**: https://localhost:7274/
  - **Chamados**: https://localhost:7274/orders

### 🔗 Links Diretos para a Documentação

Para acessar diretamente a documentação da API do EChamado.Server:

```
https://localhost:7296/api-docs/v1
```

### 🚀 Como Iniciar o Servidor

1. **Navegue até o diretório do servidor**:
   ```bash
   cd src/EChamado/Server/EChamado.Server
   ```

2. **Execute o servidor**:
   ```bash
   dotnet run
   ```

3. **Acesse a documentação**:
   - Abra o navegador em: https://localhost:7296/api-docs/v1

### ✨ Funcionalidades do Scalar

A interface do Scalar oferece:

- **📋 Lista Completa de Endpoints**: Todos os endpoints da API organizados por categoria
- **🔍 Busca Avançada**: Pressione `k` para abrir a busca rápida
- **🔐 Autenticação Integrada**: Botão "Authorize" para testar endpoints protegidos
- **📝 Exemplos de Request/Response**: Modelos de dados e exemplos de uso
- **🌙 Modo Escuro**: Tema Purple com suporte a dark mode
- **💻 Geração de Client Code**: Botão para gerar clientes em C#, JavaScript, etc.
- **📊 Models**: Visualização de todos os modelos de dados

### 🔐 Configuração de Autenticação

#### **OAuth 2.0 / OpenIddict**

A API usa autenticação OAuth 2.0 com OpenIddict. Para testar endpoints protegidos:

1. **Clique no botão "Authorize"** na interface do Scalar
2. **Use um dos métodos abaixo para obter o token**:

   **Via cURL (Password Grant)**:
   ```bash
   curl -X POST https://localhost:7133/connect/token \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "grant_type=password" \
     -d "username=admin@admin.com" \
     -d "password=Admin@123" \
     -d "client_id=mobile-client" \
     -d "scope=openid profile email roles api chamados"
   ```

3. **Copie o `access_token`** da resposta
4. **Cole no campo do Scalar** (apenas o token, sem "Bearer")
5. **Clique em "Authorize"**

#### **Usuários de Teste**

- **Admin**: admin@admin.com / Admin@123
- **User**: user@echamado.com / User@123

### 📂 Estrutura da API

A API está organizada nas seguintes seções:

- **🔑 Authentication**: Endpoints de autenticação (OAuth 2.0)
- **🎫 Orders/Chamados**: CRUD completo de chamados
- **📁 Categories**: Gestão de categorias
- **🏢 Departments**: Gestão de departamentos
- **📋 Order Types**: Tipos de chamados
- **✅ Status Types**: Tipos de status
- **📝 Sub Categories**: Subcategorias
- **👥 Users**: Gestão de usuários
- **🔐 Roles**: Gestão de roles/permissões
- **💬 Comments**: Comentários nos chamados

### 🔧 Recursos Avançados

#### **Gridify Endpoints**

Endpoints com filtro dinâmico, ordenação e paginação:

```
GET /v1/orders/gridify?Filter=Title @= "Suporte"&Page=1&PageSize=20&OrderBy=CreatedAt desc
```

#### **OData Endpoints**

Endpoints com suporte a OData para consultas avançadas:

```
GET /odata/Orders?$filter=StatusName eq 'Aberto'&$orderby=CreatedAt desc&$top=20
```

#### **AI Natural Language Query**

Conversão de linguagem natural para queries:

```
POST /v1/ai/nl-to-gridify
Body: {
  "entityName": "Order",
  "query": "Mostrar chamados abertos do TI"
}
```

### 🐛 Solução de Problemas

#### **Erro: "Não é possível acessar a página"**

**Causa**: O servidor não está rodando
**Solução**:
1. Verifique se o servidor está executando na porta 7296
2. Execute: `dotnet run` no diretório `src/EChamado/Server/EChamado.Server`

#### **Erro: "Failed to load API specification"**

**Causa**: O JSON do OpenAPI não está sendo gerado
**Solução**:
1. Verifique se o build foi bem-sucedido: `dotnet build`
2. Confirme que o arquivo XML está sendo gerado: `bin/Debug/net9.0/EChamado.Server.xml`

#### **Erro 401 Unauthorized nos endpoints**

**Causa**: Token de autenticação não fornecido ou inválido
**Solução**:
1. Clique em "Authorize" no Scalar
2. Obtenha um novo token usando as instruções acima
3. Certifique-se de que o token não expirou (24h)

### 📚 Documentação Adicional

- **API Reference Completa**: https://localhost:7296/api-docs/v1
- **OpenAPI Schema**: https://localhost:7296/openapi/v1.json
- **Guia de Autenticação**: `docs/AUTENTICACAO-SISTEMAS-EXTERNOS.md`
- **Exemplos de Uso**: `docs/exemplos-autenticacao-openiddict.md`

### 🎯 Próximos Passos

1. **Explore a API**: Acesse https://localhost:7296/api-docs/v1
2. **Teste a Autenticação**: Use o botão "Authorize" no Scalar
3. **Consulte os Endpoints**: Veja exemplos de requests/responses
4. **Integre em sua Aplicação**: Use os clientes gerados pelo Scalar

### 📞 Suporte

Para dúvidas ou problemas:
- **GitHub**: https://github.com/mzet97/e-chamado
- **Documentação Completa**: Ver arquivo `README.md` na raiz do projeto

---

## 🔥 Destaques

- ✅ **Scalar 1.2.42**: Versão mais recente com interface moderna
- ✅ **OpenAPI 3.0**: Especificação completa e padronizada
- ✅ **OAuth 2.0 + PKCE**: Autenticação segura e moderna
- ✅ **Swagger UI**: Interface alternativa disponível em `/swagger`
- ✅ **XML Comments**: Documentação detalhada em todos os endpoints
- ✅ **Gridify Integration**: Filtros dinâmicos com IA
- ✅ **OData Support**: Consultas avançadas e flexíveis
