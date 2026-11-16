#!/usr/bin/env bash
# Compila a API, sobe numa porta de teste e exercita todos os endpoints.
# Uso (na raiz do repositório):  bash scripts/verificar-api.sh
# O resultado também é gravado em scripts/verificar-api.log
cd "$(dirname "$0")/.." || exit 1
LOG="scripts/verificar-api.log"
exec > >(tee "$LOG") 2>&1

PORTA=5099
BASE="http://localhost:$PORTA"
DB_TMP="$(mktemp -d)/teste.db"
FALHAS=0

ok()   { echo "  OK   $1"; }
falha(){ echo "  FALHA $1"; FALHAS=$((FALHAS+1)); }
checa(){ # descrição, esperado, obtido
  if [ "$2" = "$3" ]; then ok "$1"; else falha "$1 (esperado $2, obtido $3)"; fi
}

echo "== dotnet --version: $(dotnet --version)"
echo "== Build"
dotnet build Imobiliaria.sln -nologo -v q 2>&1 | tail -40
[ "${PIPESTATUS[0]}" -eq 0 ] || { echo "BUILD FALHOU"; exit 1; }

echo "== Subindo API em $BASE (banco temporário)"
ConnectionStrings__DefaultConnection="Data Source=$DB_TMP" \
ASPNETCORE_ENVIRONMENT=Development \
dotnet run --project API --no-build --urls "$BASE" > /tmp/imobiliaria-api.log 2>&1 &
PID=$!
trap 'kill $PID 2>/dev/null' EXIT
for i in $(seq 1 40); do
  curl -s -o /dev/null "$BASE/Imovel/ListarImoveis" && break
  sleep 1
done

echo "== Imóveis"
N=$(curl -s "$BASE/Imovel/ListarImoveis" | python3 -c 'import sys,json;print(len(json.load(sys.stdin)))')
checa "seed carregou 30 imóveis" 30 "$N"
N=$(curl -s "$BASE/Imovel/ListarImoveisPorTipoServico?tipoServico=Comprar" | python3 -c 'import sys,json;print(len(json.load(sys.stdin))>0)')
checa "lista por tipo de serviço" True "$N"
ID=$(curl -s -X POST "$BASE/Imovel/AdicionarImovel" -H 'Content-Type: application/json' \
  -d '{"tipo":"Casa","area":100,"preco":500000,"anos":2,"tipoServico":"Comprar","titulo":"Teste","quartos":3}' \
  | python3 -c 'import sys,json;print(json.load(sys.stdin)["id"])')
[ -n "$ID" ] && ok "adicionar imóvel ($ID)" || falha "adicionar imóvel"
Q=$(curl -s "$BASE/Imovel/ListarImovelPorId?id=$ID" | python3 -c 'import sys,json;print(json.load(sys.stdin)["quartos"])')
checa "campo quartos persistido" 3 "$Q"
printf '\x89PNG\r\n\x1a\n' > /tmp/teste.png
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT -F "imagem=@/tmp/teste.png" "$BASE/Imovel/UploadImage?imovelId=$ID")
checa "upload de imagem do imóvel" 200 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X DELETE "$BASE/Imovel/RemoverImovel?id=$ID")
checa "remover imóvel" 204 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' "$BASE/Imovel/ListarImovelPorId?id=$ID")
checa "imóvel removido não existe mais" 404 "$C"
P=$(curl -s -X POST "$BASE/Imovel/PrecificarImovel" -H 'Content-Type: application/json' \
  -d '{"vendaOuAluguel":"venda","novo":true,"tipoImovel":"apartamento","quartos":2,"banheiros":1,"vagas":1,"areaM2":60,"areasComuns":[]}' \
  | python3 -c 'import sys,json;print(json.load(sys.stdin)["preco"]>0)')
checa "precificador retorna valor > 0" True "$P"

K=$(curl -s "$BASE/Imovel/ListarImoveis" | python3 -c 'import sys,json;d=json.load(sys.stdin)[0];print("urlsImagens" in d and "tipoServico" in d)')
checa "contrato JSON do imóvel (urlsImagens)" True "$K"

