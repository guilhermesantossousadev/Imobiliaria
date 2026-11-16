#!/usr/bin/env bash
# Sobe a API (http://localhost:5070) e o front (http://localhost:5173) juntos.
# Uso (na raiz do repositório):  bash scripts/iniciar.sh
cd "$(dirname "$0")/.." || exit 1

trap 'kill 0' EXIT

echo "Instalando dependências do front (se necessário)..."
( cd FrontEnd/imobiliaria && [ -f .env ] || cp .env.example .env; npm install --no-audit --no-fund )

echo "Iniciando API em http://localhost:5070 (Swagger em /swagger)"
( ASPNETCORE_ENVIRONMENT=Development dotnet run --project API --urls http://localhost:5070 ) &

echo "Iniciando front em http://localhost:5173"
( cd FrontEnd/imobiliaria && npm run dev ) &

wait
