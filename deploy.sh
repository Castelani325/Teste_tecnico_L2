#!/bin/bash
set -e

echo "Atualizando codigo..."
git pull

echo "Subindo containers (rebuild se necessario)..."
docker compose up -d --build

echo "Deploy concluido. Containers ativos:"
docker compose ps
