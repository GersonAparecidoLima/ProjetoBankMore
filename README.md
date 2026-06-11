# BankMore API 🚀

API bancária desenvolvida em **.NET 8** utilizando **Clean Architecture**, **DDD**, **CQRS**, **MediatR**, **Dapper**, **JWT Authentication**, **SQL Server**, **Docker** e **xUnit**.

O projeto simula o núcleo de uma instituição financeira (*Core Banking*), com foco em segurança, consistência transacional, desacoplamento entre camadas e boas práticas de engenharia de software.

---

# 🛠️ Tecnologias Utilizadas

* .NET 8
* ASP.NET Core Web API
* Dapper
* SQL Server
* JWT Authentication
* MediatR
* CQRS
* DDD (Domain-Driven Design)
* Docker / Docker Compose
* Swagger / OpenAPI
* xUnit
* Moq

---

# 📐 Arquitetura

O projeto segue os princípios de **DDD (Domain-Driven Design)** e **Clean Architecture**, promovendo baixo acoplamento e alta coesão entre as camadas.

```text
BankMore.Api
├── Controllers
├── Services
├── Program.cs

BankMore.Application
├── Commands
├── Queries
├── Handlers

BankMore.Domain
├── Entities
├── Interfaces
├── Rules

BankMore.Infrastructure
├── Repositories
├── Data
├── Persistence

BankMore.Tests
├── Unit Tests
```

### Padrões Utilizados

* CQRS (Command Query Responsibility Segregation)
* Repository Pattern
* Dependency Injection
* Dependency Inversion Principle (SOLID)
* Transaction Script para operações financeiras
* JWT Authentication

---

# 🔒 Segurança

A API implementa autenticação e autorização utilizando **JWT (JSON Web Token)**.

### Fluxo de autenticação

1. O usuário realiza login.
2. A senha é validada utilizando SHA256 + Salt.
3. Um token JWT é gerado.
4. Os endpoints financeiros exigem Bearer Token.
5. Requisições sem token retornam HTTP 401 Unauthorized.

### Proteções implementadas

* Senhas armazenadas com Hash SHA256 + Salt
* Autorização via JWT
* Endpoints protegidos com `[Authorize]`
* Uso de GUIDs como identificadores públicos
* Validação de saldo antes de saques e transferências

---

# 🏦 Funcionalidades Implementadas

### Cadastro de Usuários

* Criação de conta corrente
* Validação de documento
* Armazenamento seguro de credenciais

### Autenticação

* Login com validação de senha
* Geração de Token JWT

### Operações Financeiras

* Depósito
* Saque
* Transferência entre contas
* Consulta de saldo
* Consulta de extrato

### Garantia de Qualidade

* Testes unitários com xUnit
* Mocks utilizando Moq
* Cobertura das principais regras de negócio

---

# 📡 Principais Endpoints

### Login

```http
POST /api/auth/login
```

Request:

```json
{
  "identificador": "52277",
  "senha": "MinhaSenha123"
}
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

---

### Consultar Saldo

```http
GET /api/accounts/{id}/saldo
```

Header:

```text
Authorization: Bearer {token}
```

---

### Consultar Extrato

```http
GET /api/accounts/{id}/extrato
```

Header:

```text
Authorization: Bearer {token}
```

---

# 📝 Decisões Arquiteturais

### Idempotência

As transferências recebem uma chave de idempotência para evitar duplicidade em cenários de reenvio da mesma requisição.

### Consistência Transacional

As operações financeiras são executadas dentro de transações atômicas utilizando Dapper, garantindo rollback automático em caso de falha.

### Aplicação Stateless

A API não mantém estado em memória, permitindo escalabilidade horizontal e futura execução em ambientes Kubernetes sem alterações no código.

---

# 🚀 Executando o Projeto

## Pré-requisitos

* Docker
* Docker Compose

## Clonar o repositório

```bash
git clone https://github.com/GersonAparecidoLima/ProjetoBankMore.git
```

## Executar containers

```bash
docker-compose up --build
```

O Docker iniciará:

* SQL Server
* API BankMore

---

# 📚 Documentação

Após a inicialização dos containers:

🌐 Swagger UI

```text
http://localhost:5044/swagger
```

---

# ✅ Status do Projeto

Projeto funcional com:

* JWT Authentication
* CQRS + MediatR
* Dapper
* SQL Server
* Docker
* Testes Unitários
* Arquitetura em Camadas
* Aplicação dos princípios SOLID
