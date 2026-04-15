# Gestao-FDC

API ASP.NET Core para gestao de uma lanchonete, com catalogo, clientes, pedidos, estoque, financeiro e autenticacao JWT.

## 1. Estrutura do projeto

```text
Gestao-FDC
|-- Configuration/      Configuracoes tipadas como JWT e dados do negocio
|-- Controllers/        Endpoints HTTP organizados por modulo
|-- DTOs/               Contratos de entrada e saida da API
|-- Data/               DbContext, seeder e repositorio generico
|-- Interfaces/         Contratos de servicos e repositorios
|-- Models/             Entidades de dominio
|-- Services/           Regras de negocio centrais
|-- Migrations/         Historico de schema do Entity Framework Core
|-- Tests/              Testes unitarios com xUnit
|-- docs/postman/       Colecao Postman de exemplo
|-- scripts/db/         Scripts SQL de criacao e seed
|-- Dockerfile          Imagem da API
|-- docker-compose.yml  Execucao local com persistencia do banco
```

## 2. Modulos implementados

### Autenticacao e usuarios

- `AuthController` recebe login e devolve JWT.
- `AuthService` valida senha com `PasswordHasher`, monta claims e gera token.
- `UsersController` permite criar usuarios autenticados como administrador.
- O seed inicial cria um usuario padrao:

```text
usuario: admin
senha: Admin@123
```

### Catalogo

- `CategoriesController` gerencia categorias.
- `ProductsController` gerencia produtos e estoque inicial.
- Foram adicionados DTOs para evitar expor a entidade diretamente no `POST` e `PUT`.

### Clientes

- `CustomersController` permite cadastrar e manter os dados do cliente.
- O campo `LastOrderDate` e atualizado quando um pedido e registrado para o cliente.

### Pedidos

- `OrdersController` trabalha com DTOs de criacao e de alteracao de status.
- `OrderService` concentra a regra principal:
  - valida itens
  - carrega produtos
  - calcula total
  - baixa estoque quando o produto controla estoque
  - cria receita automaticamente para pedidos pagos no ato
  - cria receita na entrega para pedidos de mesa
  - gera link de WhatsApp com os dados do pedido

### Financeiro

- `FinancialController` expone:
  - listagem de transacoes
  - cadastro manual de receita/despesa
  - resumo por periodo
  - receita diaria
- `FinancialService` calcula totais e saldo.

### Estoque

- `InventoryController` faz CRUD dos insumos.
- A baixa automatica de estoque do produto acontece no fluxo de pedido.

## 3. Como cada camada se conecta

### Models

As classes em `Models/` representam o dominio persistido no banco. Elas sao o formato que o Entity Framework mapeia para tabelas.

### DTOs

As classes em `DTOs/` representam o contrato da API. A ideia e separar:

- o que o banco precisa guardar
- do que o cliente da API pode enviar

Isso melhora validacao, legibilidade e manutencao.

### Repository

`Repository<T>` abstrai operacoes CRUD simples. Ele reduz repeticao nos controllers e deixa os servicos focados em regra de negocio.

### Services

Os servicos concentram a regra que nao deve ficar no controller. Exemplo:

- autenticacao e emissao de token
- criacao de pedido com transacao
- lancamento financeiro automatico

### Controllers

Controllers recebem a requisicao HTTP, validam o contrato via DTO, chamam o servico ou repositorio e devolvem a resposta.

## 4. Banco de dados

O projeto usa SQLite via EF Core.

### Arquivos principais

- `Data/AppDbContext.cs`: mapeia os `DbSet`s e relacionamentos
- `Migrations/`: historico de evolucao do schema
- `scripts/db/init.sql`: script SQL de criacao das tabelas
- `scripts/db/seed.sql`: script SQL de popular dados basicos

### Seed automatico

No startup, a aplicacao executa:

1. `context.Database.Migrate()`
2. `DataSeeder.Seed(context)`

Isso garante que o banco seja criado, atualizado e populado com dados basicos no primeiro start.

## 5. Swagger e exemplos de requisicao

### Swagger

Com a API rodando, acesse:

```text
http://localhost:5286/swagger
```

O Swagger foi configurado com suporte a JWT Bearer. Depois do login, basta clicar em `Authorize` e informar:

```text
Bearer SEU_TOKEN
```

### Arquivos de exemplo

- `Gestao-FDC.http`: exemplos prontos para VS Code ou Rider
- `docs/postman/Gestao-FDC.postman_collection.json`: colecao Postman

## 6. Testes unitarios

Os testes ficam em `Tests/` e usam:

- xUnit
- SQLite em memoria

Casos cobertos:

- login com credenciais validas
- bloqueio de cadastro com username duplicado
- criacao de pedido com baixa de estoque e receita automatica
- criacao de receita ao entregar pedido de mesa

Execute com:

```bash
dotnet test Gestao-FDC.sln
```

## 7. Como rodar localmente

### Opcao 1: sem Docker

```bash
dotnet restore
dotnet build Gestao-FDC.sln
dotnet run --project Gestao-FDC.csproj
```

Endpoints usuais:

- API: `http://localhost:5286`
- Swagger: `http://localhost:5286/swagger`

### Opcao 2: com Docker

```bash
docker compose up --build
```

Endpoints usuais:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

## 8. Observacoes de implementacao

### Por que usar DTO no lugar da entidade direto no controller?

Porque a entidade representa o modelo persistido. Se o cliente puder postar a entidade inteira, ele passa a controlar campos que deveriam ser internos, como ids calculados, relacionamentos ou propriedades de auditoria.

### Por que o `OrderService` usa transacao?

Criar pedido envolve varias escritas:

- salvar pedido
- atualizar estoque
- registrar financeiro
- atualizar ultimo pedido do cliente

Se uma parte falhar no meio, a transacao evita que o banco fique inconsistente.

### Por que manter `FinancialService` separado do controller?

Porque calculos de resumo, receita diaria e saldo pertencem a regra de negocio. O controller so orquestra HTTP.

## 9. Proximos passos recomendados

- adicionar testes de integracao para controllers
- versionar melhor DTOs de resposta
- substituir o `Repository<T>` por repositorios especializados onde houver consulta mais complexa
- configurar segredo JWT por variavel de ambiente em producao
