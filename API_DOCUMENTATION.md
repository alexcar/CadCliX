# CadCliX - API de Importação de Dados Contábeis

API RESTful desenvolvida em ASP.NET Core para importar e consultar dados de um sistema contábil legado.

## 🚀 Tecnologias

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core (In-Memory Database)
- Swagger/Swashbuckle

## 📋 Funcionalidades

- Importação de endereços (Address) via API REST
- Importação de clientes (Customer) via API REST
- Consulta de dados armazenados em memória
- Validação de dados usando padrão Result
- Documentação interativa com Swagger UI

## 🔧 Configuração e Execução

1. Restaurar pacotes NuGet:
```bash
dotnet restore
```

2. Executar a aplicação:
```bash
dotnet run --project CadCliX/CadCliX.csproj
```

3. Acessar o Swagger UI:
```
https://localhost:{porta}/
```
ou
```
http://localhost:{porta}/
```

A documentação Swagger abrirá automaticamente como página inicial da aplicação.

## 📚 Swagger UI

O Swagger UI fornece:
- **Documentação interativa completa** de todos os endpoints
- **Interface para testar** as chamadas da API diretamente no navegador
- **Schemas detalhados** dos DTOs com exemplos
- **Códigos de resposta** e descrições de erros
- **Exemplos de requisições** para cada endpoint

### Recursos do Swagger:
- URL padrão: `https://localhost:{porta}/`
- JSON da API: `https://localhost:{porta}/swagger/v1/swagger.json`
- Documentação XML incluída com descrições detalhadas
- Tempo de resposta exibido para cada requisição
- Modelos expandidos para melhor visualização

## 📡 Endpoints da API

### Addresses (Endereços)

#### GET /api/addresses
Lista todos os endereços ativos.

**Resposta 200 - Sucesso:**
```json
[
  {
    "id": 1,
    "street": "Rua das Flores",
    "number": "123",
    "complement": "Apto 45",
    "neighborhood": "Centro",
    "city": "Rio de Janeiro",
    "state": "RJ",
    "country": "Brasil",
    "zipCode": "20000-000"
  }
]
```

#### GET /api/addresses/{id}
Busca um endereço específico por ID.

**Parâmetros:**
- `id` (path) - ID do endereço

**Resposta 200 - Sucesso:**
```json
{
  "id": 1,
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Apto 45",
  "neighborhood": "Centro",
  "city": "Rio de Janeiro",
  "state": "RJ",
  "country": "Brasil",
  "zipCode": "20000-000"
}
```

**Resposta 404 - Não encontrado:**
```json
{
  "message": "Endereço não encontrado."
}
```

#### POST /api/addresses
Importa um novo endereço do sistema legado.

**Corpo da requisição:**
```json
{
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Apto 45",
  "neighborhood": "Centro",
  "city": "Rio de Janeiro",
  "state": "RJ",
  "country": "Brasil",
  "zipCode": "20000-000"
}
```

**Resposta 201 - Criado:**
```json
{
  "id": 1,
  "street": "Rua das Flores",
  "number": "123",
  "complement": "Apto 45",
  "neighborhood": "Centro",
  "city": "Rio de Janeiro",
  "state": "RJ",
  "country": "Brasil",
  "zipCode": "20000-000"
}
```

**Resposta 400 - Validação:**
```json
{
  "message": "Rua é obrigatória.; Cidade é obrigatória."
}
```

### Customers (Clientes)

#### GET /api/customers
Lista todos os clientes ativos com seus endereços.

**Resposta 200 - Sucesso:**
```json
[
  {
    "id": 1,
    "firstName": "João",
    "lastName": "Silva",
    "company": "Empresa XYZ Ltda",
    "tipoPessoa": 1,
    "rg": "12.345.678-9",
    "cpf": "123.456.789-00",
    "cnpj": null,
    "addressId": 1,
    "address": {
      "id": 1,
      "street": "Rua das Flores",
      "number": "123",
      "complement": "Apto 45",
      "neighborhood": "Centro",
      "city": "Rio de Janeiro",
      "state": "RJ",
      "country": "Brasil",
      "zipCode": "20000-000"
    }
  }
]
```

#### GET /api/customers/{id}
Busca um cliente específico por ID com seu endereço.

**Parâmetros:**
- `id` (path) - ID do cliente

**Resposta 200 - Sucesso:** Similar ao item do array acima.

**Resposta 404 - Não encontrado:**
```json
{
  "message": "Cliente não encontrado."
}
```

#### POST /api/customers
Importa um novo cliente do sistema legado.

**Corpo da requisição (Pessoa Física - TipoPessoa = 1):**
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

**Corpo da requisição (Pessoa Jurídica - TipoPessoa = 2):**
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

**Resposta 201 - Criado:** Cliente criado com dados completos incluindo endereço.

**Resposta 400 - Validação:**
```json
{
  "message": "Primeiro nome é obrigatório., CPF é obrigatório para pessoa física."
}
```

**Resposta 400 - Endereço não existe:**
```json
{
  "message": "O endereço informado não existe."
}
```

## 📝 Observações

- O banco de dados é **In-Memory**, os dados são perdidos quando a aplicação é reiniciada
- **TipoPessoa**: 1 = Física, 2 = Jurídica
- Para pessoa física, CPF é obrigatório
- Para pessoa jurídica, CNPJ é obrigatório
- O endereço deve ser criado antes do cliente
- Todos os campos marcados como obrigatórios são validados

## 🔄 Fluxo de Importação Recomendado

1. **Abrir o Swagger UI** em `https://localhost:{porta}/`
2. **Importar endereços** via POST /api/addresses
3. **Anotar os IDs** retornados na resposta
4. **Importar clientes** via POST /api/customers usando os IDs dos endereços
5. **Consultar dados** via GET nos respectivos endpoints

## 🧪 Testando a API com Swagger

1. Execute a aplicação com `dotnet run`
2. Abra o navegador em `https://localhost:{porta}/` (a porta será exibida no console)
3. No Swagger UI, você verá duas seções: **Addresses** e **Customers**
4. Clique em qualquer endpoint para expandir
5. Clique em **"Try it out"** para habilitar o teste
6. Preencha os parâmetros/body necessários
7. Clique em **"Execute"** para fazer a requisição
8. Veja a resposta abaixo com código de status, corpo e headers

## 📦 Pacotes NuGet Utilizados

- `Microsoft.EntityFrameworkCore.InMemory` - Banco de dados em memória
- `Swashbuckle.AspNetCore` - Geração de documentação Swagger/OpenAPI
- `Microsoft.AspNetCore.OpenApi` - Suporte OpenAPI

## 🏗️ Estrutura do Projeto

```
CadCliX/
├── Common/              # Classes base e Result Pattern
├── Controllers/         # Controllers da API (com documentação XML)
├── Data/               # DbContext do Entity Framework
├── DTOs/               # Data Transfer Objects (com exemplos Swagger)
├── Entities/           # Entidades de domínio
├── Enums/              # Enumerações
└── Repositories/       # Repositórios de acesso a dados
```
