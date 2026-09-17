# \# KRT Onboarding API

# 

# API desenvolvida em .NET 8 para gerenciamento de contas de clientes do banco fictício KRT.

# 

# O projeto foi desenvolvido como parte de um desafio técnico, com foco na organização do código, separação de responsabilidades, boas práticas e facilidade de manutenção.

# 

# \## Tecnologias utilizadas

# 

# \* .NET 8

# \* ASP.NET Core Web API

# \* Entity Framework Core

# \* SQL Server

# \* Swagger / OpenAPI

# \* xUnit

# \* Redis \*(cache)\*

# \* Mensageria \*(eventos de conta)\*

# 

# \## Funcionalidades

# 

# A API permite realizar o gerenciamento de contas através das seguintes operações:

# 

# \* Criar uma conta

# \* Consultar todas as contas

# \* Consultar uma conta por ID

# \* Atualizar uma conta

# \* Excluir uma conta

# 

# Cada conta possui:

# 

# \* ID

# \* Nome do titular

# \* CPF

# \* Status (Ativa/Inativa)

# 

# \## Estrutura do projeto

# 

# A solução foi dividida em camadas para manter as responsabilidades separadas:

# 

# ```text

# KRT.Onboarding

# │

# ├── src

# │   ├── KRT.Onboarding.Api

# │   ├── KRT.Onboarding.Application

# │   ├── KRT.Onboarding.Domain

# │   └── KRT.Onboarding.Infrastructure

# │

# └── tests

# &#x20;   └── KRT.Onboarding.UnitTests

# ```

# 

# \### Domain

# 

# Contém as regras e os objetos principais do domínio.

# 

# Entre eles:

# 

# \* `Account`

# \* `Cpf`

# \* `AccountStatus`

# \* Exceções de domínio

# 

# O CPF foi implementado como Value Object para centralizar sua normalização e validação.

# 

# \### Application

# 

# Responsável pelos casos de uso da aplicação e pelos contratos utilizados pelas demais camadas.

# 

# Nesta camada estão, por exemplo:

# 

# \* `IAccountService`

# \* `IAccountRepository`

# \* `AccountService`

# \* DTOs

# 

# \### Infrastructure

# 

# Responsável pelo acesso a recursos externos utilizados pela aplicação.

# 

# Inclui:

# 

# \* Entity Framework Core

# \* SQL Server

# \* Implementação dos repositories

# \* Configurações das entidades

# \* Migrations

# \* Cache

# \* Mensageria

# 

# \### API

# 

# Camada responsável por receber as requisições HTTP e disponibilizar os endpoints da aplicação.

# 

# Os controllers foram mantidos com pouca responsabilidade, deixando regras e casos de uso para as camadas apropriadas.

# 

# \## Endpoints

# 

# | Método | Endpoint             | Descrição                 |

# | ------ | -------------------- | ------------------------- |

# | POST   | `/api/accounts`      | Cria uma nova conta       |

# | GET    | `/api/accounts`      | Retorna todas as contas   |

# | GET    | `/api/accounts/{id}` | Retorna uma conta pelo ID |

# | PUT    | `/api/accounts/{id}` | Atualiza uma conta        |

# | DELETE | `/api/accounts/{id}` | Exclui uma conta          |

# 

# \## Banco de dados

# 

# Foi utilizado SQL Server com Entity Framework Core.

# 

# As alterações do banco são controladas através de migrations.

# 

# Para criar ou atualizar o banco local:

# 

# ```bash

# dotnet ef database update --project KRT.Onboarding.Infrastructure --startup-project KRT.Onboarding.Api

# ```

# 

# \## Configuração da conexão

# 

# A connection string não é armazenada no repositório.

# 

# Para desenvolvimento local, é possível utilizar o User Secrets do .NET.

# 

# No projeto `KRT.Onboarding.Api`:

# 

# ```bash

# dotnet user-secrets init

# ```

# 

# Depois configure sua conexão:

# 

# ```bash

# dotnet user-secrets set "ConnectionStrings:DefaultConnection" "SUA\_CONNECTION\_STRING"

