# CadCliX - API de Importação de Dados Contábeis

API RESTful desenvolvida em ASP.NET Core para importar e consultar dados de um sistema contábil legado.

## 🚀 Tecnologias

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (In-Memory Database)
- OpenAPI/Swagger

## 📋 Funcionalidades

- Importação de endereços (Address) via API REST
- Importação de clientes (Customer) via API REST
- Consulta de dados armazenados em memória
- Validação de dados usando padrão Result

## 🔧 Configuração e Execução

1. Restaurar pacotes NuGet:
```bash
dotnet restore
```

2. Executar a aplicação:
```bash
dotnet run
```

3. Acessar a documentação OpenAPI:
```
https://localhost:{porta}/openapi/v1.json
```

## 📡 Endpoints da API

### Addresses (Endereços)

#### GET /api/addresses
Lista todos os endereços ativos.

#### GET /api/addresses/{id}
Busca um endereço específico por ID.

#### POST /api/addresses
Importa um novo endereço.

**Exemplo de requisição:**
```json
{
  "street": "Rua Exemplo",
  "number": "123",
  "complement": "Apto 45",
  "neighborhood": "Centro",
  "city": "Rio de Janeiro",
  "state": "RJ",
  "country": "Brasil",
  "zipCode": "20000-000"
}
```

### Customers (Clientes)

#### GET /api/customers
Lista todos os clientes ativos com seus endereços.

#### GET /api/customers/{id}
Busca um cliente específico por ID com seu endereço.

#### POST /api/customers
Importa um novo cliente.

**Exemplo de requisição (Pessoa Física):**
```json
{
  "firstName": "João",
  "lastName": "Silva",
  "company": "Empresa XYZ Ltda",
  "tipoPessoa": 1,
  "rg": "12.345.678-9",
  "cpf": "123.456.789-00",
  "addressId": 1
}
```

**Exemplo de requisição (Pessoa Jurídica):**
```json
{
  "firstName": "Maria",
  "lastName": "Santos",
  "company": "ABC Comércio LTDA",
  "tipoPessoa": 2,
  "rg": "98.765.432-1",
  "cpf": "987.654.321-00",
  "cnpj": "12.345.678/0001-99",
  "addressId": 1
}
```

## 📝 Observações

- O banco de dados é **In-Memory**, os dados são perdidos quando a aplicação é reiniciada
- TipoPessoa: 1 = Física, 2 = Jurídica
- Para pessoa física, CPF é obrigatório
- Para pessoa jurídica, CNPJ é obrigatório
- O endereço deve ser criado antes do cliente

## 🔄 Fluxo de Importação Recomendado

1. Importar endereços via POST /api/addresses
2. Anotar os IDs retornados
3. Importar clientes via POST /api/customers usando os IDs dos endereços
4. Consultar dados via GET nos respectivos endpoints
