# 📦 Order Status Flow - Shop Management

**Ngày cập nhật:** 21/01/2026

---

## 🔄 LUỒNG TRẠNG THÁI ĐƠN HÀNG MỚI

### **Order Status Flow**

```
┌──────────┐
│ Pending  │  Customer tạo đơn, thanh toán thành công
└────┬─────┘
     │ Shop click "Bắt đầu xử lý"
     ▼
┌────────────┐
│ Processing │  Đang xử lý đơn hàng
└────┬───────┘
     │ Shop click "Bắt đầu chuẩn bị hàng"
     ▼
┌───────────┐
│ Preparing │  Đang chuẩn bị hàng, đóng gói
└────┬──────┘
     │ Shop nhập Carrier + TrackingCode
     │ Shop click "Chuyển giao vận chuyển"
     ▼
┌──────────┐
│ Shipped  │  Hàng đang vận chuyển
└────┬─────┘
     │ Shop click "Xác nhận đã giao hàng"
     ▼
┌───────────┐
│ Completed │  Giao hàng thành công
└───────────┘

Alternative:
┌──────────┐
│ Pending  │ → Reject → Cancelled
└──────────┘
```

---

## 📋 CHI TIẾT TRẠNG THÁI

### **1. Pending** - Chờ xử lý ⏳

**Màu:** Badge warning (vàng)  
**Icon:** `fa-clock`  
**Mô tả:** Đơn hàng mới từ customer, đã thanh toán thành công  
**Actions:**

- ✅ Bắt đầu xử lý → `Processing`
- ❌ Từ chối → `Cancelled`

---

### **2. Processing** - Đang xử lý ⚙️

**Màu:** Badge info (xanh dương nhạt)  
**Icon:** `fa-cog fa-spin`  
**Mô tả:** Shop đã nhận và bắt đầu xử lý đơn  
**Actions:**

- ✅ Bắt đầu chuẩn bị hàng → `Preparing`

---

### **3. Preparing** - Đang chuẩn bị hàng 📦

**Màu:** Badge primary (xanh dương)  
**Icon:** `fa-box-open`  
**Mô tả:** Đang đóng gói, chuẩn bị sẵn sàng để giao  
**Actions:**

- ✅ Chuyển giao vận chuyển (nhập Carrier + TrackingCode) → `Shipped`

**Form cần nhập:**

```
- Đơn vị vận chuyển: GHN, GHTK, VNPost, J&T, etc.
- Mã vận đơn: VD GHN123456789
```

---

### **4. Shipped** - Chuyển giao vận chuyển 🚚

**Màu:** Badge primary (xanh dương)  
**Icon:** `fa-shipping-fast`  
**Mô tả:** Hàng đã giao cho đơn vị vận chuyển, đang trên đường  
**Actions:**

- ✅ Xác nhận đã giao hàng → `Completed`
- 🔄 Cập nhật thông tin vận chuyển (Carrier, TrackingCode, Shipment Status)

**Shipment Status:**

- `Pending` - Đang chờ
- `Shipping` - Đang vận chuyển (default khi tạo)
- `Delivered` - Đã giao (tự động khi mark completed)

---

### **5. Completed** - Đã hoàn thành ✅

**Màu:** Badge success (xanh lá)  
**Icon:** `fa-check-circle`  
**Mô tả:** Giao hàng thành công, đơn hoàn tất  
**Actions:** Không có (trạng thái cuối)

---

### **6. Cancelled** - Đơn hàng bị hủy ❌

**Màu:** Badge danger (đỏ)  
**Icon:** `fa-times-circle`  
**Mô tả:** Đơn hàng bị từ chối (chỉ từ Pending)  
**Actions:** Không có (trạng thái cuối)

---

## 🚚 SHIPMENT STATUS

| Status        | Vietnamese      | Icon               | Màu     | Khi nào                       |
| ------------- | --------------- | ------------------ | ------- | ----------------------------- |
| **Pending**   | Đang chờ        | `fa-clock`         | Warning | Shipment chưa được xử lý      |
| **Shipping**  | Đang vận chuyển | `fa-shipping-fast` | Info    | Default khi tạo shipment      |
| **Delivered** | Đã giao         | `fa-check-circle`  | Success | Tự động khi order → Completed |

---

## 📊 STATISTICS MAPPING

```csharp
public class ShopOrderStatistics
{
    public int TotalOrders { get; set; }         // Tất cả
    public int PendingOrders { get; set; }       // Status = "Pending"
    public int ConfirmedOrders { get; set; }     // Status = "Processing" OR "Preparing"
    public int ShippingOrders { get; set; }      // Status = "Shipped"
    public int DeliveredOrders { get; set; }     // Status = "Completed"
    public int CancelledOrders { get; set; }     // Status = "Cancelled"
    public decimal TotalRevenue { get; set; }    // Sum where Status = "Completed"
}
```

---

## 🎯 CONTROLLER ACTIONS

