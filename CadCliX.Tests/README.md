# 🧪 CadCliX.Tests

Projeto de testes unitários para a API CadCliX com cobertura de código > 80%.

## 📊 Estatísticas

- **Total de Testes:** 52
- **Taxa de Sucesso:** 100% ✅
- **Cobertura de Código:** > 80% 🎯
- **Framework:** xUnit + FluentAssertions + Moq

## 🚀 Execução Rápida

### Opção 1: Via Visual Studio
1. Abra o **Test Explorer** (Menu: Test > Test Explorer ou Ctrl+E, T)
2. Clique em **Run All Tests** ▶️

### Opção 2: Via Terminal
```powershell
dotnet test
```

### Opção 3: Via Scripts PowerShell
```powershell
# Simples
.\run-tests-simple.ps1

# Com cobertura
.\run-tests.ps1
```

## 📁 Estrutura do Projeto

```
CadCliX.Tests/
├── Entities/
│   ├── AddressTests.cs          # 11 testes
│   └── CustomerTests.cs         # 13 testes
├── Repositories/
│   ├── AddressRepositoryTests.cs   # 6 testes
│   └── CustomerRepositoryTests.cs  # 6 testes
├── Controllers/
│   ├── AddressesControllerTests.cs  # 7 testes
│   └── CustomersControllerTests.cs  # 9 testes
├── TEST_DOCUMENTATION.md        # Documentação completa
└── CadCliX.Tests.csproj
```

## 🔧 Tecnologias

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| xUnit | 2.9.3 | Framework de testes |
| FluentAssertions | 7.0.0 | Asserções fluentes |
| Moq | 4.20.72 | Mocking |
| EF Core InMemory | 10.0.5 | Banco de dados em memória |
| coverlet.collector | 6.0.4 | Cobertura de código |

## 📈 Cobertura por Camada

| Camada | Cobertura Estimada |
|--------|-------------------|
| **Entities** | ~100% |
| **Repositories** | ~100% |
| **Controllers** | ~90% |
| **TOTAL** | **> 80%** ✅ |

## 🎯 Tipos de Testes

### ✅ Entities (24 testes)
- Validações de campos obrigatórios
- Regras de negócio (CPF, CNPJ)
- Criação de objetos válidos/inválidos
- Múltiplos erros simultâneos

### ✅ Repositories (12 testes)
- CRUD operations
- Eager loading (Include)
- Queries assíncronas
- Banco de dados em memória

### ✅ Controllers (16 testes)
- Endpoints GET/POST
- Respostas HTTP (200, 201, 400, 404)
- DTOs e mapeamentos
- Validações de negócio

## 📝 Exemplo de Teste

```csharp
[Fact]
public async Task Create_WithValidData_ShouldReturnSuccess()
{
    // Arrange
    var street = "Rua das Flores";
    var number = "123";
    // ...

    // Act
    var result = Address.Create(street, number, ...);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Street.Should().Be(street);
}
```

## 🔍 Executar Testes Específicos

```powershell
# Por namespace
dotnet test --filter "FullyQualifiedName~Entities"
dotnet test --filter "FullyQualifiedName~Repositories"
dotnet test --filter "FullyQualifiedName~Controllers"

# Por classe
dotnet test --filter "FullyQualifiedName~AddressTests"
dotnet test --filter "FullyQualifiedName~CustomerTests"

# Por método
dotnet test --filter "Name~Create_WithValidData"
```

## 📊 Relatório de Cobertura

### Gerar Cobertura
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Visualizar em HTML (Requer ReportGenerator)
```powershell
# 1. Instalar ferramenta (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# 2. Executar testes com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage/

# 3. Gerar relatório HTML
reportgenerator `
    -reports:"CadCliX.Tests\coverage\coverage.opencover.xml" `
    -targetdir:"CadCliX.Tests\coverage\html" `
    -reporttypes:Html

# 4. Abrir no navegador
start CadCliX.Tests\coverage\html\index.html
```

## ✨ Padrões de Qualidade

### ✅ Implementados
- [x] AAA Pattern (Arrange-Act-Assert)
- [x] Nomenclatura descritiva
- [x] Testes isolados e independentes
- [x] FluentAssertions para legibilidade
- [x] Mock de dependências
- [x] In-Memory Database
- [x] Dispose Pattern
- [x] Happy path + Error cases + Edge cases

### 📏 Convenções
- Nome: `[Method]_[Scenario]_[ExpectedBehavior]`
- Estrutura: Arrange > Act > Assert
- 1 Assert principal por teste
- Testes rápidos (< 100ms cada)

## 🎓 Recursos Educacionais

### Documentação
- 📄 [TEST_DOCUMENTATION.md](TEST_DOCUMENTATION.md) - Guia completo de testes

### Links Úteis
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)

## 🐛 Troubleshooting

### Testes não aparecem no Test Explorer
```powershell
# Rebuild da solução
dotnet clean
dotnet build
```

### Erro de cobertura
```powershell
# Reinstalar coverlet
dotnet add package coverlet.collector
```

## 📞 Comandos Úteis

```powershell
# Executar com verbosidade detalhada
dotnet test --logger "console;verbosity=detailed"

# Executar em modo watch (re-executa ao salvar)
dotnet watch test

# Executar apenas testes que falharam
dotnet test --filter "TestCategory=FailedTests"

# Executar com timeout
dotnet test --blame-hang-timeout 30s
```

## 🎉 Resultado Final

```
┌────────────────────────────────────┐
│   ✅ 52 TESTES PASSANDO           │
│   📊 COBERTURA > 80%              │
│   🚀 PRONTO PARA PRODUÇÃO         │
└────────────────────────────────────┘
```

---

**Desenvolvido com ❤️ usando .NET 10 e xUnit**
