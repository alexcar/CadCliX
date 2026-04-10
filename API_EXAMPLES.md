# 📡 Exemplos de Requisições - CadCliX API

Este arquivo contém exemplos de requisições usando diferentes ferramentas.

## 🌐 Via Swagger UI (Recomendado)

**Acesse:** `https://localhost:{porta}/`

O Swagger UI já está configurado e é a forma mais fácil de testar a API!

---

## 🔧 Via cURL (PowerShell)

### 1. Criar Endereço

```powershell
curl.exe -X POST "https://localhost:7xxx/api/addresses" `
  -H "Content-Type: application/json" `
  -d '{
    \"street\": \"Rua das Flores\",
    \"number\": \"123\",
    \"complement\": \"Apto 45\",
    \"neighborhood\": \"Centro\",
    \"city\": \"Rio de Janeiro\",
    \"state\": \"RJ\",
    \"country\": \"Brasil\",
    \"zipCode\": \"20000-000\"
  }'
```

### 2. Listar Endereços

```powershell
curl.exe -X GET "https://localhost:7xxx/api/addresses" `
  -H "accept: application/json"
```

### 3. Buscar Endereço por ID

```powershell
curl.exe -X GET "https://localhost:7xxx/api/addresses/1" `
  -H "accept: application/json"
```

### 4. Criar Cliente (Pessoa Física)

```powershell
curl.exe -X POST "https://localhost:7xxx/api/customers" `
  -H "Content-Type: application/json" `
  -d '{
    \"firstName\": \"João\",
    \"lastName\": \"Silva\",
    \"company\": \"Empresa XYZ Ltda\",
    \"tipoPessoa\": 1,
    \"rg\": \"12.345.678-9\",
    \"cpf\": \"123.456.789-00\",
    \"addressId\": 1
  }'
```

### 5. Criar Cliente (Pessoa Jurídica)

```powershell
curl.exe -X POST "https://localhost:7xxx/api/customers" `
  -H "Content-Type: application/json" `
  -d '{
    \"firstName\": \"Maria\",
    \"lastName\": \"Santos\",
    \"company\": \"ABC Comércio LTDA\",
    \"tipoPessoa\": 2,
    \"rg\": \"98.765.432-1\",
    \"cpf\": \"987.654.321-00\",
    \"cnpj\": \"12.345.678/0001-99\",
    \"addressId\": 1
  }'
```

### 6. Listar Clientes

```powershell
curl.exe -X GET "https://localhost:7xxx/api/customers" `
  -H "accept: application/json"
```

### 7. Buscar Cliente por ID

```powershell
curl.exe -X GET "https://localhost:7xxx/api/customers/1" `
  -H "accept: application/json"
```

---

## 📮 Via Postman

### Configuração Base
- **Base URL:** `https://localhost:7xxx`
- **Headers:** `Content-Type: application/json`

### Collection de Endpoints

#### 1. POST - Criar Endereço
```
URL: {{baseUrl}}/api/addresses
Method: POST
Body (JSON):
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

#### 2. GET - Listar Endereços
```
URL: {{baseUrl}}/api/addresses
Method: GET
```

#### 3. GET - Buscar Endereço
```
URL: {{baseUrl}}/api/addresses/1
Method: GET
```

#### 4. POST - Criar Cliente
```
URL: {{baseUrl}}/api/customers
Method: POST
Body (JSON):
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

#### 5. GET - Listar Clientes
```
URL: {{baseUrl}}/api/customers
Method: GET
```

---

## 🔥 Via HTTPie (se instalado)

### Criar Endereço
```bash
http POST https://localhost:7xxx/api/addresses \
  street="Rua das Flores" \
  number="123" \
  complement="Apto 45" \
  neighborhood="Centro" \
  city="Rio de Janeiro" \
  state="RJ" \
  country="Brasil" \
  zipCode="20000-000"
```

### Listar Endereços
```bash
http GET https://localhost:7xxx/api/addresses
```

### Criar Cliente
```bash
http POST https://localhost:7xxx/api/customers \
  firstName="João" \
  lastName="Silva" \
  company="Empresa XYZ Ltda" \
  tipoPessoa:=1 \
  rg="12.345.678-9" \
  cpf="123.456.789-00" \
  addressId:=1
```

---

## 🧪 Via JavaScript (Fetch API)

```javascript
// Criar Endereço
const criarEndereco = async () => {
  const response = await fetch('https://localhost:7xxx/api/addresses', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      street: "Rua das Flores",
      number: "123",
      complement: "Apto 45",
      neighborhood: "Centro",
      city: "Rio de Janeiro",
      state: "RJ",
      country: "Brasil",
      zipCode: "20000-000"
    })
  });
  
  const data = await response.json();
  console.log('Endereço criado:', data);
  return data;
};

// Listar Endereços
const listarEnderecos = async () => {
  const response = await fetch('https://localhost:7xxx/api/addresses');
  const data = await response.json();
  console.log('Endereços:', data);
  return data;
};

// Criar Cliente
const criarCliente = async (addressId) => {
  const response = await fetch('https://localhost:7xxx/api/customers', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      firstName: "João",
      lastName: "Silva",
      company: "Empresa XYZ Ltda",
      tipoPessoa: 1,
      rg: "12.345.678-9",
      cpf: "123.456.789-00",
      addressId: addressId
    })
  });
  
  const data = await response.json();
  console.log('Cliente criado:', data);
  return data;
};

// Usar
criarEndereco()
  .then(endereco => criarCliente(endereco.id))
  .then(cliente => console.log('Processo completo!', cliente));
```

---

## 📝 Notas Importantes

1. **Substitua `7xxx`** pela porta HTTPS real exibida ao executar `dotnet run`
2. **Certificado SSL:** Você pode receber avisos sobre certificado não confiável em desenvolvimento
3. **Dados em Memória:** Os dados são perdidos ao reiniciar a aplicação
4. **Ordem de Criação:** Sempre crie o endereço antes do cliente

---

## ✅ Respostas de Sucesso

### 200 OK (GET)
Retorna os dados solicitados

### 201 Created (POST)
Retorna o objeto criado com ID atribuído

## ❌ Respostas de Erro

### 400 Bad Request
Dados inválidos ou validação falhou
```json
{
  "message": "Primeiro nome é obrigatório., CPF é obrigatório para pessoa física."
}
```

### 404 Not Found
Recurso não encontrado
```json
{
  "message": "Cliente não encontrado."
}
```

---

## 🎯 Melhor Opção

**Use o Swagger UI:** É a forma mais fácil, rápida e completa de testar a API!

Acesse: `https://localhost:{porta}/`
