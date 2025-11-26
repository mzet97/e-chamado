# 🚀 Guia de Onboarding para Desenvolvedores

## Primeiros Passos no EChamado

### 🎯 Bem-vindos ao Projeto!

Este guia foi criado para facilitar a entrada de novos desenvolvedores no projeto EChamado. Siga os passos abaixo para ter o ambiente funcionando e começar a contribuir rapidamente.

---

## 📋 Pré-requisitos

### Ferramentas Obrigatórias
- **.NET 9 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Git** - [Download aqui](https://git-scm.com/)
- **Visual Studio 2022** ou **VS Code** (recomendado: VS Code)
- **Docker Desktop** - [Download aqui](https://www.docker.com/products/docker-desktop)
- **PostgreSQL** (opcional - pode usar Docker)

### Extensões Recomendadas (VS Code)
- **C#** (Microsoft)
- **GitLens** 
- **Docker** (Microsoft)
- **Prettier - Code formatter**
- **SonarLint**

---

## 🏗️ 1. Configuração do Ambiente (30 minutos)

### 1.1 Clone do Repositório

```bash
# Clone o repositório
git clone https://github.com/mzet97/e-chamado.git

# Entre no diretório
cd e-chamado/src/EChamado

# Configure seu nome e email no Git
git config user.name "Seu Nome"
git config user.email "seu.email@empresa.com"

# Configure branch principal
git branch --set-upstream-to=origin/main main
```

### 1.2 Ambiente de Desenvolvimento

```bash
# Entre no diretório do projeto
cd src/EChamado

# Execute o script de inicialização (recomendado)
# Linux/Mac
./start-all-projects.sh

# Windows  
.\start-all-projects.ps1
```

### 1.3 Configuração Manual (Alternativa)

Se preferir configurar manualmente:

```bash
# 1. Subir serviços de infraestrutura
docker-compose up -d

# 2. Aguardar os serviços subirem (30-60 segundos)
# 3. Verificar status dos containers
docker-compose ps

# 4. Executar migrations do banco de dados
cd Server/EChamado.Server
dotnet ef database update

# 5. Executar as aplicações em terminais separados:
# Terminal 1 - Auth Server
cd Echamado.Auth
dotnet run

# Terminal 2 - API Server  
cd Server/EChamado.Server
dotnet run

# Terminal 3 - Client
cd Client/EChamado.Client
dotnet run
```

---

## 🌐 2. Acesso à Aplicação

### URLs Principais
- **Aplicação Cliente**: https://localhost:7274
- **Servidor de Auth**: https://localhost:7132
- **API Swagger**: https://localhost:7296/swagger
- **Kibana (Logs)**: http://localhost:5601

### Credenciais de Teste
```
Admin:
  Email: admin@echamado.com
  Senha: Admin@123

Usuário:
  Email: user@echamado.com  
  Senha: User@123
```

---

## 🎓 3. Aprendizado Inicial (2-3 horas)

### 3.1 Arquitetura do Projeto

**Leia primeiro (nesta ordem):**
1. **[Visão Geral da Arquitetura](../architecture/overview.md)** - 30 min
2. **[Diagrama de Classes](../architecture/class-diagram.md)** - 45 min
3. **[Casos de Uso](../architecture/use-cases.md)** - 45 min

### 3.2 Estrutura de Pastas

```
src/EChamado/
├── Server/                     # Backend API
│   ├── EChamado.Server/        # API REST Endpoints
│   ├── EChamado.Server.Application/  # CQRS (Commands/Queries)
│   ├── EChamado.Server.Domain/       # Entidades e Domínio
│   └── EChamado.Server.Infrastructure/ # Repos e Persistência
├── Client/                     # Frontend Blazor WASM
│   └── EChamado.Client/        # Interface do usuário
├── Echamado.Auth/             # Servidor de autenticação
└── EChamado.Shared/           # DTOs e tipos compartilhados
```

### 3.3 Tecnologias Principais

| Tecnologia | Uso | Learning Curve |
|------------|-----|----------------|
| **Clean Architecture** | Separação de camadas | Médio |
| **CQRS (Paramore.Brighter)** | Commands/Queries separadas | Médio |
| **Blazor WebAssembly** | Frontend | Médio |
| **Entity Framework Core** | ORM | Baixo |
| **OpenIddict** | Autenticação OIDC | Alto |
| **Docker** | Containerização | Baixo |

---

## 🔧 4. Primeiro Commit (1 hora)

### 4.1 Tarefa Sugerida: Corrigir Documentação

**Objetivo**: Fazer seu primeiro commit documentando algo que você aprendeu.

**Passos:**
1. Abra o arquivo `docs/onboarding/first-steps.md`
2. Adicione uma seção com sua experiência inicial
3. Faça commit das mudanças

```bash
# Fazer mudanças no arquivo
git add .
git commit -m "docs: adicionar experiência inicial de onboarding

- Documentar primeira impressão da arquitetura
- Adicionar notas sobre configuração do ambiente
- Incluir dicas para novos desenvolvedores

Closes #123"

git push origin main
```

### 4.2 Tarefa Alternativa: Teste Local

**Execute os testes para verificar se tudo está funcionando:**

```bash
# Testes unitários
dotnet test EChamado.Server.UnitTests

# Testes de integração  
dotnet test EChamado.Server.IntegrationTests

# Testes E2E
cd Tests/EChamado.E2E.Tests
dotnet test
```

---

## 📚 5. Roadmap de Aprendizado

### Semana 1: Fundamentos
- [x] **Dia 1**: Configuração do ambiente + primeiro commit
- [x] **Dia 2**: Entender arquitetura Clean Architecture
- [x] **Dia 3**: Estudar CQRS e Mediator pattern
- [x] **Dia 4**: Explorar Frontend Blazor + MudBlazor
- [x] **Dia 5**: Primeiro bug fix pequeno

### Semana 2: Desenvolvimento
- [x] **Dia 6-7**: Implementar feature simples (endpoint + UI)
- [x] **Dia 8-9**: Trabalhar em testes unitários
- [x] **Dia 10**: Participar de code review

### Semana 3: Autonomia
- [x] **Dia 11-13**: Implementar feature completa (end-to-end)
- [x] **Dia 14-15**: Mentorear próximo desenvolvedor

---

## 🛠️ 6. Comandos Essenciais

### Desenvolvimento Diário

```bash
# Iniciar ambiente completo
./start-all-projects.sh

# Apenas subir serviços de infraestrutura
docker-compose up -d postgres redis rabbitmq elasticsearch kibana logstash

# Executar migrations
dotnet ef database update

# Executar testes
dotnet test

# Build do projeto
dotnet build

# Limpar e rebuild
dotnet clean && dotnet build

# Executar apenas backend
cd Server/EChamado.Server && dotnet run

# Executar apenas frontend  
cd Client/EChamado.Client && dotnet run
```

### Debug e Troubleshooting

```bash
# Ver logs dos containers
docker-compose logs -f

# Ver logs específicos
docker-compose logs -f api-server
docker-compose logs -f postgres

# Resetar banco de dados
docker-compose stop postgres
docker volume rm echamado_postgres_data
docker-compose up -d postgres
dotnet ef database update

# Limpar cache Redis
docker-compose exec redis redis-cli FLUSHALL
```

---

## 📖 7. Recursos de Aprendizado

### Documentação Interna
- **[Arquitetura](../architecture/)** - Documentação técnica completa
- **[Casos de Uso](../architecture/use-cases.md)** - Cenários de negócio
- **[API Documentation](https://localhost:7296/swagger)** - Swagger da API
- **[Health Checks](https://localhost:7296/health)** - Status dos serviços

### Recursos Externos
- **[Clean Architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html)** - Uncle Bob
- **[CQRS](https://martinfowler.com/bliki/CQRS.html)** - Martin Fowler
- **[Blazor Documentation](https://docs.microsoft.com/pt-br/aspnet/core/blazor/)** - Microsoft
- **[EF Core](https://docs.microsoft.com/pt-br/ef/core/)** - Entity Framework

### Vídeos e Tutoriais
- **YouTube**: "Clean Architecture .NET"
- **Pluralsight**: "Building Microservices with .NET"
- **Microsoft Learn**: Blazor learning path

---

## 🤝 8. Como Obter Ajuda

### Durante o Onboarding
1. **Slack/Teams**: Canal #echamado-dev
2. **Mentor Assignado**: [Nome do mentor]
3. **Documentação**: Consulte primeiro
4. **Issues**: Abra uma issue se encontrar problemas

### Quando Pedir Ajuda
- **Antes de pedir**: Tente resolver por 30 minutos
- **Informações**: Inclua contexto, logs, screenshots
- **Formato**: Use template de issue

### Code Review
- **Branch Strategy**: `feature/descricao-curta`
- **PR Size**: Máximo 400 linhas alteradas
- **Reviewer**: Pelo menos 1 approval necessário
- **CI**: Todos os testes devem passar

---

## ✅ 9. Checklist do Primeiro Dia

### Setup Técnico
- [ ] .NET 9 SDK instalado
- [ ] Git configurado com nome e email
- [ ] Docker Desktop funcionando
- [ ] Projeto clonado e compilando
- [ ] Serviços subindo corretamente
- [ ] Aplicação acessível no browser

### Aprendizado
- [ ] Arquitetura geral entendida
- [ ] Estrutura de pastas explorada
- [ ] Primeiro commit realizado
- [ ] Testes executados com sucesso
- [ ] Documentação inicial lida

### Social
- [ ] Apresentação no Slack/Teams
- [ ] Mentor contatado
- [ ] Times relevantes adicionados
- [ ] Agenda da próxima semana confirmada

---

## 🎯 10. Primeira Semana: Objetivos

### Objetivos Técnicos
- [ ] Configurar ambiente completo
- [ ] Entender arquitetura Clean Architecture
- [ ] Implementar primeiro endpoint
- [ ] Escrever primeiro teste unitário
- [ ] Fazer primeiro code review

### Objetivos Sociais
- [ ] Conhecer toda a equipe
- [ ] Participar de pelo menos 1 daily
- [ ] Ir a 1 reunião de planejamento
- [ ] Fazer pair programming com mentor

---

## 📞 Contatos Importantes

### Time do Projeto
- **Tech Lead**: [Nome] - [email@empresa.com]
- **Mentor Onboarding**: [Nome] - [email@empresa.com]
- **Product Owner**: [Nome] - [email@empresa.com]

### Times de Suporte
- **DevOps**: #devops-team
- **QA**: #qa-team  
- **Product**: #product-team

---

## 🎉 Parabéns!

Você completou o onboarding inicial do EChamado! Agora você está pronto para começar a contribuir para o projeto.

### Próximos Passos
1. **Escolha uma tarefa** do backlog
2. **Crie uma branch** seguindo nossa convenção
3. **Implemente a feature** seguindo nossos padrões
4. **Escreva testes** para sua implementação
5. **Faça um PR** para code review

### Lembre-se
- **Pergunte sempre** quando tiver dúvidas
- **Documente** o que você aprender
- **Compartilhe** conhecimento com outros
- **Celebre** suas conquistas

---

**Bem-vindos ao time! 🚀**

---

**Última atualização:** 26 de novembro de 2025  
**Versão:** 1.0.0  
**Status:** ✅ Guia validado por novos desenvolvedores