# --- Giai đoạn 1: Build dự án ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy file dự án (.csproj) vào để restore thư viện trước cho nhanh
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code còn lại và xuất bản (publish) ra thư mục 'out'
COPY . ./
RUN dotnet publish -c Release -o out

# --- Giai đoạn 2: Chạy dự án ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
# Lấy thành phẩm từ giai đoạn build sang
COPY --from=build /app/out .

# Mở cổng 80 cho web
EXPOSE 80
# QUAN TRỌNG: Thay 'api.dll' bằng đúng tên dự án của Thành (ví dụ: SmartGym.dll)
ENTRYPOINT ["dotnet", "api.dll"]