echo "== Usuários"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Usuario/LogarUsuario" -H 'Content-Type: application/json' -d '{"username":"Admin","password":"Admin"}')
checa "login do admin" 200 "$C"
FOTO=$(curl -s -X POST "$BASE/Usuario/LogarUsuario" -H 'Content-Type: application/json' -d '{"username":"Admin","password":"Admin"}' | python3 -c 'import sys,json;d=json.load(sys.stdin);print("fotoDePerfilURL" in d and d["tipo"]=="Administrador")')
checa "contrato JSON do usuário (fotoDePerfilURL, tipo)" True "$FOTO"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Usuario/LogarUsuario" -H 'Content-Type: application/json' -d '{"username":"Admin","password":"errada"}')
checa "login com senha errada é negado" 401 "$C"
R=$(curl -s -X POST "$BASE/Usuario/AdicionarUsuario" -H 'Content-Type: application/json' \
  -d '{"nome":"Teste","idade":30,"genero":"Outro","cep":"30000-000","rua":"Rua A","numero":10,"bairro":"Centro","cidade":"BH","unidadeFederativa":"MG","identificacao":"123","username":"teste","password":"senha123"}')
UID_=$(echo "$R" | python3 -c 'import sys,json;print(json.load(sys.stdin)["id"])')
[ -n "$UID_" ] && ok "cadastro de usuário ($UID_)" || falha "cadastro de usuário"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Usuario/AdicionarUsuario" -H 'Content-Type: application/json' -d '{"username":"teste","password":"x"}')
checa "username duplicado é recusado" 409 "$C"
SEMSENHA=$(curl -s -X POST "$BASE/Usuario/LogarUsuario" -H 'Content-Type: application/json' -d '{"username":"teste","password":"senha123"}' | python3 -c 'import sys,json;d=json.load(sys.stdin);print("password" not in {k.lower() for k in d})')
checa "login não devolve senha" True "$SEMSENHA"
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT "$BASE/Usuario/EditarUsername" -H 'Content-Type: application/json' -d "{\"id\":$UID_,\"valor\":\"teste2\"}")
checa "trocar username" 204 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT "$BASE/Usuario/EditarSenha" -H 'Content-Type: application/json' -d "{\"id\":$UID_,\"valor\":\"novasenha\"}")
checa "trocar senha" 204 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Usuario/LogarUsuario" -H 'Content-Type: application/json' -d '{"username":"teste2","password":"novasenha"}')
checa "login com novo username/senha" 200 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X PUT -F "imagem=@/tmp/teste.png" "$BASE/Usuario/UploadImage?usuarioId=$UID_")
checa "upload de foto de perfil" 200 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X DELETE "$BASE/Usuario/RemoverUsuario?id=$UID_")
checa "remover usuário" 204 "$C"

echo "== Mensagens"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Mensagem/EnviarMensagem" -H 'Content-Type: application/json' -d '{"nome":"A","email":"a@b.com","texto":"oi","origem":"contato"}')
checa "enviar mensagem de contato" 200 "$C"
C=$(curl -s -o /dev/null -w '%{http_code}' -X POST "$BASE/Mensagem/EnviarMensagem" -H 'Content-Type: application/json' -d '{"nome":"A","email":"invalido"}')
checa "e-mail inválido é recusado" 400 "$C"
N=$(curl -s "$BASE/Mensagem/ListarMensagens" | python3 -c 'import sys,json;print(len(json.load(sys.stdin)))')
checa "mensagem listada" 1 "$N"

echo "== Front-end (build de produção)"
( cd FrontEnd/imobiliaria && npm install --no-audit --no-fund >/dev/null 2>&1 && npm run build 2>&1 | tail -4 ) ; [ "${PIPESTATUS[0]}" -eq 0 ] && ok "build do front" || falha "build do front"

echo
if [ "$FALHAS" -eq 0 ]; then echo "TUDO OK"; else echo "$FALHAS verificação(ões) falharam. Log da API:"; tail -30 /tmp/imobiliaria-api.log; fi
