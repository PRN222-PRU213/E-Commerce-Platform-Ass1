# 🛒 E-Commerce Platform – Assignment 1

## 📌 Giới thiệu

**E-Commerce Platform – Assignment 1** là một dự án web thương mại điện tử được xây dựng bằng **ASP.NET Core MVC**, áp dụng mô hình **3-layer Architecture** nhằm tách biệt rõ ràng giữa giao diện, xử lý nghiệp vụ và truy cập dữ liệu.

Dự án phục vụ mục đích học tập, rèn luyện kỹ năng thiết kế kiến trúc, làm việc với Entity Framework Core và SQL Server.

---

## 🧱 Kiến trúc tổng thể

```
E-Commerce-Platform-Ass1.sln
│
├── 📁 E-Commerce-Platform-Ass1.Web          ─────────────── [Presentation Layer]
│   ├── Controllers/                          (14 Controllers)
│   │   ├── AdminController.cs
│   │   ├── AuthenticationController.cs
│   │   ├── CartController.cs / CartItemController.cs
│   │   ├── CheckoutController.cs
│   │   ├── HomeController.cs
│   │   ├── OrderController.cs
│   │   ├── PaymentController.cs
│   │   ├── ProductController.cs / ProductsController.cs
│   │   ├── RefundController.cs
│   │   ├── ShopController.cs / ShopOrdersController.cs
│   │   └── WalletController.cs
│   │
│   ├── Views/                                (Razor Views)
│   │   ├── Admin/
│   │   ├── Authentication/
│   │   ├── Cart/ / Checkout/
│   │   ├── Home/ / Order/ / Payment/
│   │   ├── Product/ / Products/
│   │   ├── Shop/ / ShopOrders/
│   │   ├── Wallet/
│   │   └── Shared/ (_Layout, Partials)
│   │
│   ├── Models/                               (ViewModels)
│   │   ├── AdminViewModels.cs
│   │   ├── CartViewModel.cs
│   │   ├── HomeViewModels.cs
│   │   ├── ProductViewModels.cs
│   │   ├── ShopViewModels.cs
│   │   └── ... (Login, Register, Order VMs)
│   │
│   ├── Infrastructure/Extensions/            (DI & Mapping)
│   │   ├── AddDependencyInjection.cs
│   │   ├── AdminMappingExtensions.cs
│   │   └── ShopMappingExtensions.cs
│   │
│   ├── wwwroot/                              (Static Files)
│   │   ├── css/ / js/ / images/
│   │
│   └── Program.cs                            (Entry Point)
│
├── 📁 E-Commerce-Platform-Ass1.Service      ─────────────── [Business Logic Layer]
│   ├── Services/                             (16 Services)
│   │   ├── AdminService.cs
│   │   ├── CartService.cs
│   │   ├── CheckoutService.cs
│   │   ├── CloudinaryService.cs
│   │   ├── EmailService.cs
│   │   ├── MomoService.cs
│   │   ├── OrderService.cs
│   │   ├── ProductService.cs / ProductVariantService.cs
│   │   ├── RefundService.cs
│   │   ├── ShopOrderService.cs / ShopService.cs
│   │   ├── UserService.cs
│   │   └── WalletService.cs
│   │
│   ├── Services/IServices/                   (15 Interfaces)
│   │   ├── IAdminService.cs
│   │   ├── ICartService.cs
│   │   ├── IProductService.cs
│   │   ├── IShopService.cs
│   │   └── ... (Các interface tương ứng)
│   │
│   ├── DTOs/                                 (16 Data Transfer Objects)
│   │   ├── ProductDto.cs / ProductDetailDto.cs
│   │   ├── OrderDtos.cs
│   │   ├── ShopDto.cs / ShopStatisticsDto.cs
│   │   └── ... (Các DTO khác)
│   │
│   └── Common/ / Helper/ / Utils/            (Utilities)
│
├── 📁 E-Commerce-Platform-Ass1.Data         ─────────────── [Data Access Layer]
│   ├── Database/
│   │   ├── ApplicationDbContext.cs           (EF Core DbContext)
│   │   │
│   │   ├── Entities/                         (15 Domain Entities)
│   │   │   ├── User.cs / Role.cs
│   │   │   ├── Shop.cs
│   │   │   ├── Product.cs / ProductVariant.cs
│   │   │   ├── Category.cs
│   │   │   ├── Cart.cs / CartItem.cs
│   │   │   ├── Order.cs / OrderItem.cs
│   │   │   ├── Payment.cs / Refund.cs / Wallet.cs
│   │   │   ├── Review.cs / Shipment.cs
│   │   │
│   │   └── Configurations/                   (15 EF Configurations)
│   │       └── (FluentAPI configurations)
│   │
│   ├── Repositories/                         (15 Repositories)
│   │   ├── UserRepository.cs
│   │   ├── ShopRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── OrderRepository.cs
│   │   ├── CartRepository.cs
│   │   └── ... (Các repository khác)
│   │
│   ├── Repositories/Interfaces/              (15 Interfaces)
│   │   ├── IUserRepository.cs
│   │   ├── IProductRepository.cs
│   │   └── ... (Các interface tương ứng)
│   │
│   ├── Momo/                                 (Momo Payment Integration)
│   │
│   └── Migrations/                           (EF Migrations)
│
└── 📄 Supporting Files
    ├── seed-data.sql
    ├── ORDER_STATUS_FLOW.md
    └── README.md
```

---

## � Mô tả Hoạt động các Tầng

### 1️⃣ **Presentation Layer** (`E-Commerce-Platform-Ass1.Web`)

