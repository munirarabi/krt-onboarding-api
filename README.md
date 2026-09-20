# KRT Onboarding API

API REST desenvolvida em **.NET 8** para gerenciamento de contas de clientes de um banco fictício (Onboarding).

Desafio técnico, com foco em boas práticas, separação de responsabilidades, regras de domínio, cache, arquitetura orientada a eventos e testes unitários.

## Tecnologias utilizadas

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Redis
* Docker
* xUnit
* Moq
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

### 2. Configure a conexão com SQL Server

A aplicação espera uma connection string chamada:

```text
DefaultConnection
```

Por segurança, credenciais reais não devem ser versionadas no repositório.

É recomendado utilizar User Secrets durante o desenvolvimento local.

Exemplo:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<sua-connection-string>" --project KRT.Onboarding.Api
```

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
    "Redis": "localhost:6379"
  }
}
```

### 4. Execute as migrations

```bash
dotnet ef database update --project KRT.Onboarding.Infrastructure --startup-project KRT.Onboarding.Api
```

### 5. Execute a API

```bash
dotnet run --project KRT.Onboarding.Api
```

### 6. Swagger

Com a aplicação executando em ambiente de desenvolvimento, acesse o endpoint `/swagger` disponibilizado pela API para visualizar e testar os endpoints.

## Endpoints

Principais operações disponíveis:

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

### Domain

Contém as regras e objetos centrais do domínio da aplicação.

Principais elementos:

* `Account`
* `AccountStatus`
* `Cpf`
* `HolderName`
* `DomainException`
* Eventos de conta

O domínio não possui dependência das demais camadas.

### Application

Responsável pelos casos de uso e contratos necessários para execução das regras da aplicação.

Principais elementos:

* `AccountService`
* `AccountDto`
* `IAccountRepository`
* `IAccountCacheService`
* `IEventPublisher`
* Mapeamentos
* Exceptions da aplicação

A camada Application depende do Domain, mas não conhece detalhes de SQL Server, Redis ou tecnologias externas.

### Infrastructure

Responsável pelas implementações relacionadas a recursos externos.

Principais responsabilidades:

* Entity Framework Core
* SQL Server
* Implementação do `IAccountRepository`
* Redis
* Implementação do `IAccountCacheService`
* Implementação do `IEventPublisher`
* Migrations

### API

Responsável pela exposição HTTP da aplicação.

Contém:

* Controllers
* Requests
* Responses
* Middleware global de exceptions
* Configuração de Dependency Injection
* Swagger

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

## Funcionalidades

A API disponibiliza operações para:

* Criar uma conta
* Listar contas
* Consultar uma conta por ID
* Atualizar uma conta
* Excluir uma conta

Uma conta possui:

* `Id`
* `HolderName`
* `Cpf`
* `Status`

Os status disponíveis atualmente são:

```text
Inactive = 0
Active   = 1
```

Toda nova conta é criada inicialmente com status `Active`.

## Regras de domínio

### CPF

O CPF foi implementado como um Value Object.

Durante a criação:

* O valor é normalizado para conter somente números.
* Deve possuir 11 dígitos.
* CPFs formados pelo mesmo dígito são rejeitados.
* Os dígitos verificadores são validados.
* O CPF é armazenado sem pontuação.

Exemplo:

```text
529.982.247-25
```

é normalizado para:

```text
52998224725
```

A validação dos dígitos verificadores foi adotada como uma premissa de negócio adicional para aumentar a consistência dos dados.

O CPF também possui índice `UNIQUE` no banco de dados, garantindo a unicidade mesmo fora do fluxo normal da aplicação.

### Nome do titular

`HolderName` também foi implementado como Value Object.

As principais regras são:

* Campo obrigatório.
* Mínimo de 2 caracteres.
* Máximo de 150 caracteres.
* Deve conter letras.
* Não permite números.
* Suporta caracteres acentuados.
* Permite apóstrofo, hífen e ponto.
* Espaços adicionais são normalizados.

### Status

O status da conta é representado pelo enum `AccountStatus`.

