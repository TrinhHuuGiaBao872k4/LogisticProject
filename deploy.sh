#!/bin/bash

echo "📥 Pulling latest code from GitHub..."
git pull

echo "🛑 Stopping and removing old containers..."
docker-compose down || true

echo "🔧 Building Docker images..."
docker-compose build

echo "🚀 Starting up new containers..."
docker-compose up -d

echo "🌐 Opening necessary ports (if needed)..."
sudo ufw allow 5000/tcp
sudo ufw allow 8787/tcp
sudo ufw allow 8877/tcp
sudo ufw allow 2181/tcp
sudo ufw allow 9092/tcp
sudo ufw reload

echo "✅ Deployment completed successfully!"
echo "🔗 Logistic Blazor UI: http://$(curl -s ifconfig.me):5000"
echo "🔗 Logistic API:        http://$(curl -s ifconfig.me):8787"
echo "🔗 Payment API:         http://$(curl -s ifconfig.me):8877"
