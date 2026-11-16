#!/usr/bin/env bash
# Mac: dê dois cliques neste arquivo para subir API + front e abrir o site.
cd "$(dirname "$0")" || exit 1
( sleep 12; open "http://localhost:5173" ) &
bash scripts/iniciar.sh