# ```

# 

# Exemplo de connection string para SQL Server:

# 

# ```text

# Server=localhost;Database=KRTOnboarding;User Id=sa;Password=SUA\_SENHA;TrustServerCertificate=True;

# ```

# 

# As credenciais utilizadas localmente não devem ser adicionadas ao Git.

# 

# \## Executando o projeto

# 

# Clone o repositório e restaure as dependências:

# 

# ```bash

# dotnet restore

# ```

# 

# Configure a connection string utilizando User Secrets e aplique as migrations:

# 

# ```bash

# dotnet ef database update --project KRT.Onboarding.Infrastructure --startup-project KRT.Onboarding.Api

# ```

# 

# Depois execute a API:

# 

# ```bash

# dotnet run --project KRT.Onboarding.Api

# ```

# 

# A documentação dos endpoints pode ser acessada através do Swagger durante a execução da aplicação.

# 

# \## Validações e tratamento de erros

# 

# As validações foram distribuídas de acordo com a responsabilidade de cada camada.

# 

# Validações relacionadas ao domínio ficam concentradas nas entidades e Value Objects, enquanto erros conhecidos da aplicação são tratados de forma centralizada.

# 

# A API possui tratamento global de exceções para evitar a necessidade de `try/catch` em cada controller.

# 

# Alguns retornos utilizados:

# 

# \* `400 Bad Request` para dados inválidos

# \* `404 Not Found` quando uma conta não é encontrada

# \* `409 Conflict` em situações de conflito, como CPF já cadastrado

# \* `500 Internal Server Error` para erros inesperados

# 

# Detalhes internos de erros inesperados não são retornados para o consumidor da API.

# 

# \## Cache

# 

# Para reduzir consultas repetidas ao banco de dados, a solução utiliza cache para consultas de contas.

# 

# A estratégia adotada é Cache-Aside:

# 

# 1\. A aplicação verifica se a conta está disponível no cache.

# 2\. Caso esteja, retorna o dado armazenado.

# 3\. Caso não esteja, consulta o banco de dados e adiciona o resultado ao cache.

# 4\. Alterações ou exclusões invalidam o cache correspondente.

# 

# O tempo de expiração foi definido considerando o requisito de evitar consultas repetidas da mesma conta durante o mesmo dia.

# 

# \## Eventos e mensageria

# 

# Alterações importantes realizadas nas contas geram eventos para que outras áreas possam reagir sem criar dependência direta com a API de Onboarding.

# 

# São considerados os seguintes eventos:

# 

# \* `AccountCreated`

# \* `AccountUpdated`

# \* `AccountDeleted`

# 

# Dessa forma, outros serviços, como prevenção a fraude ou cartões, podem consumir esses eventos sem que o serviço de contas precise conhecer diretamente esses sistemas.

# 

# \## Testes

# 

# Os testes unitários estão no projeto:

# 

# ```text

# KRT.Onboarding.UnitTests

# ```

# 

# Os testes cobrem principalmente regras de domínio e casos de uso da aplicação.

# 

# Para executar:

# 

# ```bash

# dotnet test

# ```

# 

# \## Decisões adotadas

# 

# Algumas decisões foram tomadas durante o desenvolvimento para manter a solução simples e organizada:

# 

# \* O CPF é tratado como Value Object.

# \* O CPF é normalizado antes de ser armazenado.

# \* Não é permitido cadastrar mais de uma conta com o mesmo CPF.

# \* Uma nova conta é criada inicialmente com status `Active`.

# \* O CPF não pode ser alterado através da atualização da conta.

# \* O domínio não possui dependência do Entity Framework ou da camada de infraestrutura.

# \* O acesso ao banco é realizado através de repositories.

# \* Os controllers são responsáveis apenas pela comunicação HTTP e chamada dos casos de uso.

# \* O tratamento de exceções é centralizado.

# \* Cache e mensageria são tratados através de abstrações para reduzir o acoplamento.

# 

# \## Autor

# 

# Munir Marques



