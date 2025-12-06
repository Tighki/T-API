Write-Host "Starting T-API Microservices..." -ForegroundColor Green

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\CoolingService'; Write-Host 'CoolingService starting on https://localhost:7001' -ForegroundColor Cyan; dotnet run"
Start-Sleep -Seconds 3

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\UserService'; Write-Host 'UserService starting on https://localhost:7002' -ForegroundColor Yellow; dotnet run"
Start-Sleep -Seconds 3

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\PortfolioService'; Write-Host 'PortfolioService starting on https://localhost:7003' -ForegroundColor Magenta; dotnet run"

Write-Host "`nSwagger UIs:" -ForegroundColor White
Write-Host "  CoolingService:   https://localhost:7001/swagger" -ForegroundColor Cyan
Write-Host "  UserService:      https://localhost:7002/swagger" -ForegroundColor Yellow
Write-Host "  PortfolioService: https://localhost:7003/swagger" -ForegroundColor Magenta

