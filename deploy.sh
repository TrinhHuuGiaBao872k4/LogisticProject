#!/bin/bash

set -e # Dừng script nếu có lỗi

echo "📥 Pulling latest code from GitHub..."
if ! git pull; then
  echo "❌ Failed to pull code from GitHub"
  exit 1
fi

echo "📝 Loading environment variables..."
[ -f .env ] && export $(cat .env | xargs)

echo "🛑 Stopping and removing old containers..."
docker-compose down || true

echo "🔧 Building Docker images..."
if ! docker-compose build; then
  echo "❌ Docker build failed"
  exit 1
fi

echo "🚀 Starting up new containers..."
if ! docker-compose up -d; then
  echo "❌ Failed to start containers"
  exit 1
fi

echo "🌐 Opening necessary ports..."
# Các cổng cho ứng dụng
sudo ufw allow 5000/tcp  # Blazor UI (80->5000)
sudo ufw allow 8787/tcp  # Logistic API (82->8787)
sudo ufw allow 8877/tcp  # Payment API (81->8877)

# Các cổng cho hệ thống
sudo ufw allow 2181/tcp  # Zookeeper
sudo ufw allow 9092/tcp  # Kafka (nếu có)
sudo ufw allow 1433/tcp  # Azure SQL Edge

# Cổng cho GatewayService (nếu sử dụng)
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

sudo ufw reload

echo "🩺 Checking services health..."
sleep 10 # Đợi các service khởi động
unhealthy=$(docker ps --filter "health=unhealthy" --format "{{.Names}}")
if [ -n "$unhealthy" ]; then
  echo "⚠️ Unhealthy containers: $unhealthy"
  exit 1
fi

echo "✅ Deployment completed successfully!"
echo "🔗 Blazor UI:        http://$(curl -s ifconfig.me):5000"
echo "🔗 Logistic API:     http://$(curl -s ifconfig.me):8787"
echo "🔗 Payment API:      http://$(curl -s ifconfig.me):8877"
echo "ℹ️  SQL Server:       $(curl -s ifconfig.me):1433"