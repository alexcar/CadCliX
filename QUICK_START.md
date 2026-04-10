# 🚀 Início Rápido - CadCliX API

## Como Executar

1. **No terminal do Visual Studio ou PowerShell:**
   ```powershell
   cd C:\Code\Infnet\DevOps\CadCliX
   dotnet run --project CadCliX/CadCliX.csproj
   ```

2. **A aplicação iniciará e mostrará a URL:**
   ```
   info: Microsoft.Hosting.Lifetime[14]
         Now listening on: https://localhost:7xxx
         Now listening on: http://localhost:5xxx
   ```

3. **Abra seu navegador em:**
   ```
   https://localhost:7xxx
   ```
   (use a porta HTTPS exibida no console)

## 🎯 Swagger UI já está configurado!

Ao acessar a URL raiz da aplicação, você verá automaticamente:

✅ **Interface Swagger UI** com documentação interativa
✅ **Todos os endpoints** organizados por controller
✅ **Botões "Try it out"** para testar diretamente
✅ **Exemplos de requisições** para cada endpoint
✅ **Schemas detalhados** de todos os DTOs
✅ **Códigos de resposta** documentados

## 📝 Teste Rápido

### 1. Criar um Endereço

No Swagger UI:
1. Expanda **Addresses** > **POST /api/addresses**
2. Clique em **"Try it out"**
3. Use este exemplo:
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
4. Clique em **"Execute"**
5. Anote o **ID** retornado (será 1 se for o primeiro)

### 2. Criar um Cliente

1. Expanda **Customers** > **POST /api/customers**
2. Clique em **"Try it out"**
3. Use este exemplo (substitua addressId pelo ID do passo anterior):
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
4. Clique em **"Execute"**

### 3. Listar Todos os Clientes

1. Expanda **Customers** > **GET /api/customers**
2. Clique em **"Try it out"**
3. Clique em **"Execute"**
4. Veja a lista de clientes com seus endereços

## 🎨 Recursos do Swagger Configurados

- ✅ Documentação XML incluída
- ✅ Exemplos em todos os DTOs
- ✅ Descrições detalhadas de cada endpoint
- ✅ Validações documentadas
- ✅ Códigos de status HTTP documentados (200, 201, 400, 404)
- ✅ Tempo de resposta exibido
- ✅ Modelos expandidos automaticamente
- ✅ Rota raiz "/" aponta para Swagger UI

## 🔍 URLs Úteis

- **Swagger UI:** `https://localhost:{porta}/`
- **Swagger JSON:** `https://localhost:{porta}/swagger/v1/swagger.json`
- **API Addresses:** `https://localhost:{porta}/api/addresses`
- **API Customers:** `https://localhost:{porta}/api/customers`

## 💡 Dicas

- O Swagger UI é a página inicial - não precisa adicionar /swagger na URL
- Todos os exemplos já estão pré-preenchidos nos schemas
- Use HTTPS para evitar avisos de segurança
- O banco é em memória - os dados são perdidos ao reiniciar
- Para pessoa jurídica, use `tipoPessoa: 2` e adicione o campo `cnpj`

## 🛠️ Pacotes Swagger Instalados

```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="8.0.0" />
```

O Swagger está 100% funcional e pronto para uso! 🎉
