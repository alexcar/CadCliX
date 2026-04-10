# CI/CD Pipeline - Configuração

## 📋 Visão Geral

Este projeto utiliza **GitHub Actions** para automatizar o pipeline CI/CD. O pipeline é acionado automaticamente quando um Pull Request da branch `develop` é merged na branch `main`.

## 🔄 Fluxo do Pipeline

```
PR Merge (develop → main) → Build → Testes Unitários → Docker Build → Docker Push
```

### Etapas do Pipeline

1. **Checkout do Código**
   - Obtém o código-fonte do repositório

2. **Setup do .NET 10**
   - Configura o ambiente .NET 10

3. **Restore de Dependências**
   - Restaura todos os pacotes NuGet necessários

4. **Build**
   - Compila o projeto em modo Release
   - Valida que não há erros de compilação

5. **Execução dos Testes Unitários**
   - Executa todos os 52 testes unitários
   - Gera relatório de resultados (TRX)
   - Pipeline falha se algum teste não passar

6. **Build da Imagem Docker**
   - Cria imagem Docker multi-stage otimizada
   - Usa cache para builds mais rápidos

7. **Push para Docker Hub**
   - Envia a imagem para `alexcar/cadclix`
   - Tags criadas: `latest`, `main-<sha>`, `main`

## 🔐 Configuração de Secrets

Para que o pipeline funcione corretamente, você precisa configurar os seguintes **secrets** no GitHub:

### 1. DOCKERHUB_USERNAME
- **Valor**: `alexcar`
- **Descrição**: Nome de usuário do Docker Hub

### 2. DOCKERHUB_TOKEN
- **Valor**: Token de acesso do Docker Hub
- **Descrição**: Token de autenticação (não use a senha)

### Como Criar o Docker Hub Token

1. Acesse: https://hub.docker.com/settings/security
2. Clique em **"New Access Token"**
3. Nome sugerido: `github-actions-cadclix`
4. Permissões: **Read, Write, Delete**
5. Copie o token gerado (será exibido apenas uma vez)

### Como Configurar os Secrets no GitHub

1. Acesse seu repositório: https://github.com/alexcar/CadCliX
2. Vá em **Settings** → **Secrets and variables** → **Actions**
3. Clique em **"New repository secret"**
4. Adicione cada secret:
   - Nome: `DOCKERHUB_USERNAME` | Valor: `alexcar`
   - Nome: `DOCKERHUB_TOKEN` | Valor: `<seu-token-copiado>`

## 📦 Estrutura de Tags Docker

As imagens Docker são versionadas automaticamente:

- `alexcar/cadclix:latest` - Última versão da branch main
- `alexcar/cadclix:main` - Última versão da branch main
- `alexcar/cadclix:main-<commit-sha>` - Versão específica por commit

## 🚀 Como Usar

### Workflow Normal de Desenvolvimento

1. **Desenvolva na branch `develop`**
   ```bash
   git checkout develop
   git add .
   git commit -m "feat: nova funcionalidade"
   git push origin develop
   ```

2. **Crie um Pull Request**
   - No GitHub, crie PR de `develop` para `main`
   - Aguarde revisão de código

3. **Merge do Pull Request**
   - Ao fazer merge, o pipeline é **automaticamente acionado**
   - Acompanhe a execução em: **Actions** tab do GitHub

4. **Pipeline Executado**
   - ✅ Build realizado
   - ✅ Testes executados (52 tests)
   - ✅ Imagem Docker criada
   - ✅ Imagem enviada para Docker Hub

### Executar Container Localmente

Após o push, você pode executar a imagem:

```bash
# Pull da imagem
docker pull alexcar/cadclix:latest

# Executar container
docker run -d -p 8080:8080 --name cadclix alexcar/cadclix:latest

# Acessar a aplicação
# Swagger UI: http://localhost:8080
```

### Executar Build Docker Localmente

Para testar o Dockerfile antes do commit:

```bash
# Build local
docker build -t cadclix-local .

# Executar localmente
docker run -d -p 8080:8080 --name cadclix-test cadclix-local

# Verificar logs
docker logs cadclix-test

# Parar e remover
docker stop cadclix-test && docker rm cadclix-test
```

## 🐛 Troubleshooting

### Pipeline Falha no Login Docker

**Erro**: `Error: Cannot perform an interactive login from a non TTY device`

**Solução**: Verifique se os secrets `DOCKERHUB_USERNAME` e `DOCKERHUB_TOKEN` estão configurados corretamente.

### Testes Falhando no Pipeline

**Erro**: `Test Run Failed`

**Solução**: 
1. Execute os testes localmente: `dotnet test`
2. Corrija os testes que falharam
3. Commit e push novamente

### Imagem não Aparece no Docker Hub

**Possíveis causas**:
1. Secrets não configurados
2. Pipeline não completou com sucesso
3. Nome do repositório incorreto

**Verificação**:
- Acesse: https://hub.docker.com/r/alexcar/cadclix
- Verifique os logs do workflow na aba Actions

## 📊 Monitoramento

### Verificar Status do Pipeline

1. Acesse: https://github.com/alexcar/CadCliX/actions
2. Veja todos os workflows executados
3. Clique em um workflow para ver detalhes
4. Cada step mostra logs detalhados

### Verificar Imagens no Docker Hub

1. Acesse: https://hub.docker.com/r/alexcar/cadclix
2. Veja todas as tags disponíveis
3. Verifique data do último push

## 🔒 Segurança

### Boas Práticas

✅ **Use Access Tokens, não senhas**
- Tokens podem ser revogados individualmente
- Tokens têm escopo limitado

✅ **Não commite secrets no código**
- Secrets devem estar apenas no GitHub Settings
- Nunca inclua tokens em arquivos de configuração

✅ **Revise permissões**
- Token precisa apenas de Read/Write
- Não dê permissões administrativas

## 📝 Arquivos do Pipeline

```
.
├── .github/
│   └── workflows/
│       └── ci-cd.yml           # Definição do pipeline
├── Dockerfile                   # Multi-stage build .NET 10
├── .dockerignore               # Arquivos ignorados no build
└── CI-CD-SETUP.md              # Esta documentação
```

## 🎯 Próximos Passos

Após configurar os secrets:

1. ✅ Faça um commit de teste na branch `develop`
2. ✅ Crie um PR para `main`
3. ✅ Faça o merge do PR
4. ✅ Acompanhe a execução na aba **Actions**
5. ✅ Verifique a imagem no Docker Hub
6. ✅ Teste o container localmente

## 📚 Recursos Adicionais

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Hub Documentation](https://docs.docker.com/docker-hub/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)

---

**Desenvolvido para**: CadCliX - Sistema de Importação de Dados Contábeis
**Repositório**: https://github.com/alexcar/CadCliX
**Docker Hub**: https://hub.docker.com/r/alexcar/cadclix