| Action              | Method | Route                              | Description                                   |
| ------------------- | ------ | ---------------------------------- | --------------------------------------------- |
| **StartProcessing** | POST   | `/ShopOrders/StartProcessing/{id}` | Pending → Processing                          |
| **StartPreparing**  | POST   | `/ShopOrders/StartPreparing/{id}`  | Processing → Preparing                        |
| **Ship**            | POST   | `/ShopOrders/Ship/{id}`            | Preparing → Shipped (+ create Shipment)       |
| **UpdateShipment**  | POST   | `/ShopOrders/UpdateShipment/{id}`  | Update Carrier, TrackingCode, Shipment Status |
| **MarkDelivered**   | POST   | `/ShopOrders/MarkDelivered/{id}`   | Shipped → Completed                           |
| **Reject**          | POST   | `/ShopOrders/Reject/{id}`          | Pending → Cancelled                           |

---

## 🖼️ UI COMPONENTS

### **Detail.cshtml - Actions Section**

**Pending:**

```html
<button>Bắt đầu xử lý</button> <button>Từ chối đơn hàng</button>
```

**Processing:**

```html
<button>Bắt đầu chuẩn bị hàng</button>
```

**Preparing:**

```html
<form>
  <select>Đơn vị vận chuyển</select>
  <input>Mã vận đơn</input>
  <button>Chuyển giao vận chuyển</button>
</form>
```

**Shipped:**

```html
<button>Xác nhận đã giao hàng</button>
<!-- Shipment Info Card -->
<form>Cập nhật vận chuyển</form>
```

**Completed / Cancelled:**

```html
<div class="alert">Đơn hàng đã hoàn thành/bị hủy</div>
```

---

### **Index.cshtml - Filter Buttons**

```html
<button>Tất cả</button>
<button>Chờ xử lý (Pending)</button>
<button>Đang xử lý (Processing)</button>
<button>Chuẩn bị hàng (Preparing)</button>
<button>Vận chuyển (Shipped)</button>
<button>Hoàn thành (Completed)</button>
<button>Đã hủy (Cancelled)</button>
```

---

## 🔄 SERVICE METHODS

### **IShopOrderService**

```csharp
Task<ServiceResult> StartProcessingAsync(Guid orderId, Guid shopId);
Task<ServiceResult> StartPreparingAsync(Guid orderId, Guid shopId);
Task<ServiceResult> ShipOrderAsync(Guid orderId, Guid shopId, CreateShipmentDto dto);
Task<ServiceResult> UpdateShipmentAsync(Guid orderId, Guid shopId, UpdateShipmentDto dto);
Task<ServiceResult> MarkAsDeliveredAsync(Guid orderId, Guid shopId);
Task<ServiceResult> RejectOrderAsync(Guid orderId, Guid shopId, string? reason);
```

---

## ✅ VALIDATION RULES

| Action              | Current Status Required | Next Status  |
| ------------------- | ----------------------- | ------------ |
| **StartProcessing** | `Pending`               | `Processing` |
| **StartPreparing**  | `Processing`            | `Preparing`  |
| **Ship**            | `Preparing`             | `Shipped`    |
| **UpdateShipment**  | `Shipped`               | (no change)  |
| **MarkDelivered**   | `Shipped`               | `Completed`  |
| **Reject**          | `Pending`               | `Cancelled`  |

---

## 🎨 COLOR SCHEME

| Status     | Color Class     | Hex Code  | Usage                   |
| ---------- | --------------- | --------- | ----------------------- |
| Pending    | `badge-warning` | `#ffc107` | Vàng - Chờ action       |
| Processing | `badge-info`    | `#17a2b8` | Xanh nhạt - Đang xử lý  |
| Preparing  | `badge-primary` | `#007bff` | Xanh dương - Chuẩn bị   |
| Shipped    | `badge-primary` | `#007bff` | Xanh dương - Vận chuyển |
| Completed  | `badge-success` | `#28a745` | Xanh lá - Thành công    |
| Cancelled  | `badge-danger`  | `#dc3545` | Đỏ - Thất bại           |

---

## 📝 NOTES

1. **Shipment chỉ được tạo khi:** Order chuyển từ `Preparing` → `Shipped`
2. **Shipment Info hiển thị khi:** Order ở trạng thái `Shipped` hoặc `Completed`
3. **Form Update Shipment hiển thị khi:** Order = `Shipped` VÀ Shipment.Status != `Delivered`
4. **Payment luôn là `Completed`** (do thanh toán Momo thành công trước khi tạo order)
5. **Reject chỉ cho phép ở `Pending`** - không thể hủy khi đã xử lý

---

## 🔗 FILES CHANGED

- ✅ [Detail.cshtml](E-Commerce-Platform-Ass1.Web/Views/ShopOrders/Detail.cshtml)
- ✅ [Index.cshtml](E-Commerce-Platform-Ass1.Web/Views/ShopOrders/Index.cshtml)
- ✅ [ShopOrdersController.cs](E-Commerce-Platform-Ass1.Web/Controllers/ShopOrdersController.cs)
- ✅ [IShopOrderService.cs](E-Commerce-Platform-Ass1.Service/Services/IServices/IShopOrderService.cs)
- ✅ [ShopOrderService.cs](E-Commerce-Platform-Ass1.Service/Services/ShopOrderService.cs)

---

_Luồng này tối ưu cho quản lý đơn hàng của shop với các bước rõ ràng từ nhận đơn đến giao hàng thành công._
