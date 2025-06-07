# Giai đoạn build: dùng SDK đầy đủ
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc
WORKDIR /src

# Copy file .csproj và restore packages
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code
COPY . .

# Build và publish ứng dụng sang thư mục /app
RUN dotnet publish -c Release -o /app 

# Giai đoạn runtime: dùng runtime nhẹ (không có SDK)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Tạo thư mục làm việc cho app
WORKDIR /app

# Copy file publish từ build stage
COPY --from=build /app .

# EXPOSE cổng để container lắng nghe
# ENV ASPNETCORE_URLS=http://+:80
# EXPOSE 80
# ENV ASPNETCORE_URLS=http://+:82
# EXPOSE 82

# Lệnh chạy app khi container start
ENTRYPOINT ["dotnet", "LogisticService.dll"]
