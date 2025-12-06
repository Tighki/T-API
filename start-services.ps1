Write-Host "Starting T-API Microservices..." -ForegroundColor Green

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\CoolingService'; Write-Host 'CoolingService on http://localhost:5001' -ForegroundColor Cyan; dotnet run --urls 'http://localhost:5001'"
Start-Sleep -Seconds 3

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\UserService'; Write-Host 'UserService on http://localhost:5002' -ForegroundColor Yellow; dotnet run --urls 'http://localhost:5002'"
Start-Sleep -Seconds 3

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\PortfolioService'; Write-Host 'PortfolioService on http://localhost:5003' -ForegroundColor Magenta; dotnet run --urls 'http://localhost:5003'"

Write-Host "`nSwagger UIs:" -ForegroundColor White
Write-Host "  CoolingService:   http://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "  UserService:      http://localhost:5002/swagger" -ForegroundColor Yellow
Write-Host "  PortfolioService: http://localhost:5003/swagger" -ForegroundColor Magenta