Além da validação realizada pelo Domain, existe uma `CHECK CONSTRAINT` no SQL Server:

```sql
[Status] IN (0, 1)
```

Dessa forma, existem duas camadas de proteção:

```text
Domain
  └── impede estados inválidos pela aplicação

SQL Server
  └── impede persistência de valores inválidos diretamente no banco
```

## Persistência

A persistência utiliza **SQL Server** com **Entity Framework Core**.

O acesso aos dados é abstraído através de:

```text
IAccountRepository
```

e implementado na camada Infrastructure.

Consultas somente de leitura utilizam `AsNoTracking()` quando apropriado, evitando tracking desnecessário pelo Entity Framework.

As alterações estruturais do banco são controladas através de migrations.

Para aplicar as migrations:

```bash
dotnet ef database update --project KRT.Onboarding.Infrastructure --startup-project KRT.Onboarding.Api
```

## Cache

Para reduzir consultas repetidas ao banco de dados foi utilizado **Redis**, seguindo a estratégia **Cache-Aside**.

O cache é aplicado na consulta de conta por ID.

Fluxo:

```text
GET /api/accounts/{id}
          │
          ▼
     Consulta Redis
       /       \
    HIT         MISS
     │            │
     │            ▼
     │       SQL Server
     │            │
     │            ▼
     │       Salva no Redis
     │            │
     └────────────┘
          │
          ▼
       Response
```

Quando a conta está no cache, a consulta ao banco de dados é evitada.

Quando não está:

1. A aplicação consulta o Redis.
2. O cache retorna `null`.
3. A aplicação consulta o Repository.
4. O resultado é armazenado no Redis.
5. A conta é retornada.

As chaves seguem o padrão:

```text
account:{id}
```

O cache possui expiração absoluta de 1 dia.

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

### Por que `GetAll` não utiliza cache?

O cache foi aplicado especificamente ao cenário de consultas repetidas da mesma conta.

A consulta por ID possui uma chave previsível e uma estratégia simples de invalidação.

O cache de listagens adicionaria maior complexidade, principalmente considerando possíveis evoluções como:

* paginação;
* filtros;
* ordenação;
* diferentes combinações de consulta.

Por isso, foi priorizada uma solução direcionada ao requisito apresentado, evitando adicionar complexidade sem uma necessidade concreta.

## Eventos e mensageria

As operações de criação, atualização e exclusão geram eventos representando alterações realizadas nas contas.

Eventos existentes:

```text
AccountCreatedEvent
AccountUpdatedEvent
AccountDeletedEvent
```

A Application não depende diretamente de uma tecnologia específica de mensageria.

A publicação é abstraída através de:

```csharp
IEventPublisher
```

Isso permite que uma implementação externa seja adicionada sem alterar o `AccountService`.

Por exemplo:

```text
AccountService
      │
      ▼
IEventPublisher
      │
      ├── RabbitMQ
      ├── AWS SQS/SNS
      └── outro broker
```

### Implementação atual

Neste projeto, `EventPublisher` é uma implementação simplificada que não realiza comunicação com um broker externo.

O objetivo desta implementação é demonstrar a separação arquitetural e o ponto em que os eventos seriam publicados, sem adicionar uma infraestrutura de mensageria que não era necessária para execução local do desafio.

Portanto, o projeto **não deve ser interpretado como possuindo integração real com RabbitMQ, SQS ou outro broker**.

### Tratamento de falhas

A publicação dos eventos ocorre após a operação principal da conta.

O código está preparado para registrar uma eventual falha de publicação sem retornar erro ao cliente depois que a operação principal já tiver sido concluída.

Em um cenário de produção com necessidade de garantia de entrega, uma evolução recomendada seria a implementação do padrão **Transactional Outbox**, juntamente com processamento assíncrono e política de retry.

Exemplo:

```text
Transaction
   │
   ├── Account
   │
   └── OutboxMessage
          │
        COMMIT
          │
          ▼
   Background Worker
          │
          ▼
       Broker
```

Isso permitiria maior confiabilidade entre persistência e publicação dos eventos.

## Tratamento de erros

