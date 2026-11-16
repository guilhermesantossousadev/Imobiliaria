@echo off
REM Windows: dois cliques para subir API + front e abrir o site.
cd /d "%~dp0"
if not exist "FrontEnd\imobiliaria\.env" copy "FrontEnd\imobiliaria\.env.example" "FrontEnd\imobiliaria\.env" >nul
echo Instalando dependencias do front...
pushd FrontEnd\imobiliaria
call npm install --no-audit --no-fund
popd
start "Sweet Home - API" cmd /k "set ASPNETCORE_ENVIRONMENT=Development&& dotnet run --project API --urls http://localhost:5070"
start "Sweet Home - Front" cmd /k "cd FrontEnd\imobiliaria && npm run dev"
timeout /t 12 >nul
start http://localhost:5173
