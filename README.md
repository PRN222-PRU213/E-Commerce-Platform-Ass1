# 🛒 E-Commerce Platform – Assignment 1

## 📌 Giới thiệu
**E-Commerce Platform – Assignment 1** là một dự án web thương mại điện tử được xây dựng bằng **ASP.NET Core MVC**, áp dụng mô hình **3-layer Architecture** nhằm tách biệt rõ ràng giữa giao diện, xử lý nghiệp vụ và truy cập dữ liệu.

Dự án phục vụ mục đích học tập, rèn luyện kỹ năng thiết kế kiến trúc, làm việc với Entity Framework Core và SQL Server.

---

## 🧱 Kiến trúc tổng thể

```
E-Commerce-Platform-Ass1
│
├── E-Commerce-Platform-Ass1.Data      // Data Access Layer
├── E-Commerce-Platform-Ass1.Service   // Business Logic Layer
├── E-Commerce-Platform-Ass1.Web       // Presentation Layer (MVC)
│
├── E-Commerce-Platform-Ass1.sln
├── README.md
└── .gitignore
```

### 🔹 Các Layer
- **Data**  
  - Chứa DbContext, Entity, Migration  
  - Làm việc trực tiếp với SQL Server thông qua Entity Framework Core

- **Service**  
  - Xử lý nghiệp vụ  
  - Giao tiếp với Data layer  
  - Được inject vào Web thông qua Dependency Injection

- **Web**  
  - ASP.NET Core MVC  
  - Bao gồm Controller, View, ViewModel  
  - Là nơi người dùng tương tác với hệ thống

---

## ⚙️ Công nghệ sử dụng
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor View
- Dependency Injection
- Git & GitHub

---

## 🚀 Hướng dẫn chạy project trên máy local

### 1️⃣ Yêu cầu môi trường
- .NET SDK 6.0 hoặc 7.0
- Visual Studio 2022
- SQL Server (LocalDB hoặc SQL Server Express)
- Git

---

### 2️⃣ Clone project
```bash
git clone git@github.com:PRN222-Group4/E-Commerce-Platform-Ass1.git
cd E-Commerce-Platform-Ass1
```

---

### 3️⃣ Mở project
- Mở file `E-Commerce-Platform-Ass1.sln` bằng Visual Studio
- Set `E-Commerce-Platform-Ass1.Web` làm Startup Project

---

### 4️⃣ Cấu hình Database
Mở file `appsettings.json` trong project Web:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=YourDatabase;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

### 5️⃣ Migration & Update Database
Mở Package Manager Console và chạy:

```powershell
Update-Database
```

(Đảm bảo project Data được chọn làm Default Project)

---

### 6️⃣ Chạy ứng dụng
- Nhấn F5 hoặc Ctrl + F5
- Truy cập: https://localhost:xxxx

---

## 📂 Quy tắc tham chiếu project
- Web → Service
- Service → Data
- Data ❌ không tham chiếu ngược lên layer trên

---

## 👨‍💻 Tác giả
- Sinh viên: **
- Assignment 1 – ASP.NET Core MVC

---

## 📄 License
Project được sử dụng cho mục đích học tập.