| Thành phần         | Mô tả                                                                                                               |
| ------------------ | ------------------------------------------------------------------------------------------------------------------- |
| **Controllers**    | Nhận HTTP requests từ người dùng, điều hướng logic, gọi Services và trả về Views. Sử dụng ASP.NET Core MVC pattern. |
| **Views (Razor)**  | Hiển thị giao diện HTML cho người dùng. Sử dụng Razor syntax để render dữ liệu động từ ViewModels.                  |
| **ViewModels**     | Định nghĩa cấu trúc dữ liệu được truyền từ Controller → View. Tách biệt với Domain Entities.                        |
| **Infrastructure** | Chứa DI registration (`AddDependencyInjection.cs`) và mapping extensions để chuyển đổi giữa Entities ↔ ViewModels.  |
| **Program.cs**     | Entry point, cấu hình middleware, DI container, DbContext, Authentication, Session.                                 |

**Luồng xử lý:**

```
HTTP Request → Controller → Gọi Service → Nhận kết quả → Mapping → Return View(ViewModel)
```

---

### 2️⃣ **Business Logic Layer** (`E-Commerce-Platform-Ass1.Service`)

| Thành phần            | Mô tả                                                                                                                           |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| **Services**          | Chứa business logic chính. Ví dụ: `ProductService` xử lý nghiệp vụ CRUD sản phẩm, `CheckoutService` xử lý quy trình thanh toán. |
| **IServices**         | Interface định nghĩa contract cho các Services, hỗ trợ DI và unit testing.                                                      |
| **DTOs**              | Data Transfer Objects - chuyển dữ liệu giữa các tầng, tránh expose trực tiếp Entities.                                          |
| **External Services** | `CloudinaryService` (upload ảnh), `EmailService` (gửi mail), `MomoService` (thanh toán Momo).                                   |

**Các Services chính:**

- `UserService`: Quản lý người dùng, đăng ký, đăng nhập
- `ProductService`: CRUD sản phẩm, tìm kiếm, lọc
- `ShopService`: Quản lý shop của seller
- `OrderService` / `ShopOrderService`: Xử lý đơn hàng
- `CartService`: Quản lý giỏ hàng
- `WalletService` / `RefundService`: Ví và hoàn tiền
- `AdminService`: Chức năng quản trị

---

### 3️⃣ **Data Access Layer** (`E-Commerce-Platform-Ass1.Data`)

| Thành phần               | Mô tả                                                                               |
| ------------------------ | ----------------------------------------------------------------------------------- |
| **Entities**             | Domain models ánh xạ trực tiếp với bảng trong database (User, Product, Order, ...). |
| **ApplicationDbContext** | EF Core DbContext, quản lý kết nối database và DbSets.                              |
| **Configurations**       | Fluent API configurations cho các entities (quan hệ, constraints, indexes).         |
| **Repositories**         | Triển khai Repository Pattern, đóng gói các truy vấn database.                      |
| **IRepositories**        | Interface cho repositories, hỗ trợ abstraction và unit testing.                     |

**Các Entities chính:**

- `User`, `Role` - Quản lý người dùng
- `Shop` - Thông tin cửa hàng
- `Product`, `ProductVariant`, `Category` - Sản phẩm
- `Cart`, `CartItem` - Giỏ hàng
- `Order`, `OrderItem`, `Shipment` - Đơn hàng
- `Payment`, `Wallet`, `Refund` - Thanh toán
- `Review` - Đánh giá

---

## 🔁 Luồng Dữ liệu Tổng quan

```
┌─────────────────────────────────────────────────────────────────────┐
│                         USER (Browser)                              │
└─────────────────────────────────────────────────────────────────────┘
                                  │
                                  ▼ HTTP Request
┌─────────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER (Web)                         │
│  ┌────────────┐    ┌────────────┐    ┌────────────┐                │
│  │ Controller │ ◄──│  Mapping   │───►│   Views    │                │
│  │            │    │ Extensions │    │  (Razor)   │                │
│  └─────┬──────┘    └────────────┘    └────────────┘                │
│        │ Inject IServices                                           │
└────────┼────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────┐
│                   BUSINESS LOGIC LAYER (Service)                    │
│  ┌────────────┐    ┌────────────┐    ┌────────────┐                │
│  │  Services  │───►│    DTOs    │    │  External  │                │
│  │            │    │            │    │  Services  │                │
│  └─────┬──────┘    └────────────┘    │(Email,Momo)│                │
│        │ Inject IRepositories        └────────────┘                │
└────────┼────────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    DATA ACCESS LAYER (Data)                         │
│  ┌────────────┐    ┌────────────┐    ┌────────────┐                │
│  │Repositories│───►│ DbContext  │───►│  Entities  │                │
│  │            │    │            │    │            │                │
│  └────────────┘    └─────┬──────┘    └────────────┘                │
│                          │ EF Core                                  │
└──────────────────────────┼──────────────────────────────────────────┘
                           │
                           ▼
                  ┌────────────────┐
                  │   SQL Server   │
                  │   Database     │
                  └────────────────┘
```

---

## 📌 Tổng kết Kiến trúc

| Tầng             | Project    | Trách nhiệm                          | Dependencies |
| ---------------- | ---------- | ------------------------------------ | ------------ |
| **Presentation** | `.Web`     | UI, Controllers, ViewModels, Routing | → Service    |
| **Business**     | `.Service` | Business Logic, DTOs, Validation     | → Data       |
| **Data**         | `.Data`    | Entities, Repositories, DB Access    | → Database   |

---

## ⚙️ Công nghệ sử dụng

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor View
- Dependency Injection
- Cloudinary (Image Upload)
- Momo Payment Gateway
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

- Sinh viên: \*\*
- Assignment 1 – ASP.NET Core MVC

---

## 📄 License

Project được sử dụng cho mục đích học tập.
