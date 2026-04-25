#!/usr/bin/env bash
set -e

echo "==> wholesale deploy başlıyor..."

# .env yoksa oluştur
if [ ! -f .env ]; then
  echo "UYARI: .env dosyası bulunamadı, .env.example'dan kopyalanıyor..."
  cp .env.example .env
  echo "!!! .env içindeki JWT_SECRET'ı değiştirmeyi unutma!"
fi

# En son kodu çek
git pull

# Build + restart (sadece değişen container'ı rebuild eder)
docker compose build api
docker compose up -d

echo "==> Deploy tamamlandı."
echo "    API  : http://localhost:5100/health"
echo "    Seq  : http://localhost:5341"
