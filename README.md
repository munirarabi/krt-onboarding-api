# KRT Onboarding API

API REST desenvolvida em **.NET 8** para gerenciamento de contas de clientes de um banco fictício (KRT - API de Onboarding).

Projeto feito com foco em boas práticas, separação de responsabilidades, regras de domínio e cache.

## Tecnologias utilizadas

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Redis
* Docker
* xUnit
* Swagger Documentação

## Executando o projeto

### Pré-requisitos

É necessário possuir:

* .NET 8 SDK
* SQL Server
* Docker Desktop

O Redis pode ser executado através de Docker.

### 1. Clone o repositório

```bash
git clone <repository-url>
```

Acesse o diretório:

```bash
cd krt-onboarding-api
```

### 2. Configure a conexão com SQL Server (appSettings)

A aplicação espera uma connection string chamada:

```text
DefaultConnection
```

Por segurança, credenciais reais não devem ser versionadas no repositório.

### 3. Inicie o Redis

O projeto contém configuração Docker para Redis.

```bash
docker compose up -d
```

A configuração local utiliza:

```text
localhost:6379
```

A connection string esperada é:

```json
{
  "ConnectionStrings": {
    "KRTOnboarding_SQL": "Server=serverHere,1433;Database=KRTOnboarding;User Id=userHere;Password=passwordHere;TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  }
}
```

### 4. Execute as migrations

```bash
dotnet ef database update --project KRT.Onboarding.Infrastructure --startup-project KRT.Onboarding.Api
```

### 5. Execute a API pelo Visual Studio ou
```bash
dotnet run
```

### 6. Swagger

Com a aplicação executando em ambiente de desenvolvimento, acesse `/swagger` para visualizar e testar

## Endpoints

Principais operações disponíveis (endpoints):

```text
POST    /api/accounts
GET     /api/accounts
GET     /api/accounts/{id}
PUT     /api/accounts/{id}
DELETE  /api/accounts/{id}
```

### Criar conta

Exemplo:

```json
{
  "holderName": "Munir Marques",
  "cpf": "529.982.247-25"
}
```

A conta será criada inicialmente como `Active`.

### Atualizar conta

Exemplo:

```json
{
  "holderName": "Munir Marques",
  "status": 0
}
```

Onde:

```text
0 = Inactive
1 = Active
```

## Arquitetura

A solução foi organizada buscando separar as responsabilidades entre domínio, casos de uso, infraestrutura e camada de apresentação.

```text
KRT.Onboarding
│
├── KRT.Onboarding.Api
├── KRT.Onboarding.Application
├── KRT.Onboarding.Domain
├── KRT.Onboarding.Infrastructure
└── KRT.Onboarding.UnitTests
```

O fluxo principal da aplicação pode ser representado por:

```text
HTTP Request (Cliente -> API)
     │
     ▼
Controller
     │
     ▼
Application Service
     │
     ├──── Domain
     │
     ├──── Repository
     │
     ├──── Cache
     │
     └──── Event Publisher
```

## Cache

Para reduzir consultas repetidas ao banco de dados foi utilizado **Redis**, seguindo a estratégia **Cache-Aside**.

O cache é aplicado na consulta de conta por ID (GetById).

Quando a conta está no cache, a consulta ao banco de dados é evitada.

Quando não está:

1. A aplicação consulta o Redis.
2. O cache retorna `null`.
3. A aplicação consulta o Repository.
4. O resultado é armazenado no Redis.
5. A conta é retornada.

O cache possui expiração absoluta de 1 dia (24 horas, proposta pelo desafio.).

### Invalidação

Operações que modificam os dados invalidam o cache correspondente:

```text
Update Account
     │
     └── Remove account:{id}

Delete Account
     │
     └── Remove account:{id}
```

Isso reduz o risco de retornar informações desatualizadas.

## Princípios aplicados

Durante o desenvolvimento foram considerados conceitos como:

* Clean Code
* SOLID
* DDD
* Dependency Injection
* Value Objects
* Cache-Aside
* Arquitetura de Event-driven 
* Testes unitários

## Autor

**Munir Arabi Marques**
