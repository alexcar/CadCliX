# Script simples para executar os testes

Write-Host "🧪 Executando Testes..." -ForegroundColor Cyan
dotnet test CadCliX.Tests/CadCliX.Tests.csproj --logger "console;verbosity=normal"

Write-Host ""
Write-Host "✅ Concluído!" -ForegroundColor Green
