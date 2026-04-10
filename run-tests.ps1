# Script para executar testes com cobertura de código

Write-Host "🧪 Executando Testes Unitários CadCliX..." -ForegroundColor Cyan
Write-Host ""

# Executar testes com cobertura
Write-Host "📊 Coletando dados de cobertura..." -ForegroundColor Yellow
dotnet test CadCliX.Tests/CadCliX.Tests.csproj `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=./coverage/ `
    /p:Exclude="[CadCliX.Tests]*" `
    --logger "console;verbosity=normal"

Write-Host ""
Write-Host "✅ Testes concluídos!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Arquivo de cobertura gerado em: CadCliX.Tests\coverage\coverage.cobertura.xml" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Para visualizar o relatório HTML:" -ForegroundColor Yellow
Write-Host "   1. Instale o ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor White
Write-Host "   2. Execute: reportgenerator -reports:`"CadCliX.Tests\coverage\coverage.cobertura.xml`" -targetdir:`"CadCliX.Tests\coverage\html`" -reporttypes:Html" -ForegroundColor White
Write-Host "   3. Abra: CadCliX.Tests\coverage\html\index.html" -ForegroundColor White
Write-Host ""
