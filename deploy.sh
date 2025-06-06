# 1. Pull code mới nhất từ git
echo "🔄 Pulling latest code from Git..."
git pull origin master  # Thay 'main' bằng branch bạn muốn deploy (master/main/develop...)

# 2. Dừng và xóa container cũ
echo "🛑 Stopping and removing old containers..."
docker-compose down

# 3. Build lại và chạy container mới
echo "🔨 Rebuilding and starting new containers..."
docker-compose up -d --build

# 4. Kiểm tra kết quả
echo "✅ Deployment completed!"
docker ps  # Hiển thị các container đang chạy