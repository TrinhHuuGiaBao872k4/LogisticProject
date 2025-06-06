#!/bin/bash

set -e

echo "🛠 Initializing Docker Buildx..."
docker buildx install >/dev/null 2>&1 || echo "⚠ Buildx initialization skipped"

echo "📥 Pulling latest code from GitHub..."
git fetch origin
git reset --hard origin/master

echo "📝 Loading environment variables..."
[ -f .env ] && export $(cat .env | xargs)

echo "🛑 Stopping and removing old containers..."
docker-compose down --remove-orphans || true

echo "🔧 Building Docker images..."
docker-compose build --pull --no-cache || {
  echo "❌ Build failed, checking logs..."
  docker-compose logs --tail=50
  exit 1
}

echo "🚀 Starting up new containers..."
docker-compose up -d || {
  echo "❌ Startup failed, checking logs..."
  docker-compose logs --tail=50
  exit 1
}

echo "🔒 Configuring firewall..."
sudo ufw --force enable
sudo ufw allow ssh
sudo ufw allow 5000/tcp  # Blazor UI
sudo ufw allow 8787/tcp  # Logistic API
sudo ufw allow 8877/tcp  # Payment API
sudo ufw allow 2181/tcp  # Zookeeper
sudo ufw allow 9092/tcp  # Kafka
sudo ufw allow 1433/tcp  # SQL Server
sudo ufw reload

echo "🩺 Running comprehensive health checks..."
sleep 15  # Đợi các service khởi động hoàn toàn

healthy=true
declare -A services=(
  ["Blazor UI"]="logistic_blazor_web_app:5000"
  ["Logistic API"]="logistic_service:8787" 
  ["Payment API"]="payment_service:8877"
  ["Kafka"]="kafka:9092"
  ["Zookeeper"]="zoo:2181"
)

for service in "${!services[@]}"; do
  container=$(echo ${services[$service]} | cut -d: -f1)
  port=$(echo ${services[$service]} | cut -d: -f2)
  
  # Kiểm tra container status
  status=$(docker inspect -f '{{.State.Status}}' $container 2>/dev/null || echo "missing")
  
  # Kiểm tra kết nối mạng
  if nc -z localhost $port; then
    network="✅"
  else
    network="❌"
    healthy=false
  fi
  
  echo "Service $service: Status=$status, Network=$network"
  
  if [ "$status" != "running" ]; then
    echo "🔍 Last 10 lines of logs:"
    docker logs $container --tail 10 2>/dev/null || echo "No logs available"
    healthy=false
  fi
done

if $healthy; then
  echo "✅ Deployment completed successfully!"
  echo "🔗 Blazor UI:    http://$(curl -s ifconfig.me):5000"
  echo "🔗 Logistic API: http://$(curl -s ifconfig.me):8787"
  echo "🔗 Payment API:  http://$(curl -s ifconfig.me):8877"
  echo "ℹ️  SQL Server:   $(curl -s ifconfig.me):1433"
else
  echo "❌ Deployment completed with errors!"
  echo "⚠️ Some services may not be functioning properly"
  exit 1
fi