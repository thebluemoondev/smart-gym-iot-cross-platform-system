# 🏋️‍♂️ Smart Gym Management System 

![GitHub top language](https://img.shields.io/github/languages/top/thebluemoondev/SmartGym-Management?style=for-the-badge)
![Docker](https://img.shields.io/badge/docker-%230db7ed.svg?style=for-the-badge&logo=docker&logoColor=white)
![.Net](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=.net&logoColor=white)
![Raspberry Pi](https://img.shields.io/badge/-Raspberry%20Pi-C51A4A?style=for-the-badge&logo=Raspberry-Pi)

Hệ thống quản lý phòng Gym thông minh tích hợp công nghệ **IoT (RFID RC522)**, Backend **.NET 10** và triển khai trên hạ tầng **Raspberry Pi 5** thông qua **Docker**. Dự án giải quyết bài toán kiểm soát ra vào thời gian thực và quản lý chỉ số sức khỏe hội viên.

---

## 🌟 Tính năng chính

### 🔹 Hệ thống Backend & Admin Panel
- **Quản lý hội viên:** Đăng ký, phân quyền (Admin, PT, Member) và quản lý gói tập tập trung.
- **Xác thực Real-time:** API xử lý check-in từ đầu đọc RFID với độ trễ thấp (< 500ms).
- **Cloudflare Tunnel:** Truy cập hệ thống an toàn từ xa mà không cần Port Forwarding.

### 🔹 IoT & Hardware (ESP32 + RC522)
- **Quét thẻ RFID:** Nhận diện hội viên và điều khiển cửa tự động.
- **Tương tác:** Hiển thị thông tin qua màn hình LCD 16x2 và đèn LED thông báo trạng thái gói tập.

---
```text
## 📂 Cấu trúc dự án

./
├── api/ # Mã nguồn ASP.NET Core 10 Web API (Backend)
├── website/ # Dashboard & Register Page (HTML/CSS/JS)
├── smart-gym-iot/ # Mã nguồn ESP32 & cảm biến RFID RC522
├── app/ # Nguyên mẫu ứng dụng di động (Kivy/Python)
└── docker/ # Dockerfile và docker-compose.yml
```

## 🛠️ Công nghệ sử dụng

**Backend:** C# (.NET 10), Entity Framework Core, SQL Server  
**Frontend:** HTML5, CSS3 (Modern Dark Mode), Axios JS  
**IoT:** C++ (Arduino Framework), ESP32 DevKit V1  
**Hạ tầng:** Docker, Raspberry Pi 5, Cloudflare Tunnel

---

## 🚀 Hướng dẫn triển khai nhanh

### 1. Clone mã nguồn
```bash
git clone https://github.com/thebluemoondev/smart-gym-iot-cross-platform-system.git
cd smart-gym-iot-cross-platform-system
```
### 2. Khởi chạy hệ thống với Docker
```bash
docker-compose up -d --build
```
### 3. Cấu hình thiết bị IoT
Mở mã nguồn trong thư mục smart-gym-iot/

Cập nhật thông số WiFi SSID, Password và API_URL (Domain của bạn)

Nạp chương trình vào ESP32 bằng Arduino IDE hoặc VS Code (PlatformIO)

👤 Tác giả
Nguyễn Như Thành (Bluemoon)

GitHub: @thebluemoondev

Website: thanhchinh.io.vn

