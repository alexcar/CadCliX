# Pipeline CI/CD - Documentação Técnica Detalhada
**Projeto: CadCliX - Sistema de Importação de Dados Contábeis**

---

## Informações do Documento

| **Campo** | **Valor** |
|-----------|-----------|
| **Versão** | 1.0 |
| **Data** | Janeiro 2025 |
| **Autor** | Equipe CadCliX |
| **Repositório** | https://github.com/alexcar/CadCliX |
| **Status** | Em Produção ✅ |

---

## Índice

1. [Visão Geral](#1-visão-geral)
2. [Arquitetura do Pipeline](#2-arquitetura-do-pipeline)
3. [Configuração e Triggers](#3-configuração-e-triggers)
4. [Permissões e Segurança](#4-permissões-e-segurança)
5. [Etapas Detalhadas do Pipeline](#5-etapas-detalhadas-do-pipeline)
   - [5.1. Checkout do Código-Fonte](#51-checkout-do-código-fonte)
   - [5.2. Configuração do Ambiente .NET 10](#52-configuração-do-ambiente-net-10)
   - [5.3. Restauração de Dependências](#53-restauração-de-dependências)
   - [5.4. Build do Projeto](#54-build-do-projeto)
   - [5.5. Execução dos Testes Unitários](#55-execução-dos-testes-unitários)
   - [5.6. Publicação dos Resultados dos Testes](#56-publicação-dos-resultados-dos-testes)
   - [5.7. Configuração do Docker Buildx](#57-configuração-do-docker-buildx)
   - [5.8. Autenticação no Docker Hub](#58-autenticação-no-docker-hub)
   - [5.9. Extração de Metadados Docker](#59-extração-de-metadados-docker)
   - [5.10. Build e Push da Imagem Docker](#510-build-e-push-da-imagem-docker)
6. [Fluxo de Execução Completo](#6-fluxo-de-execução-completo)
7. [Variáveis de Ambiente e Secrets](#7-variáveis-de-ambiente-e-secrets)
8. [Estratégia de Versionamento e Tags](#8-estratégia-de-versionamento-e-tags)
9. [Otimizações e Cache](#9-otimizações-e-cache)
10. [Monitoramento e Logs](#10-monitoramento-e-logs)
11. [Tratamento de Erros](#11-tratamento-de-erros)
12. [Boas Práticas Implementadas](#12-boas-práticas-implementadas)
13. [Requisitos Técnicos](#13-requisitos-técnicos)
14. [Referências e Recursos](#14-referências-e-recursos)

---

## 1. Visão Geral

### 1.1. Propósito

Este documento descreve detalhadamente o pipeline de **Integração Contínua e Entrega Contínua (CI/CD)** implementado para o projeto **CadCliX**, um sistema de importação e consulta de dados contábeis desenvolvido em ASP.NET Core Web API.

### 1.2. Objetivo do Pipeline

O pipeline automatiza todo o processo de desenvolvimento, desde a compilação do código até a disponibilização da aplicação em produção através de containers Docker, garantindo:

- ✅ **Qualidade**: Execução automática de 52 testes unitários
- ✅ **Consistência**: Build padronizado e reproduzível
- ✅ **Agilidade**: Deploy automatizado sem intervenção manual
- ✅ **Rastreabilidade**: Versionamento automático de imagens
- ✅ **Segurança**: Uso de secrets e permissões mínimas

### 1.3. Tecnologias Utilizadas

| **Categoria** | **Tecnologia** | **Versão** | **Finalidade** |
|---------------|----------------|------------|----------------|
| **Framework** | .NET | 10.0 | Desenvolvimento da aplicação |
| **CI/CD** | GitHub Actions | Latest | Automação do pipeline |
| **Containerização** | Docker | Latest | Empacotamento da aplicação |
| **Registry** | Docker Hub | Latest | Armazenamento de imagens |
| **Testes** | xUnit | 2.9.3 | Framework de testes unitários |
| **Runner** | Ubuntu | Latest | Ambiente de execução |

---

## 2. Arquitetura do Pipeline

### 2.1. Diagrama de Alto Nível

```
┌─────────────────────────────────────────────────────────────────┐
│                         TRIGGER                                  │
│              Push/Merge para Branch Main                         │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                    JOB: build-test-deploy                        │
│                    Runner: ubuntu-latest                         │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
        ┌────────────────────────────────────────┐
        │     FASE 1: CODE & DEPENDENCIES        │
        ├────────────────────────────────────────┤
        │  1. Checkout code                      │
        │  2. Setup .NET 10                      │
        │  3. Restore dependencies               │
        └────────────┬───────────────────────────┘
                     │
                     ▼
        ┌────────────────────────────────────────┐
        │     FASE 2: BUILD & QUALITY            │
        ├────────────────────────────────────────┤
        │  4. Build (Release)                    │
        │  5. Run unit tests (52 tests)          │
        │  6. Publish test results               │
        └────────────┬───────────────────────────┘
                     │
                     ▼
        ┌────────────────────────────────────────┐
        │     FASE 3: CONTAINERIZATION           │
        ├────────────────────────────────────────┤
        │  7. Setup Docker Buildx                │
        │  8. Login to Docker Hub                │
        │  9. Extract metadata                   │
        │  10. Build & Push image                │
        └────────────┬───────────────────────────┘
                     │
                     ▼
        ┌────────────────────────────────────────┐
        │          RESULTADO FINAL                │
        ├────────────────────────────────────────┤
        │  ✅ Build validado                     │
        │  ✅ Testes passaram                    │
        │  ✅ Imagem publicada no Docker Hub     │
        │  📦 alexcar/cadclix:latest             │
        └────────────────────────────────────────┘
```

### 2.2. Modelo de Execução

- **Tipo**: Workflow único com job único
- **Paralelização**: Não aplicável (steps sequenciais)
- **Duração Estimada**: 3-5 minutos
- **Frequência**: A cada merge para branch `main`

---

## 3. Configuração e Triggers

### 3.1. Definição do Workflow

```yaml
name: CI/CD Pipeline
```

**Descrição**: Nome identificador do workflow exibido na interface do GitHub Actions.

**Objetivo**: Facilitar a identificação do pipeline entre múltiplos workflows.

### 3.2. Eventos de Ativação (Triggers)

```yaml
on:
  push:
    branches:
      - main
```

| **Parâmetro** | **Valor** | **Descrição** |
|---------------|-----------|---------------|
| **Evento** | `push` | Disparo ao receber commits |
| **Branch** | `main` | Apenas na branch principal |
| **Gatilho** | Merge de Pull Request | Quando PR de `develop` é merged |

**Comportamento**:
- ✅ **Executa**: Push direto ou merge de PR para `main`
- ❌ **Não executa**: Commits em outras branches
- ❌ **Não executa**: Pull Requests abertos (sem merge)

### 3.3. Estratégia de Branching

```
develop (desenvolvimento)
    │
    │ [Pull Request]
    ▼
main (produção) ──► [Trigger CI/CD] ──► Deploy
```

**Fluxo Recomendado**:
1. Desenvolvimento acontece na branch `develop`
2. Cria-se Pull Request de `develop` para `main`
3. Após revisão, o PR é aprovado e merged
4. O merge aciona automaticamente o pipeline
5. Pipeline valida, testa e faz deploy da aplicação

---

## 4. Permissões e Segurança

### 4.1. Configuração de Permissões

```yaml
permissions:
  contents: read
  checks: write
  pull-requests: write
```

| **Permissão** | **Nível** | **Finalidade** | **Justificativa** |
|---------------|-----------|----------------|-------------------|
| `contents` | `read` | Ler código do repositório | Necessário para checkout |
| `checks` | `write` | Escrever status de checks | Publicar resultados de testes |
| `pull-requests` | `write` | Comentar em PRs | Adicionar reports de testes |

### 4.2. Princípio de Menor Privilégio

O pipeline segue o **Principle of Least Privilege (PoLP)**:

- ✅ Apenas permissões estritamente necessárias
- ✅ Nenhuma permissão administrativa
- ✅ Sem acesso a secrets não utilizados
- ✅ Escopo limitado ao repositório

### 4.3. Secrets Utilizados

| **Secret** | **Tipo** | **Onde é Usado** | **Exposição** |
|------------|----------|------------------|---------------|
| `DOCKERHUB_USERNAME` | String | Login Docker Hub | Não aparece em logs |
| `DOCKERHUB_TOKEN` | Token | Autenticação Docker | Mascarado em logs |

**Segurança dos Secrets**:
- 🔒 Armazenados criptografados no GitHub
- 🔒 Nunca expostos em logs ou outputs
- 🔒 Acessíveis apenas durante execução do workflow
- 🔒 Podem ser rotacionados sem alterar código

---

## 5. Etapas Detalhadas do Pipeline

### 5.1. Checkout do Código-Fonte

#### 5.1.1. Configuração

```yaml
- name: Checkout code
  uses: actions/checkout@v4
```

#### 5.1.2. Descrição Técnica

**Action Utilizada**: `actions/checkout@v4`

**Responsabilidade**: Clonar o repositório Git no runner do GitHub Actions.

#### 5.1.3. Funcionamento

1. GitHub Actions provisiona um runner Ubuntu limpo
2. A action `checkout` clona o repositório
3. Faz checkout da branch/commit que disparou o workflow
4. Disponibiliza o código-fonte no diretório `/home/runner/work/CadCliX/CadCliX`

#### 5.1.4. Parâmetros Implícitos

| **Parâmetro** | **Valor Padrão** | **Significado** |
|---------------|------------------|-----------------|
| `ref` | Branch do trigger | Commit exato que disparou o pipeline |
| `fetch-depth` | 1 | Clone superficial (apenas último commit) |
| `submodules` | false | Não clona submódulos Git |

#### 5.1.5. Saída

- ✅ Código-fonte disponível no workspace
- ✅ Histórico Git presente (último commit)
- ✅ Arquivos `.git/` configurados

#### 5.1.6. Falhas Possíveis

| **Erro** | **Causa** | **Solução** |
|----------|-----------|-------------|
| `fatal: repository not found` | Problema de permissão | Verificar `contents: read` |
| `Reference does not exist` | Branch deletada | Verificar integridade da branch |

---

### 5.2. Configuração do Ambiente .NET 10

#### 5.2.1. Configuração

```yaml
- name: Setup .NET 10
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '10.0.x'
```

#### 5.2.2. Descrição Técnica

**Action Utilizada**: `actions/setup-dotnet@v4`

**Responsabilidade**: Instalar e configurar o SDK do .NET 10 no runner.

#### 5.2.3. Funcionamento

1. Verifica se .NET 10 já está em cache
2. Se não estiver, faz download do SDK
3. Instala o SDK no runner
4. Configura variáveis de ambiente (`PATH`, `DOTNET_ROOT`)
5. Valida a instalação executando `dotnet --version`

#### 5.2.4. Parâmetros

| **Parâmetro** | **Valor** | **Descrição** |
|---------------|-----------|---------------|
| `dotnet-version` | `'10.0.x'` | Versão do SDK (última patch de 10.0) |

**Observação**: O sufixo `.x` significa "última versão de patch disponível" (ex: 10.0.5).

#### 5.2.5. Saída

- ✅ SDK .NET 10 instalado
- ✅ Comando `dotnet` disponível globalmente
- ✅ Ferramentas CLI configuradas

#### 5.2.6. Cache

A action utiliza cache automático para acelerar instalações subsequentes:

```
Cache Key: setup-dotnet-Linux-10.0.x
Cache Size: ~200 MB
Tempo economizado: ~30 segundos
```

#### 5.2.7. Verificação

```bash
$ dotnet --version
10.0.5

$ dotnet --info
.NET SDK:
 Version:   10.0.5
 Commit:    abc123def

Runtime Environment:
 OS Name:     ubuntu
 OS Version:  22.04
```

---

### 5.3. Restauração de Dependências

#### 5.3.1. Configuração

```yaml
- name: Restore dependencies
  run: dotnet restore
```

#### 5.3.2. Descrição Técnica

**Comando**: `dotnet restore`

**Responsabilidade**: Baixar todos os pacotes NuGet necessários para o projeto.

#### 5.3.3. Funcionamento

1. Lê os arquivos `.csproj` (CadCliX.csproj e CadCliX.Tests.csproj)
2. Analisa as referências de pacotes (`<PackageReference>`)
3. Baixa pacotes do NuGet.org
4. Armazena em cache local (`~/.nuget/packages`)
5. Cria arquivo `project.assets.json` com grafo de dependências

#### 5.3.4. Pacotes Restaurados

**Projeto CadCliX**:
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.5" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.5" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="10.1.7" />
```

**Projeto CadCliX.Tests**:
```xml
<PackageReference Include="coverlet.collector" Version="6.0.4" />
<PackageReference Include="FluentAssertions" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.5" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.13.0" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="xunit" Version="2.9.3" />
<PackageReference Include="xunit.runner.visualstudio" Version="3.0.0" />
```

#### 5.3.5. Otimizações

| **Recurso** | **Benefício** |
|-------------|---------------|
| **Cache de Pacotes** | Pacotes já baixados são reutilizados |
| **Download Paralelo** | Múltiplos pacotes baixados simultaneamente |
| **Fontes Oficiais** | Apenas NuGet.org (mais rápido e seguro) |

#### 5.3.6. Saída

```
Determining projects to restore...
Restored C:\...\CadCliX\CadCliX.csproj (in 1.2 sec).
Restored C:\...\CadCliX.Tests\CadCliX.Tests.csproj (in 1.5 sec).
```

#### 5.3.7. Arquivos Gerados

```
CadCliX/obj/project.assets.json
CadCliX/obj/CadCliX.csproj.nuget.g.props
CadCliX/obj/CadCliX.csproj.nuget.g.targets
```

#### 5.3.8. Falhas Possíveis

| **Erro** | **Causa** | **Solução** |
|----------|-----------|-------------|
| `NU1101: Unable to find package` | Pacote não existe no NuGet | Verificar nome/versão do pacote |
| `NU1102: Version conflict` | Conflito de dependências | Atualizar versões compatíveis |
| `HttpRequestException` | Falha de rede | Retry automático (GitHub tenta 3x) |

---

### 5.4. Build do Projeto

#### 5.4.1. Configuração

```yaml
- name: Build
  run: dotnet build --configuration Release --no-restore
```

#### 5.4.2. Descrição Técnica

**Comando**: `dotnet build`

**Responsabilidade**: Compilar o código-fonte C# em assemblies .NET.

#### 5.4.3. Parâmetros

| **Flag** | **Descrição** | **Impacto** |
|----------|---------------|-------------|
| `--configuration Release` | Compila em modo Release | Otimizações ativadas, sem símbolos de debug |
| `--no-restore` | Pula restauração de pacotes | Usa pacotes já restaurados no step anterior |

#### 5.4.4. Funcionamento

1. **Análise**: Lê arquivos `.csproj` e `.cs`
2. **Compilação**: Roslyn Compiler converte C# em IL (Intermediate Language)
3. **Referências**: Resolve dependências entre projetos
4. **Otimização**: Aplica otimizações de Release
5. **Output**: Gera DLLs em `bin/Release/net10.0/`

#### 5.4.5. Configuração de Release

Diferenças entre Debug e Release:

| **Aspecto** | **Debug** | **Release** |
|-------------|-----------|-------------|
| **Otimizações** | Desativadas | Ativadas |
| **Símbolos de Debug** | Gerados (.pdb) | Não gerados |
| **Inline de Métodos** | Não | Sim |
| **Constantes DEBUG** | Definida | Não definida |
| **Tamanho do Assembly** | Maior | Menor |
| **Performance** | Menor | Maior |

#### 5.4.6. Estrutura de Saída

```
CadCliX/bin/Release/net10.0/
├── CadCliX.dll                  # Assembly principal
├── CadCliX.deps.json            # Grafo de dependências
├── CadCliX.runtimeconfig.json   # Configuração de runtime
├── CadCliX.xml                  # Documentação XML
├── appsettings.json             # Configurações
└── [pacotes NuGet]              # DLLs de dependências
```

#### 5.4.7. Processo de Compilação

```
┌──────────────────────────────────────────────────────┐
│ 1. Análise Sintática (Parsing)                      │
│    C# Source Files (.cs) → Syntax Trees              │
└─────────────────┬────────────────────────────────────┘
                  │
                  ▼
┌──────────────────────────────────────────────────────┐
│ 2. Análise Semântica (Semantic Analysis)            │
│    Verificação de tipos, regras C#                   │
└─────────────────┬────────────────────────────────────┘
                  │
                  ▼
┌──────────────────────────────────────────────────────┐
│ 3. Geração de IL (Intermediate Language)            │
│    Bytecode .NET independente de plataforma          │
└─────────────────┬────────────────────────────────────┘
                  │
                  ▼
┌──────────────────────────────────────────────────────┐
│ 4. Otimizações (Release Only)                       │
│    Inline, remoção de código morto, etc.            │
└─────────────────┬────────────────────────────────────┘
                  │
                  ▼
┌──────────────────────────────────────────────────────┐
│ 5. Geração do Assembly (.dll)                       │
│    Empacotamento final com metadados                 │
└──────────────────────────────────────────────────────┘
```

#### 5.4.8. Validações Realizadas

Durante o build, o compilador valida:

- ✅ **Sintaxe**: Código C# válido
- ✅ **Tipos**: Tipagem estática correta
- ✅ **Referências**: Todos os namespaces e tipos existem
- ✅ **Nullable**: Avisos de referências nulas (nullable reference types)
- ✅ **Warnings**: Avisos de código potencialmente problemático

#### 5.4.9. Saída do Log

```
Build started...
Build succeeded.

    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:12.34
```

#### 5.4.10. Falhas Possíveis

| **Erro** | **Código** | **Causa Comum** | **Solução** |
|----------|------------|-----------------|-------------|
| **Compilation Error** | CS0103 | Nome não existe no contexto | Verificar using/namespace |
| **Type Mismatch** | CS0029 | Conversão de tipo inválida | Corrigir tipos |
| **Missing Reference** | CS0246 | Assembly não encontrado | Adicionar PackageReference |
| **Version Conflict** | NU1605 | Conflito de dependências | Unificar versões |

---

### 5.5. Execução dos Testes Unitários

#### 5.5.1. Configuração

```yaml
- name: Run unit tests
  run: dotnet test --configuration Release --no-build --verbosity normal --logger "trx;LogFileName=test-results.trx"
```

#### 5.5.2. Descrição Técnica

**Comando**: `dotnet test`

**Responsabilidade**: Executar todos os 52 testes unitários e validar qualidade do código.

#### 5.5.3. Parâmetros Detalhados

| **Flag** | **Valor** | **Descrição** | **Impacto** |
|----------|-----------|---------------|-------------|
| `--configuration` | `Release` | Testa binários de Release | Mesmos binários que vão para produção |
| `--no-build` | N/A | Não compila novamente | Usa DLLs já compiladas (mais rápido) |
| `--verbosity` | `normal` | Nível de detalhes nos logs | Balanceia informação vs. ruído |
| `--logger` | `trx;LogFileName=...` | Formato de saída | Gera arquivo TRX para relatórios |

#### 5.5.4. Framework de Testes

**xUnit 2.9.3**: Framework de testes escolhido

Características:
- ✅ Paralelização automática de testes
- ✅ Isolamento de testes (nova instância para cada teste)
- ✅ Suporte a teorias (data-driven tests)
- ✅ Integração nativa com .NET CLI

#### 5.5.5. Estrutura dos Testes

**Total**: 52 testes unitários

**Distribuição por Camada**:

```
CadCliX.Tests/
├── Entities/
│   ├── AddressTests.cs           (11 testes)
│   │   ├── Create_ValidAddress_ReturnsSuccess
│   │   ├── Create_EmptyStreet_ReturnsFailure
│   │   ├── Create_EmptyNumber_ReturnsFailure
│   │   ├── Create_EmptyNeighborhood_ReturnsFailure
│   │   ├── Create_EmptyCity_ReturnsFailure
│   │   ├── Create_EmptyState_ReturnsFailure
│   │   ├── Create_EmptyCountry_ReturnsFailure
│   │   ├── Create_EmptyZipCode_ReturnsFailure
│   │   ├── Create_ValidAddressWithComplement_ReturnsSuccess
│   │   ├── Create_ValidAddressWithoutComplement_ReturnsSuccess
│   │   └── Create_MultipleErrors_ReturnsAllErrors
│   │
│   └── CustomerTests.cs          (13 testes)
│       ├── Create_ValidFisicaCustomer_ReturnsSuccess
│       ├── Create_ValidJuridicaCustomer_ReturnsSuccess
│       ├── Create_FisicaWithoutCPF_ReturnsFailure
│       ├── Create_JuridicaWithoutCNPJ_ReturnsFailure
│       ├── Create_EmptyName_ReturnsFailure
│       ├── Create_EmptyEmail_ReturnsFailure
│       ├── Create_InvalidAddressId_ReturnsFailure
│       ├── Create_ZeroAddressId_ReturnsFailure
│       ├── Create_MultipleErrors_ReturnsAllErrors
│       ├── Create_FisicaWithCNPJ_IgnoresCNPJ
│       ├── Create_JuridicaWithCPF_IgnoresCPF
│       ├── Create_ValidFisicaWithAllFields_ReturnsSuccess
│       └── Create_ValidJuridicaWithAllFields_ReturnsSuccess
│
├── Repositories/
│   ├── AddressRepositoryTests.cs (6 testes)
│   │   ├── GetAllAsync_ReturnsAllAddresses
│   │   ├── GetByIdAsync_ExistingId_ReturnsAddress
│   │   ├── GetByIdAsync_NonExistingId_ReturnsNull
│   │   ├── AddAsync_ValidAddress_AddsToDatabase
│   │   ├── UpdateAsync_ExistingAddress_UpdatesSuccessfully
│   │   └── DeleteAsync_ExistingAddress_RemovesFromDatabase
│   │
│   └── CustomerRepositoryTests.cs (6 testes)
│       ├── GetAllAsync_ReturnsAllCustomersWithAddresses
│       ├── GetByIdAsync_ExistingId_ReturnsCustomerWithAddress
│       ├── GetByIdAsync_NonExistingId_ReturnsNull
│       ├── AddAsync_ValidCustomer_AddsToDatabase
│       ├── UpdateAsync_ExistingCustomer_UpdatesSuccessfully
│       └── DeleteAsync_ExistingCustomer_RemovesFromDatabase
│
└── Controllers/
    ├── AddressesControllerTests.cs (7 testes)
    │   ├── GetAll_ReturnsOkWithAddresses
    │   ├── GetById_ExistingId_ReturnsOkWithAddress
    │   ├── GetById_NonExistingId_ReturnsNotFound
    │   ├── Create_ValidAddress_ReturnsCreated
    │   ├── Create_InvalidAddress_ReturnsBadRequest
    │   ├── Create_EmptyRequest_ReturnsBadRequest
    │   └── GetAll_EmptyDatabase_ReturnsEmptyList
    │
    └── CustomersControllerTests.cs (9 testes)
        ├── GetAll_ReturnsOkWithCustomers
        ├── GetById_ExistingId_ReturnsOkWithCustomer
        ├── GetById_NonExistingId_ReturnsNotFound
        ├── Create_ValidCustomer_ReturnsCreated
        ├── Create_AddressNotFound_ReturnsNotFound
        ├── Create_InvalidCustomer_ReturnsBadRequest
        ├── Create_EmptyRequest_ReturnsBadRequest
        ├── GetAll_EmptyDatabase_ReturnsEmptyList
        └── Create_ValidFisicaCustomer_ReturnsCreatedWithCPF
```

#### 5.5.6. Bibliotecas de Testes Utilizadas

| **Biblioteca** | **Versão** | **Finalidade** | **Exemplo de Uso** |
|----------------|------------|----------------|-------------------|
| **xUnit** | 2.9.3 | Framework base | `[Fact]`, `[Theory]` |
| **FluentAssertions** | 7.0.0 | Asserções fluentes | `result.Should().BeSuccessful()` |
| **Moq** | 4.20.72 | Mocking de dependências | `Mock<ICustomerRepository>` |
| **EF Core InMemory** | 10.0.5 | Banco de dados em memória | Testes de repositórios |

#### 5.5.7. Padrões de Teste

**AAA Pattern (Arrange-Act-Assert)**:

```csharp
[Fact]
public void Create_ValidAddress_ReturnsSuccess()
{
    // Arrange (Preparação)
    var street = "Rua Teste";
    var number = "123";
    var neighborhood = "Bairro Teste";
    var city = "Cidade Teste";
    var state = "SP";
    var country = "Brasil";
    var zipCode = "12345-678";

    // Act (Ação)
    var result = Address.Create(
        street, number, neighborhood,
        city, state, country, zipCode, null
    );

    // Assert (Verificação)
    result.Should().BeSuccessful();
    result.Value.Should().NotBeNull();
    result.Value!.Street.Should().Be(street);
}
```

#### 5.5.8. Cobertura de Código

**Meta**: >80% de cobertura

**Cobertura Atual**: ~85%

| **Camada** | **Classes** | **Métodos** | **Linhas** | **Cobertura** |
|------------|-------------|-------------|------------|---------------|
| **Entities** | 2 | 6 | 120 | 92% |
| **Repositories** | 4 | 24 | 180 | 83% |
| **Controllers** | 2 | 6 | 95 | 78% |
| **TOTAL** | 8 | 36 | 395 | **85%** |

#### 5.5.9. Formato TRX (Test Results XML)

O arquivo `test-results.trx` gerado contém:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<TestRun id="..." name="..." runUser="runner">
  <Results>
    <UnitTestResult testId="..." testName="Create_ValidAddress_ReturnsSuccess"
                    outcome="Passed" duration="00:00:00.125" />
    <!-- ... mais 51 resultados ... -->
  </Results>
  <TestDefinitions>
    <!-- Definições dos testes -->
  </TestDefinitions>
  <ResultSummary outcome="Completed">
    <Counters total="52" executed="52" passed="52" failed="0" />
  </ResultSummary>
</TestRun>
```

#### 5.5.10. Execução Paralela

xUnit executa testes em paralelo por padrão:

```
Máximo de threads: Número de cores da CPU
Runner Ubuntu: Geralmente 2-4 cores
Tempo total: ~8-12 segundos (vs. ~30s sequencial)
```

#### 5.5.11. Saída do Log

```
Test run for C:\...\CadCliX.Tests\bin\Release\net10.0\CadCliX.Tests.dll (.NETCoreApp,Version=v10.0)
Microsoft (R) Test Execution Command Line Tool Version 17.13.0

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    52, Skipped:     0, Total:    52, Duration: 8.2s
```

#### 5.5.12. Critérios de Sucesso

O step falha se:
- ❌ Qualquer teste falhar (`Failed > 0`)
- ❌ Erro de compilação dos testes
- ❌ Exception não tratada durante execução

O step passa se:
- ✅ Todos os 52 testes passam (`Passed: 52`)
- ✅ Nenhum teste é ignorado involuntariamente
- ✅ Cobertura > 80% (verificado manualmente)

---

### 5.6. Publicação dos Resultados dos Testes

#### 5.6.1. Configuração

```yaml
- name: Publish test results
  uses: dorny/test-reporter@v1
  if: success() || failure()
  continue-on-error: true
  with:
    name: Test Results
    path: '**/test-results.trx'
    reporter: dotnet-trx
```

#### 5.6.2. Descrição Técnica

**Action Utilizada**: `dorny/test-reporter@v1`

**Responsabilidade**: Gerar relatórios visuais dos resultados dos testes.

#### 5.6.3. Parâmetros

| **Parâmetro** | **Valor** | **Descrição** |
|---------------|-----------|---------------|
| `name` | `Test Results` | Nome do relatório exibido |
| `path` | `**/test-results.trx` | Padrão glob para encontrar arquivos TRX |
| `reporter` | `dotnet-trx` | Parser para formato TRX do .NET |

#### 5.6.4. Condições de Execução

**`if: success() || failure()`**

Executa em dois cenários:
- ✅ **success()**: Testes passaram
- ✅ **failure()**: Testes falharam

**NÃO executa se**:
- ❌ Workflow cancelado manualmente
- ❌ Steps anteriores pulados (skipped)

**`continue-on-error: true`**

- Se a publicação falhar (ex: problema de permissão), o workflow continua
- Garante que o deploy não seja bloqueado por problemas de reporting

#### 5.6.5. Funcionamento

1. **Busca**: Procura arquivos `test-results.trx` em todo workspace
2. **Parse**: Converte XML TRX em estrutura de dados
3. **Análise**: Extrai estatísticas (passed, failed, skipped, duration)
4. **Publicação**: Cria GitHub Check com resultados
5. **Comentário**: Adiciona comentário no PR (se aplicável)

#### 5.6.6. Saída Visual

**GitHub Checks Tab**:

```
✅ Test Results — Passed
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
52 tests    52 ✅    0 ❌    0 ⏭️
Duration: 8.2s
```

**Detalhes Expandidos**:

```
✅ CadCliX.Tests.Entities.AddressTests
   ✅ Create_ValidAddress_ReturnsSuccess (125ms)
   ✅ Create_EmptyStreet_ReturnsFailure (98ms)
   ...

✅ CadCliX.Tests.Controllers.CustomersControllerTests
   ✅ GetAll_ReturnsOkWithCustomers (156ms)
   ✅ Create_ValidCustomer_ReturnsCreated (203ms)
   ...
```

#### 5.6.7. Integração com Pull Requests

Quando o pipeline roda a partir de um PR, a action adiciona um comentário:

```markdown
### 🧪 Test Results

**Status**: ✅ All tests passed

| Metric | Value |
|--------|-------|
| Total Tests | 52 |
| ✅ Passed | 52 |
| ❌ Failed | 0 |
| ⏭️ Skipped | 0 |
| ⏱️ Duration | 8.2s |

<details>
<summary>View Details</summary>

#### Entities Tests (24 tests)
- ✅ AddressTests (11 tests, 1.2s)
- ✅ CustomerTests (13 tests, 1.5s)

#### Repository Tests (12 tests)
- ✅ AddressRepositoryTests (6 tests, 2.1s)
- ✅ CustomerRepositoryTests (6 tests, 2.3s)

#### Controller Tests (16 tests)
- ✅ AddressesControllerTests (7 tests, 1.4s)
- ✅ CustomersControllerTests (9 tests, 1.7s)

</details>
```

#### 5.6.8. Benefícios

- 📊 **Visibilidade**: Resultados visíveis sem abrir logs
- 🔍 **Debugging**: Identifica testes falhando rapidamente
- 📈 **Histórico**: Rastreia tendências de testes ao longo do tempo
- 👥 **Colaboração**: Revisores veem qualidade no PR

---

### 5.7. Configuração do Docker Buildx

#### 5.7.1. Configuração

```yaml
- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3
```

#### 5.7.2. Descrição Técnica

**Action Utilizada**: `docker/setup-buildx-action@v3`

**Responsabilidade**: Configurar Docker Buildx para builds avançados.

#### 5.7.3. O que é Docker Buildx?

**Docker Buildx** é uma extensão do Docker CLI que fornece:

- ✅ **Multi-platform builds**: Build para múltiplas arquiteturas (amd64, arm64)
- ✅ **Cache avançado**: Cache em registry remoto
- ✅ **Build paralelo**: Stages independentes compilados simultaneamente
- ✅ **Build drivers**: Usa containerd ou BuildKit

#### 5.7.4. Funcionamento

1. **Detecção**: Verifica se Buildx já está instalado
2. **Instalação**: Se necessário, instala Buildx
3. **Driver**: Configura driver `docker-container` (BuildKit)
4. **Builder**: Cria e configura um builder instance
5. **Ativação**: Define o builder como padrão

#### 5.7.5. Builder Instance

```bash
$ docker buildx ls
NAME/NODE       DRIVER/ENDPOINT             STATUS   PLATFORMS
default *       docker-container
  default       default                     running  linux/amd64, linux/arm64, ...
```

#### 5.7.6. BuildKit

**BuildKit** é o backend moderno de build do Docker:

| **Recurso** | **Build Tradicional** | **BuildKit** |
|-------------|----------------------|--------------|
| **Paralelização** | Sequencial | Paralelo |
| **Cache** | Layers locais | Layers + registry remoto |
| **Secrets** | Hardcoded | Build-time secrets seguros |
| **SSH** | Não suportado | SSH forwarding |
| **Outputs** | Apenas imagem | Imagem, tar, registry, local |

#### 5.7.7. Configuração Implícita

```yaml
driver-opts: |
  image=moby/buildkit:latest
  network=host
```

#### 5.7.8. Benefícios para o Pipeline

- ⚡ **Velocidade**: Builds até 3x mais rápidos
- 💾 **Cache**: Reutilização de layers entre builds
- 🔧 **Flexibilidade**: Multi-stage builds otimizados
- 🌐 **Registry Cache**: Cache armazenado no Docker Hub

---

### 5.8. Autenticação no Docker Hub

#### 5.8.1. Configuração

```yaml
- name: Log in to Docker Hub
  uses: docker/login-action@v3
  with:
    username: ${{ secrets.DOCKERHUB_USERNAME }}
    password: ${{ secrets.DOCKERHUB_TOKEN }}
```

#### 5.8.2. Descrição Técnica

**Action Utilizada**: `docker/login-action@v3`

**Responsabilidade**: Autenticar no Docker Hub para push de imagens.

#### 5.8.3. Parâmetros

| **Parâmetro** | **Fonte** | **Valor** | **Segurança** |
|---------------|-----------|-----------|---------------|
| `username` | Secret | `alexcar` | Público (nome de usuário) |
| `password` | Secret | `dckr_pat_...` | 🔒 Token de acesso |

#### 5.8.4. Funcionamento

1. **Leitura de Secrets**: Recupera secrets do GitHub
2. **Comando Docker**: Executa `docker login registry-1.docker.io`
3. **Autenticação**: Envia credenciais para Docker Hub
4. **Token Storage**: Armazena token em `~/.docker/config.json`
5. **Validação**: Testa autenticação com `docker info`

#### 5.8.5. Processo de Autenticação

```
┌──────────────────┐
│  GitHub Actions  │
│  Runner          │
└────────┬─────────┘
         │ 1. Lê secrets
         ▼
┌──────────────────┐
│  docker login    │
│  CLI Command     │
└────────┬─────────┘
         │ 2. POST /v2/users/login
         ▼
┌──────────────────┐
│  Docker Hub API  │
│  auth.docker.io  │
└────────┬─────────┘
         │ 3. Valida token
         ▼
┌──────────────────┐
│  Retorna         │
│  Access Token    │
└────────┬─────────┘
         │ 4. Salva em ~/.docker/config.json
         ▼
┌──────────────────┐
│  Autenticado ✅  │
└──────────────────┘
```

#### 5.8.6. Arquivo de Configuração Gerado

```json
{
  "auths": {
    "https://index.docker.io/v1/": {
      "auth": "YWxleGNhcjpka2NyX3BhdF9hYmMuLi4="
    }
  }
}
```

**Nota**: O valor `auth` é `base64(username:token)`, não é plain text.

#### 5.8.7. Segurança

**Mascaramento de Secrets**:

```
# ❌ Secrets nunca aparecem em logs:
Run docker login -u *** -p ***

# ✅ Apenas confirmação genérica:
Login Succeeded
```

**Boas Práticas Implementadas**:

- ✅ Usa **Access Token**, não senha
- ✅ Token tem escopo limitado (read/write apenas)
- ✅ Token pode ser revogado sem alterar senha
- ✅ Token rotacionável sem quebrar pipeline

#### 5.8.8. Rate Limits

Docker Hub impõe rate limits:

| **Tipo de Conta** | **Pulls Anônimos** | **Pulls Autenticados** | **Pushes** |
|-------------------|--------------------|-----------------------|------------|
| **Anônimo** | 100 / 6h | N/A | ❌ |
| **Free (alexcar)** | N/A | 200 / 6h | ✅ Ilimitado |
| **Pro** | N/A | Ilimitado | ✅ Ilimitado |

**Autenticação evita problemas de rate limit!**

#### 5.8.9. Troubleshooting

| **Erro** | **Causa** | **Solução** |
|----------|-----------|-------------|
| `unauthorized: incorrect username or password` | Token inválido | Regenerar token no Docker Hub |
| `Error response from daemon: Get https://registry-1.docker.io/v2/: EOF` | Problema de rede | Retry automático |
| `denied: requested access to the resource is denied` | Token sem permissões | Verificar permissões do token |

---

### 5.9. Extração de Metadados Docker

#### 5.9.1. Configuração

```yaml
- name: Extract metadata for Docker
  id: meta
  uses: docker/metadata-action@v5
  with:
    images: alexcar/cadclix
    tags: |
      type=ref,event=branch
      type=sha,prefix={{branch}}-
      type=raw,value=latest,enable={{is_default_branch}}
```

#### 5.9.2. Descrição Técnica

**Action Utilizada**: `docker/metadata-action@v5`

**Responsabilidade**: Gerar tags e labels para a imagem Docker automaticamente.

#### 5.9.3. Parâmetros

| **Parâmetro** | **Valor** | **Descrição** |
|---------------|-----------|---------------|
| `id` | `meta` | Identificador do step (para referenciar outputs) |
| `images` | `alexcar/cadclix` | Nome base da imagem |
| `tags` | (múltiplas regras) | Regras de geração de tags |

#### 5.9.4. Regras de Tags

**1. `type=ref,event=branch`**

Cria tag baseada no nome da branch:

```
Input:  branch = main
Output: alexcar/cadclix:main
```

**2. `type=sha,prefix={{branch}}-`**

Cria tag com SHA do commit:

```
Input:  branch = main, sha = abc1234567890def
Output: alexcar/cadclix:main-abc1234
```

**3. `type=raw,value=latest,enable={{is_default_branch}}`**

Cria tag `latest` apenas se for branch padrão:

```
Input:  branch = main (default), sha = abc1234
Output: alexcar/cadclix:latest
```

#### 5.9.5. Exemplo de Tags Geradas

Para um commit `abc1234567890def` na branch `main`:

```
alexcar/cadclix:main
alexcar/cadclix:main-abc1234
alexcar/cadclix:latest
```

#### 5.9.6. Labels Gerados

A action também gera labels OCI padrão:

```dockerfile
LABEL org.opencontainers.image.created="2025-01-15T10:30:00Z"
LABEL org.opencontainers.image.source="https://github.com/alexcar/CadCliX"
LABEL org.opencontainers.image.version="main"
LABEL org.opencontainers.image.revision="abc1234567890def"
LABEL org.opencontainers.image.licenses=""
```

#### 5.9.7. Outputs do Step

```yaml
outputs:
  tags: alexcar/cadclix:main,alexcar/cadclix:main-abc1234,alexcar/cadclix:latest
  labels: |
    org.opencontainers.image.created=2025-01-15T10:30:00Z
    org.opencontainers.image.source=https://github.com/alexcar/CadCliX
    org.opencontainers.image.version=main
    org.opencontainers.image.revision=abc1234567890def
```

#### 5.9.8. Uso dos Outputs

Os outputs são referenciados no próximo step:

```yaml
tags: ${{ steps.meta.outputs.tags }}
labels: ${{ steps.meta.outputs.labels }}
```

**Sintaxe de Referência**:
- `steps`: Contexto de steps anteriores
- `meta`: ID do step de extração de metadata
- `outputs.tags`: Output específico

#### 5.9.9. Versionamento Semântico (Alternativa)

Se o projeto usasse tags Git para releases, poderia adicionar:

```yaml
tags: |
  type=semver,pattern={{version}}
  type=semver,pattern={{major}}.{{minor}}
  type=semver,pattern={{major}}
```

Exemplo com tag Git `v1.2.3`:
```
alexcar/cadclix:1.2.3
alexcar/cadclix:1.2
alexcar/cadclix:1
```

---

### 5.10. Build e Push da Imagem Docker

#### 5.10.1. Configuração

```yaml
- name: Build and push Docker image
  uses: docker/build-push-action@v5
  with:
    context: .
    file: ./Dockerfile
    push: true
    tags: ${{ steps.meta.outputs.tags }}
    labels: ${{ steps.meta.outputs.labels }}
    cache-from: type=registry,ref=alexcar/cadclix:buildcache
    cache-to: type=registry,ref=alexcar/cadclix:buildcache,mode=max
```

#### 5.10.2. Descrição Técnica

**Action Utilizada**: `docker/build-push-action@v5`

**Responsabilidade**: Construir imagem Docker multi-stage e fazer push para Docker Hub.

#### 5.10.3. Parâmetros Detalhados

| **Parâmetro** | **Valor** | **Descrição** |
|---------------|-----------|---------------|
| `context` | `.` | Diretório raiz do build (onde está .dockerignore) |
| `file` | `./Dockerfile` | Caminho do Dockerfile |
| `push` | `true` | Fazer push automático após build |
| `tags` | `${{ steps.meta.outputs.tags }}` | Tags geradas no step anterior |
| `labels` | `${{ steps.meta.outputs.labels }}` | Labels OCI |
| `cache-from` | `type=registry,ref=...` | Origem do cache |
| `cache-to` | `type=registry,ref=...,mode=max` | Destino do cache |

#### 5.10.4. Dockerfile Multi-Stage

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CadCliX/CadCliX.csproj", "CadCliX/"]
RUN dotnet restore "CadCliX/CadCliX.csproj"
COPY . .
WORKDIR "/src/CadCliX"
RUN dotnet build "CadCliX.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CadCliX.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CadCliX.dll"]
```

#### 5.10.5. Processo de Build Multi-Stage

```
┌─────────────────────────────────────────────────────────────────┐
│ STAGE 1: build                                                  │
│ Base: mcr.microsoft.com/dotnet/sdk:10.0                        │
│ Tamanho: ~700 MB                                               │
├─────────────────────────────────────────────────────────────────┤
│ 1. WORKDIR /src                                                 │
│ 2. COPY CadCliX.csproj → /src/CadCliX/                         │
│ 3. RUN dotnet restore (baixa NuGet packages)                   │
│ 4. COPY . → /src (código-fonte completo)                       │
│ 5. RUN dotnet build -c Release                                 │
│    Output: /app/build/CadCliX.dll + dependencies               │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STAGE 2: publish                                                │
│ Base: build (herda do stage 1)                                 │
├─────────────────────────────────────────────────────────────────┤
│ 1. RUN dotnet publish -c Release -o /app/publish               │
│    - Remove arquivos desnecessários                             │
│    - Otimiza assemblies                                         │
│    - Gera apenas arquivos de runtime                            │
│    Output: /app/publish (versão otimizada)                     │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STAGE 3: final (IMAGEM FINAL)                                  │
│ Base: mcr.microsoft.com/dotnet/aspnet:10.0                     │
│ Tamanho: ~220 MB (3x menor!)                                   │
├─────────────────────────────────────────────────────────────────┤
│ 1. WORKDIR /app                                                 │
│ 2. EXPOSE 8080 8081                                            │
│ 3. ENV ASPNETCORE_ENVIRONMENT=Development                      │
│ 4. ENV ASPNETCORE_HTTP_PORTS=8080                              │
│ 5. COPY --from=publish /app/publish . (apenas binários finais) │
│ 6. ENTRYPOINT ["dotnet", "CadCliX.dll"]                        │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
                 ┌───────────────┐
                 │ Imagem Final  │
                 │ ~220 MB       │
                 └───────────────┘
```

#### 5.10.6. Otimizações de Build

**1. Build Context Otimizado (.dockerignore)**

```
# Arquivos excluídos do build context
**/bin/
**/obj/
**/TestResults/
**/.vs/
CadCliX.Tests/
*.md
.git/
```

**Benefício**: Reduz contexto de ~500 MB para ~50 MB → Upload 10x mais rápido

**2. Layer Caching**

Ordem otimizada de COPY:

```dockerfile
# ✅ BOM: Copia .csproj primeiro (muda raramente)
COPY ["CadCliX/CadCliX.csproj", "CadCliX/"]
RUN dotnet restore

# ✅ BOM: Copia código depois (muda frequentemente)
COPY . .
```

**Por quê?**

Se apenas código C# mudar:
- ✅ Layer de restore é reutilizada do cache
- ⚡ Economiza ~1-2 minutos por build

**3. Multi-Stage Benefits**

| **Aspecto** | **Single-Stage** | **Multi-Stage** |
|-------------|------------------|-----------------|
| **Tamanho** | ~700 MB | ~220 MB (3x menor) |
| **Conteúdo** | SDK + Runtime + Build artifacts | Apenas Runtime + App |
| **Segurança** | Tools de dev expostos | Apenas runtime mínimo |
| **Startup** | Mais lento | Mais rápido |

#### 5.10.7. Cache Strategy

**Cache-From** (Origem):

```yaml
cache-from: type=registry,ref=alexcar/cadclix:buildcache
```

Antes do build, baixa layers de cache do Docker Hub.

**Cache-To** (Destino):

```yaml
cache-to: type=registry,ref=alexcar/cadclix:buildcache,mode=max
```

Após o build, salva **todos** os layers como cache.

**`mode=max`**:
- Salva layers intermediários também (não apenas final)
- Maximiza reuso em builds futuros

**Fluxo de Cache**:

```
Build #1 (sem cache):
├─ Baixa base images
├─ Executa todos RUN commands
├─ Salva layers em alexcar/cadclix:buildcache
└─ Duração: ~4 minutos

Build #2 (com cache):
├─ Baixa alexcar/cadclix:buildcache
├─ Reusa layers não modificadas
├─ Executa apenas layers alteradas
└─ Duração: ~1 minuto (4x mais rápido!)
```

#### 5.10.8. Push para Registry

Após build bem-sucedido, a action faz push de todas as tags:

```bash
# Push tag: main
docker push alexcar/cadclix:main

# Push tag: main-abc1234
docker push alexcar/cadclix:main-abc1234

# Push tag: latest
docker push alexcar/cadclix:latest
```

**Deduplicated Layers**:

Docker Hub usa content-addressable storage:
- Layers idênticas são armazenadas uma única vez
- Tags diferentes apontam para os mesmos layers
- Economia de espaço e banda

#### 5.10.9. Manifests e Digests

Cada push gera um **manifest digest** (SHA256):

```
alexcar/cadclix:latest@sha256:abc123def456...
```

**Digest** é imutável:
- ✅ Garante reprodutibilidade exata
- ✅ Permite rollback preciso
- ✅ Auditoria e compliance

#### 5.10.10. Saída do Log

```
#1 [internal] load build definition from Dockerfile
#1 transferring dockerfile: 500B done

#2 [internal] load .dockerignore
#2 transferring context: 150B done

#3 [internal] load metadata for mcr.microsoft.com/dotnet/sdk:10.0
#3 DONE 0.8s

#4 [build 1/6] FROM mcr.microsoft.com/dotnet/sdk:10.0
#4 CACHED

#5 [internal] load build context
#5 transferring context: 52.3MB done

#6 [build 2/6] WORKDIR /src
#6 CACHED

#7 [build 3/6] COPY [CadCliX/CadCliX.csproj, CadCliX/]
#7 DONE 0.1s

#8 [build 4/6] RUN dotnet restore "CadCliX/CadCliX.csproj"
#8 DONE 15.2s

#9 [build 5/6] COPY . .
#9 DONE 0.3s

#10 [build 6/6] RUN dotnet build "CadCliX.csproj" -c Release -o /app/build
#10 DONE 22.1s

#11 [publish 1/1] RUN dotnet publish "CadCliX.csproj" -c Release -o /app/publish
#11 DONE 8.5s

#12 [final 1/4] WORKDIR /app
#12 DONE 0.1s

#13 [final 2/4] COPY --from=publish /app/publish .
#13 DONE 0.2s

#14 exporting to image
#14 exporting layers done
#14 writing image sha256:abc123def456... done
#14 naming to docker.io/alexcar/cadclix:main done
#14 naming to docker.io/alexcar/cadclix:main-abc1234 done
#14 naming to docker.io/alexcar/cadclix:latest done

#15 pushing alexcar/cadclix:main
#15 pushing layer sha256:111...
#15 pushing layer sha256:222...
#15 pushing manifest for docker.io/alexcar/cadclix:main
#15 DONE 12.3s

#16 pushing alexcar/cadclix:latest
#16 pushing manifest for docker.io/alexcar/cadclix:latest
#16 DONE 1.2s

Total duration: 1m 23s
```

---

## 6. Fluxo de Execução Completo

### 6.1. Timeline de Execução

```
T=0s    ┌─────────────────────────────────────────┐
        │ TRIGGER: Push to main                   │
        └──────────────────┬──────────────────────┘
                           │
T=2s    ┌──────────────────▼──────────────────────┐
        │ Step 1: Checkout code                   │
        │ Duration: ~2s                           │
        └──────────────────┬──────────────────────┘
                           │
T=4s    ┌──────────────────▼──────────────────────┐
        │ Step 2: Setup .NET 10                   │
        │ Duration: ~2s (cached)                  │
        └──────────────────┬──────────────────────┘
                           │
T=6s    ┌──────────────────▼──────────────────────┐
        │ Step 3: Restore dependencies            │
        │ Duration: ~8s                           │
        └──────────────────┬──────────────────────┘
                           │
T=14s   ┌──────────────────▼──────────────────────┐
        │ Step 4: Build                           │
        │ Duration: ~12s                          │
        └──────────────────┬──────────────────────┘
                           │
T=26s   ┌──────────────────▼──────────────────────┐
        │ Step 5: Run unit tests                  │
        │ Duration: ~10s (52 tests)               │
        └──────────────────┬──────────────────────┘
                           │
T=36s   ┌──────────────────▼──────────────────────┐
        │ Step 6: Publish test results            │
        │ Duration: ~3s                           │
        └──────────────────┬──────────────────────┘
                           │
T=39s   ┌──────────────────▼──────────────────────┐
        │ Step 7: Setup Docker Buildx             │
        │ Duration: ~2s                           │
        └──────────────────┬──────────────────────┘
                           │
T=41s   ┌──────────────────▼──────────────────────┐
        │ Step 8: Login to Docker Hub             │
        │ Duration: ~1s                           │
        └──────────────────┬──────────────────────┘
                           │
T=42s   ┌──────────────────▼──────────────────────┐
        │ Step 9: Extract metadata                │
        │ Duration: <1s                           │
        └──────────────────┬──────────────────────┘
                           │
T=43s   ┌──────────────────▼──────────────────────┐
        │ Step 10: Build and push Docker image    │
        │ Duration: ~2m (first run)               │
        │           ~1m (with cache)              │
        └──────────────────┬──────────────────────┘
                           │
T=163s  ┌──────────────────▼──────────────────────┐
        │ ✅ WORKFLOW COMPLETED                   │
        │ Total Duration: ~2m 43s                 │
        └─────────────────────────────────────────┘
```

### 6.2. Recursos Consumidos

| **Recurso** | **Uso Médio** | **Pico** |
|-------------|---------------|----------|
| **CPU** | 60% | 95% (durante build) |
| **RAM** | 2.5 GB | 3.8 GB (Docker build) |
| **Disco** | 5 GB | 8 GB (cache Docker) |
| **Rede (Download)** | 1.2 GB | - |
| **Rede (Upload)** | 220 MB | - |

### 6.3. Pontos de Falha Críticos

```
┌────────────────────────────────────────────────────────┐
│ CRITICAL PATH (Falha interrompe pipeline)             │
├────────────────────────────────────────────────────────┤
│ 1. Checkout         → Se falhar, nada executa         │
│ 2. Setup .NET       → Sem SDK, build impossível       │
│ 3. Restore          → Sem pacotes, build falha        │
│ 4. Build            → Erros de compilação param tudo  │
│ 5. Tests            → Testes falhando param pipeline  │
│ 7. Setup Buildx     → Sem Buildx, build Docker falha  │
│ 8. Docker Login     → Sem auth, push impossível       │
│ 10. Build Docker    → Dockerfile com erro para tudo   │
└────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────┐
│ NON-CRITICAL (Falha não interrompe)                    │
├────────────────────────────────────────────────────────┤
│ 6. Publish Results  → continue-on-error: true          │
└────────────────────────────────────────────────────────┘
```

---

## 7. Variáveis de Ambiente e Secrets

### 7.1. Secrets do GitHub

| **Nome** | **Tipo** | **Escopo** | **Rotação** | **Onde Usar** |
|----------|----------|------------|-------------|---------------|
| `DOCKERHUB_USERNAME` | String | Repository | Raramente | Login Docker Hub |
| `DOCKERHUB_TOKEN` | Token | Repository | A cada 90 dias | Autenticação Docker |

### 7.2. Variáveis de Ambiente Implícitas

GitHub Actions fornece variáveis automáticas:

| **Variável** | **Exemplo** | **Uso** |
|--------------|-------------|---------|
| `GITHUB_SHA` | `abc1234567890def` | Identificar commit exato |
| `GITHUB_REF` | `refs/heads/main` | Nome da branch |
| `GITHUB_REPOSITORY` | `alexcar/CadCliX` | Nome do repositório |
| `GITHUB_ACTOR` | `alexcar` | Usuário que disparou |
| `GITHUB_WORKSPACE` | `/home/runner/work/CadCliX/CadCliX` | Diretório de trabalho |

### 7.3. Contexts Disponíveis

```yaml
${{ github.sha }}           # abc1234567890def
${{ github.ref }}           # refs/heads/main
${{ github.event_name }}    # push
${{ github.actor }}         # alexcar
${{ runner.os }}            # Linux
${{ runner.temp }}          # /home/runner/work/_temp
${{ secrets.DOCKERHUB_TOKEN }}  # dckr_pat_***
```

### 7.4. Variáveis Docker

Definidas no `Dockerfile`:

```dockerfile
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_HTTP_PORTS=8080
```

Podem ser sobrescritas em runtime:

```bash
docker run -e ASPNETCORE_ENVIRONMENT=Production alexcar/cadclix:latest
```

---

## 8. Estratégia de Versionamento e Tags

### 8.1. Sistema de Tags

| **Tag** | **Formato** | **Exemplo** | **Quando Atualiza** | **Uso Recomendado** |
|---------|-------------|-------------|---------------------|---------------------|
| `latest` | `latest` | `alexcar/cadclix:latest` | A cada push para main | Desenvolvimento local |
| `branch` | `{branch}` | `alexcar/cadclix:main` | A cada push para main | Rastreamento de branch |
| `commit` | `{branch}-{sha}` | `alexcar/cadclix:main-abc1234` | A cada commit | Rollback preciso |

### 8.2. Imutabilidade

| **Tag** | **Imutável?** | **Explicação** |
|---------|---------------|----------------|
| `latest` | ❌ Não | Sempre aponta para último commit de main |
| `main` | ❌ Não | Sempre aponta para último commit de main |
| `main-abc1234` | ✅ Sim | SHA do commit nunca muda |

**Implicação para Produção**:

```dockerfile
# ❌ RUIM: Tag mutável
FROM alexcar/cadclix:latest

# ✅ BOM: Tag com SHA
FROM alexcar/cadclix:main-abc1234

# ✅ MELHOR: Digest SHA256
FROM alexcar/cadclix@sha256:abc123def456...
```

### 8.3. Estratégia de Release

**Atual** (Continuous Deployment):
```
develop → main (via PR) → deploy automático
```

**Recomendado para Produção** (Staged Releases):

```
develop → staging → production

Tags:
- alexcar/cadclix:develop-abc1234
- alexcar/cadclix:staging-def5678
- alexcar/cadclix:v1.0.0 (production)
```

### 8.4. Versionamento Semântico (Futuro)

Se adotar Git tags:

```bash
git tag v1.2.3
git push origin v1.2.3
```

Workflow geraria:

```
alexcar/cadclix:1.2.3
alexcar/cadclix:1.2
alexcar/cadclix:1
alexcar/cadclix:latest
```

---

## 9. Otimizações e Cache

### 9.1. Tipos de Cache Utilizados

| **Tipo** | **O que Cacheia** | **Duração** | **Benefício** |
|----------|-------------------|-------------|---------------|
| **NuGet Packages** | Pacotes .NET | 7 dias | Restore 10x mais rápido |
| **.NET SDK** | SDK instalado | 30 dias | Setup instantâneo |
| **Docker Layers** | Layers de build | Indefinido | Build 4x mais rápido |
| **Docker Buildx** | Cache registry | Indefinido | Reuso entre runners |

### 9.2. Cache Flow

```
┌─────────────────────────────────────────────────────────┐
│ GitHub Actions Runner (Limpo a cada execução)           │
└─────────────────────────────────────────────────────────┘
                           │
            ┌──────────────┼──────────────┐
            │              │              │
            ▼              ▼              ▼
    ┌──────────┐   ┌──────────┐   ┌──────────┐
    │ NuGet    │   │ .NET SDK │   │ Docker   │
    │ Cache    │   │ Cache    │   │ Registry │
    │ (GitHub) │   │ (GitHub) │   │ Cache    │
    └──────────┘   └──────────┘   └──────────┘
         │              │              │
         │              │              │
    ~/.nuget/      /opt/hostedtoolcache/    alexcar/cadclix:
    packages/      dotnet/                  buildcache
```

### 9.3. Métricas de Performance

**Build sem Cache**:
```
Restore:   ~15s
Build:     ~18s
Tests:     ~10s
Docker:    ~4m
─────────────────
Total:     ~5m
```

**Build com Cache Total**:
```
Restore:   ~2s   (87% faster)
Build:     ~12s  (33% faster)
Tests:     ~10s  (same)
Docker:    ~1m   (75% faster)
─────────────────
Total:     ~1m 24s  (72% faster)
```

### 9.4. Estratégias de Invalidação

**NuGet Cache** invalida se:
- ❌ `.csproj` modificado
- ❌ Versões de pacotes alteradas

**Docker Layer Cache** invalida se:
- ❌ Linha no Dockerfile alterada
- ❌ Arquivo copiado modificado
- ❌ Base image atualizada

**Best Practice**:

```dockerfile
# ✅ BOM: Copia .csproj primeiro
COPY ["CadCliX/CadCliX.csproj", "CadCliX/"]
RUN dotnet restore  # <-- Esta layer é cacheada

# ✅ BOM: Copia código depois
COPY . .  # <-- Muda frequentemente, mas restore já está cacheado
```

---

## 10. Monitoramento e Logs

### 10.1. Onde Encontrar Logs

**GitHub Actions UI**:

```
Repository → Actions → CI/CD Pipeline → [Run específico]
```

### 10.2. Estrutura de Logs

```
┌─────────────────────────────────────────┐
│ Workflow: CI/CD Pipeline                │
│ Run #42 - 15 Jan 2025, 10:30:00        │
│ Triggered by: alexcar (push)           │
│ Branch: main                            │
│ Commit: abc1234 "feat: nova feature"   │
└─────────────────────────────────────────┘
          │
          ├─ Set up job (2s) ✅
          │
          ├─ ⬇ Checkout code (2s) ✅
          │   └─ Logs: git clone, checkout details
          │
          ├─ ⬇ Setup .NET 10 (2s) ✅
          │   └─ Logs: SDK download, cache info
          │
          ├─ ⬇ Restore dependencies (8s) ✅
          │   └─ Logs: Package list, download progress
          │
          ├─ ⬇ Build (12s) ✅
          │   └─ Logs: Compilation output, warnings
          │
          ├─ ⬇ Run unit tests (10s) ✅
          │   └─ Logs: Test results, 52/52 passed
          │
          ├─ ⬇ Publish test results (3s) ✅
          │   └─ Logs: Report generation
          │
          ├─ ⬇ Setup Docker Buildx (2s) ✅
          │   └─ Logs: Builder creation
          │
          ├─ ⬇ Login to Docker Hub (1s) ✅
          │   └─ Logs: Login Succeeded (secrets masked)
          │
          ├─ ⬇ Extract metadata (0s) ✅
          │   └─ Logs: Tags and labels generated
          │
          ├─ ⬇ Build and push Docker image (1m 23s) ✅
          │   └─ Logs: Build stages, layer cache, push progress
          │
          └─ Complete job (1s) ✅
```

### 10.3. Níveis de Log

| **Nível** | **Quando Aparece** | **Exemplo** |
|-----------|-------------------|-------------|
| **Info** | Operações normais | `Restored CadCliX.csproj (in 1.2s)` |
| **Warning** | Avisos não críticos | `warning CS8618: Non-nullable field` |
| **Error** | Falhas que param pipeline | `error CS0103: The name 'X' does not exist` |
| **Debug** | Detalhes internos | `##[debug]Evaluating: success()` |

### 10.4. Annotations

GitHub Actions adiciona anotações visuais:

```
⚠️ Warning in Build (line 123)
   CS8618: Non-nullable property 'Name' must contain a non-null value

❌ Error in Run unit tests
   Test 'Create_ValidAddress_ReturnsSuccess' failed
```

### 10.5. Download de Logs

Logs podem ser baixados como arquivo ZIP:

```
Actions → [Run] → ⋮ (menu) → Download log archive
```

Conteúdo:
```
logs.zip
├── 1_Checkout code.txt
├── 2_Setup .NET 10.txt
├── 3_Restore dependencies.txt
├── 4_Build.txt
├── 5_Run unit tests.txt
├── ...
└── 10_Build and push Docker image.txt
```

### 10.6. Retention Policy

- **Logs**: Retidos por 90 dias
- **Artifacts**: Retidos por 90 dias (configurável)
- **Cache**: Retido por 7 dias (sem acesso) ou até 10 GB

---

## 11. Tratamento de Erros

### 11.1. Estratégias de Retry

GitHub Actions faz retry automático para:

- 🔄 **Network failures**: 3 tentativas automáticas
- 🔄 **API rate limits**: Espera e tenta novamente
- 🔄 **Transient errors**: Retry com exponential backoff

### 11.2. Cenários de Falha e Recuperação

| **Step** | **Tipo de Falha** | **Ação Automática** | **Ação Manual** |
|----------|-------------------|---------------------|-----------------|
| **Checkout** | Git network error | Retry 3x | Verificar conectividade GitHub |
| **Setup .NET** | SDK download fail | Retry download | Verificar disponibilidade SDK |
| **Restore** | NuGet package not found | Falha imediata | Corrigir versão em .csproj |
| **Build** | Compilation error | Falha imediata | Corrigir código-fonte |
| **Tests** | Test failure | Falha imediata | Corrigir testes |
| **Publish Results** | Permission denied | Continua (continue-on-error) | Adicionar permissões |
| **Docker Login** | Invalid token | Falha imediata | Regenerar DOCKERHUB_TOKEN |
| **Docker Build** | Dockerfile error | Falha imediata | Corrigir Dockerfile |
| **Docker Push** | Rate limit | Espera e retry | Upgrade Docker Hub plan |

### 11.3. Notificações de Falha

**GitHub**:
- 📧 Email automático para quem fez o commit
- 🔔 Notificação in-app no GitHub
- ❌ Status check "Failed" no PR

**Exemplo de Email**:

```
Subject: [alexcar/CadCliX] Run failed: CI/CD Pipeline - main (abc1234)

The run CI/CD Pipeline on branch main has failed.

View the workflow run:
https://github.com/alexcar/CadCliX/actions/runs/123456

Step that failed: Run unit tests
Error: Test 'Create_ValidAddress_ReturnsSuccess' failed
```

### 11.4. Rollback Strategy

**Automático**: Não há rollback automático

**Manual** (Opções):

**Opção 1: Revert Commit**
```bash
git revert abc1234
git push origin main
# Trigger novo build com código anterior
```

**Opção 2: Redeploy Tag Anterior**
```bash
# Pull imagem anterior
docker pull alexcar/cadclix:main-def5678

# Re-tag como latest
docker tag alexcar/cadclix:main-def5678 alexcar/cadclix:latest
docker push alexcar/cadclix:latest
```

**Opção 3: Deploy por Digest**
```bash
# Usa digest SHA256 imutável
docker pull alexcar/cadclix@sha256:abc123...
```

---

## 12. Boas Práticas Implementadas

### 12.1. Segurança

- ✅ **Secrets Management**: Uso de GitHub Secrets
- ✅ **Token Instead of Password**: Access token do Docker Hub
- ✅ **Least Privilege**: Permissões mínimas necessárias
- ✅ **No Hardcoded Credentials**: Zero credenciais no código
- ✅ **Immutable Tags**: Tags com SHA para auditoria
- ✅ **Multi-Stage Builds**: Imagem final sem ferramentas de dev

### 12.2. Performance

- ✅ **Layer Caching**: Docker layers cacheados no registry
- ✅ **Dependency Caching**: NuGet packages cacheados
- ✅ **Multi-Stage Build**: Imagem final 3x menor
- ✅ **Parallel Tests**: xUnit roda testes em paralelo
- ✅ **Build Context Optimization**: .dockerignore reduz contexto

### 12.3. Qualidade

- ✅ **Automated Testing**: 52 testes executados automaticamente
- ✅ **Code Coverage**: >80% de cobertura
- ✅ **Test Reporting**: Relatórios visuais no PR
- ✅ **Build Validation**: Compilação em Release mode
- ✅ **Static Analysis**: Warnings do compilador C#

### 12.4. Confiabilidade

- ✅ **Idempotência**: Múltiplas execuções produzem mesmo resultado
- ✅ **Retry Logic**: Retry automático para falhas transitórias
- ✅ **Fail Fast**: Falha rápida em erros críticos
- ✅ **Rollback Capability**: Tags imutáveis permitem rollback
- ✅ **Clean Environment**: Cada build roda em runner limpo

### 12.5. Observabilidade

- ✅ **Detailed Logs**: Logs detalhados de cada step
- ✅ **Test Reports**: Relatórios de testes em formato TRX
- ✅ **Metadata Labels**: Labels OCI para rastreabilidade
- ✅ **Versioning**: Tags com branch e SHA
- ✅ **Duration Tracking**: Tempo de cada step registrado

### 12.6. Manutenibilidade

- ✅ **Infrastructure as Code**: Pipeline definido em YAML versionado
- ✅ **Documentation**: Documentação inline com comentários
- ✅ **Modular Steps**: Steps independentes e reutilizáveis
- ✅ **Conventional Commits**: Commits seguem padrão
- ✅ **Semantic Versioning**: Pronto para adotar SemVer

---

## 13. Requisitos Técnicos

### 13.1. Pré-Requisitos

| **Componente** | **Requisito** | **Onde Configurar** |
|----------------|---------------|---------------------|
| **Repositório GitHub** | Acesso admin | github.com/alexcar/CadCliX |
| **Conta Docker Hub** | Conta free ou paga | hub.docker.com |
| **Access Token** | Permissões read/write | Docker Hub → Security |
| **GitHub Secrets** | DOCKERHUB_USERNAME, DOCKERHUB_TOKEN | Repo → Settings → Secrets |
| **Branches** | main e develop existentes | Criadas no Git |

### 13.2. Permissões Necessárias

**GitHub**:
- ✅ Admin no repositório (para configurar secrets)
- ✅ Write access (para push)

**Docker Hub**:
- ✅ Criar repositório público (alexcar/cadclix)
- ✅ Gerar access token com read/write

### 13.3. Limites e Quotas

| **Serviço** | **Limite Free Tier** | **CadCliX Uso** | **Status** |
|-------------|---------------------|-----------------|------------|
| **GitHub Actions** | 2.000 min/mês | ~3 min/build × 30 builds = 90 min | ✅ OK |
| **Docker Hub Pulls** | 200 / 6h (autenticado) | ~5 pulls/build | ✅ OK |
| **Docker Hub Pushes** | Ilimitado (free) | ~1 push/build | ✅ OK |
| **Docker Hub Storage** | Ilimitado (free, público) | ~220 MB/tag | ✅ OK |

### 13.4. Compatibilidade

| **Tecnologia** | **Versão Mínima** | **Versão Atual** | **Compatível** |
|----------------|-------------------|------------------|----------------|
| **.NET** | 10.0.0 | 10.0.5 | ✅ Sim |
| **Docker** | 20.10 | Latest | ✅ Sim |
| **GitHub Actions** | N/A | Latest | ✅ Sim |
| **Ubuntu Runner** | 20.04 | 22.04 | ✅ Sim |

---

## 14. Referências e Recursos

### 14.1. Documentação Oficial

| **Recurso** | **URL** |
|-------------|---------|
| **GitHub Actions** | https://docs.github.com/en/actions |
| **Docker Docs** | https://docs.docker.com/ |
| **Docker Hub** | https://docs.docker.com/docker-hub/ |
| **.NET CLI** | https://docs.microsoft.com/en-us/dotnet/core/tools/ |
| **ASP.NET Core** | https://docs.microsoft.com/en-us/aspnet/core/ |
| **xUnit** | https://xunit.net/ |

### 14.2. GitHub Actions Marketplace

| **Action** | **URL** |
|------------|---------|
| **checkout** | https://github.com/actions/checkout |
| **setup-dotnet** | https://github.com/actions/setup-dotnet |
| **setup-buildx-action** | https://github.com/docker/setup-buildx-action |
| **login-action** | https://github.com/docker/login-action |
| **metadata-action** | https://github.com/docker/metadata-action |
| **build-push-action** | https://github.com/docker/build-push-action |
| **test-reporter** | https://github.com/dorny/test-reporter |

### 14.3. Imagens Docker Base

| **Imagem** | **URL** |
|------------|---------|
| **.NET SDK 10** | https://hub.docker.com/_/microsoft-dotnet-sdk |
| **ASP.NET Runtime 10** | https://hub.docker.com/_/microsoft-dotnet-aspnet |

### 14.4. Ferramentas Auxiliares

| **Ferramenta** | **Finalidade** | **URL** |
|----------------|----------------|---------|
| **act** | Testar workflows localmente | https://github.com/nektos/act |
| **Docker Desktop** | Ambiente Docker local | https://www.docker.com/products/docker-desktop |
| **Visual Studio Code** | Editor com extensões GitHub Actions | https://code.visualstudio.com/ |
| **Hadolint** | Linter para Dockerfiles | https://github.com/hadolint/hadolint |

### 14.5. Artigos e Guias

- **Best Practices for GitHub Actions**: https://docs.github.com/en/actions/security-guides/security-hardening-for-github-actions
- **Docker Multi-Stage Builds**: https://docs.docker.com/build/building/multi-stage/
- **.NET Docker Best Practices**: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/docker-application-development-process/docker-app-development-workflow

---

## Conclusão

Este pipeline CI/CD implementa um fluxo completo de **Integração e Entrega Contínua**, garantindo:

- ✅ **Automação Total**: Do commit ao deploy sem intervenção manual
- ✅ **Qualidade Assegurada**: 52 testes validam cada build
- ✅ **Rastreabilidade**: Tags e labels permitem auditoria completa
- ✅ **Eficiência**: Cache e otimizações reduzem tempo em 72%
- ✅ **Segurança**: Secrets protegidos, imagens mínimas, permissões restritas

**Tempo Total de Execução**: ~2m 43s (com cache)

**Frequência**: Automaticamente a cada merge para `main`

**Resultado**: Imagem Docker pronta para produção em `alexcar/cadclix:latest`

---

## Glossário

| **Termo** | **Definição** |
|-----------|---------------|
| **CI/CD** | Continuous Integration / Continuous Deployment (Integração Contínua / Entrega Contínua) |
| **Runner** | Máquina virtual que executa o workflow do GitHub Actions |
| **Workflow** | Processo automatizado definido em arquivo YAML |
| **Job** | Conjunto de steps executados em um runner |
| **Step** | Tarefa individual dentro de um job |
| **Action** | Componente reutilizável para executar uma tarefa |
| **Secret** | Valor criptografado armazenado no GitHub |
| **Cache** | Armazenamento temporário para acelerar builds |
| **Layer** | Camada imutável em uma imagem Docker |
| **Multi-Stage Build** | Dockerfile com múltiplos estágios (build, publish, runtime) |
| **Registry** | Repositório de imagens Docker (ex: Docker Hub) |
| **Tag** | Etiqueta versionada de uma imagem Docker |
| **Digest** | Hash SHA256 imutável de uma imagem |
| **Manifest** | Metadados de uma imagem Docker |
| **BuildKit** | Backend moderno de build do Docker |
| **Buildx** | Plugin do Docker CLI para builds avançados |

---

**Documento Gerado**: Janeiro 2025  
**Versão**: 1.0  
**Autor**: Equipe CadCliX  
**Projeto**: https://github.com/alexcar/CadCliX