A API possui um middleware global para tratamento de exceptions.

Exemplos:

```text
NotFoundException  → 404 Not Found
ConflictException  → 409 Conflict
DomainException    → 400 Bad Request
Erro inesperado    → 500 Internal Server Error
```

Erros inesperados são registrados através de `ILogger`, enquanto detalhes internos da exception não são expostos ao cliente.

Exemplo de resposta:

```json
{
  "status": 404,
  "message": "Account with ID '...' was not found."
}
```

## Testes unitários

Os testes estão localizados em:

```text
KRT.Onboarding.UnitTests
│
├── Domain
│   ├── Entities
│   │   └── AccountTests.cs
│   └── ValueObjects
│       ├── CpfTests.cs
│       └── HolderNameTests.cs
│
└── Application
    └── Services
        └── AccountServiceTests.cs
```

São utilizados:

* xUnit
* Moq
* FluentAssertions

Os testes seguem o padrão:

```text
Arrange
Act
Assert
```

### Domain

Os testes de domínio validam regras como:

* CPF válido e inválido.
* Normalização do CPF.
* Dígitos repetidos.
* Validação do nome.
* Normalização de espaços.
* Limites de tamanho.
* Criação de uma conta ativa.
* Geração do identificador.
* Alteração do nome.
* Alteração e validação do status.

### Application

Os testes do `AccountService` utilizam mocks para isolar dependências externas.

Dessa forma, os testes não precisam de:

* SQL Server;
* Redis;
* Docker;
* broker de mensagens.

Entre os cenários testados estão:

```text
CreateAsync
├── criação de conta
└── tentativa de CPF duplicado

GetByIdAsync
├── cache hit
├── cache miss
└── conta inexistente

UpdateAsync
├── atualização
├── invalidação do cache
└── publicação de evento

DeleteAsync
├── exclusão
├── invalidação do cache
└── publicação de evento
```

Um cenário importante é o cache hit:

```text
AccountService
     │
     ▼
Cache → encontrou
     │
     ▼
retorna resultado

Repository → não é chamado
```

Esse comportamento é verificado através do Moq, garantindo que o Repository não seja consultado quando o dado já estiver disponível no cache.

Para executar os testes:

```bash
dotnet test
```

## Decisões técnicas

Algumas decisões tomadas durante o desenvolvimento:

**Value Objects para CPF e nome**

As regras relacionadas aos valores permanecem no Domain em vez de ficarem espalhadas entre Controller, Service e banco de dados.

**CPF imutável na atualização**

A atualização da conta permite alteração do nome e status, mantendo o CPF como identificador de negócio imutável no fluxo atual.

**Redis somente no `GetById`**

O cache foi aplicado ao cenário diretamente relacionado às consultas repetidas da mesma conta, evitando complexidade desnecessária em listagens.

**Abstração para publicação de eventos**

`IEventPublisher` evita acoplamento da Application a RabbitMQ, AWS ou outra tecnologia específica.

**Implementação simplificada da mensageria**

Uma integração real com broker não foi adicionada. A arquitetura está preparada para receber uma implementação posteriormente.

**Integridade também no banco**

Além das regras do Domain, restrições importantes são reforçadas no SQL Server, como CPF único e valores válidos para status.

## Possíveis evoluções

Para uma aplicação em produção, algumas evoluções possíveis seriam:

* Transactional Outbox.
* Broker real de mensagens, como RabbitMQ ou AWS SQS/SNS.
* Retry e Dead Letter Queue.
* Testes de integração.
* Testes de API.
* Health Checks para SQL Server e Redis.
* Observabilidade e métricas.
* Autenticação e autorização.
* Paginação e filtros no `GetAll`.
* CI/CD.
* Containerização completa da API e banco de dados.

## Princípios aplicados

Durante o desenvolvimento foram considerados conceitos como:

* Clean Code
* SOLID
* DDD
* Dependency Injection
* Repository Pattern
* Value Objects
* Cache-Aside
* Event-driven architecture
* Separation of Concerns
* Testes unitários

## Autor

**Munir Marques**
