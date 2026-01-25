-- ============================================
-- E-Commerce Platform - Seed Data Script
-- ============================================
-- This script creates sample data for the local database
-- Run this script after running migrations

USE [ECommercePlatformDB]; -- Database name from appsettings.json
GO

-- Delete old data (if needed)
DELETE FROM reviews;
DELETE FROM order_items;
DELETE FROM payments;
DELETE FROM shipments;
DELETE FROM orders;
DELETE FROM CartItems;
DELETE FROM Carts;
DELETE FROM product_variants;
DELETE FROM products;
DELETE FROM shops;
DELETE FROM categories;
DELETE FROM users;
DELETE FROM roles;
DELETE FROM Refunds
-- GO

-- ============================================
-- 1. ROLES
-- ============================================
INSERT INTO roles (RoleId, Name, Description, CreatedAt)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin', 'System administrator', GETDATE()),
    ('22222222-2222-2222-2222-222222222222', 'Customer', 'Customer', GETDATE()),
    ('33333333-3333-3333-3333-333333333333', 'Seller', 'Shop owner', GETDATE());
GO

-- ============================================
-- 2. USERS
-- ============================================
-- Password: "123456" (BCrypt hash)
-- You can change the password hash as needed
INSERT INTO users (Id, Name, PasswordHash, Email, RoleId, Status, CreatedAt)
VALUES
    -- Admin
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Admin User', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'admin@example.com', '11111111-1111-1111-1111-111111111111', 1, GETDATE()),
    
    -- Sellers
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Shop Owner 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller1@example.com', '33333333-3333-3333-3333-333333333333', 1, GETDATE()),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Shop Owner 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller2@example.com', '33333333-3333-3333-3333-333333333333', 1, GETDATE()),
    
    -- Customers
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Customer 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer1@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE()),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Customer 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer2@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Customer 3', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer3@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE());
GO
SELECT * From users
UPDATE users SET EmailVerified = 1 WHERE EmailVerified = 0;

-- ============================================
-- 3. CATEGORIES
-- ============================================
INSERT INTO categories (Id, Name, Status)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Electronics', 'Active'),
    ('10000000-0000-0000-0000-000000000002', 'Fashion', 'Active'),
    ('10000000-0000-0000-0000-000000000003', 'Home & Kitchen', 'Active'),
    ('10000000-0000-0000-0000-000000000004', 'Books', 'Active'),
    ('10000000-0000-0000-0000-000000000005', 'Sports', 'Active');
GO

-- ============================================
-- 4. SHOPS
-- ============================================
INSERT INTO shops (Id, UserId, ShopName, Description, Status, CreatedAt)
VALUES
    ('20000000-0000-0000-0000-000000000001', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Tech Store', 'Trusted electronics store', 'Active', GETDATE()),
    ('20000000-0000-0000-0000-000000000002', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Fashion Shop', 'Men and women fashion', 'Active', GETDATE());
GO

-- ============================================
-- 5. PRODUCTS
-- ============================================
INSERT INTO products (Id, ShopId, CategoryId, Name, Description, BasePrice, Status, AvgRating, ImageUrl, CreatedAt)
VALUES
    -- Electronics
    ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'iPhone 15 Pro', 'Premium smartphone', 25000000, 'Active', 4.5, 'https://example.com/iphone15.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Samsung Galaxy S24', 'Android flagship phone', 22000000, 'Active', 4.3, 'https://example.com/galaxy-s24.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'MacBook Pro M3', 'Professional laptop', 45000000, 'Active', 4.8, 'https://example.com/macbook-pro.jpg', GETDATE()),
    
    -- Fashion
    ('30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Men Shirt', 'Premium office shirt', 500000, 'Active', 4.2, 'https://example.com/ao-so-mi.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Women Jeans', 'Fashion jeans', 800000, 'Active', 4.0, 'https://example.com/quan-jean.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Sports Shoes', 'Multi-purpose running shoes', 1200000, 'Active', 4.5, 'https://example.com/giay-the-thao.jpg', GETDATE());
GO

-- ============================================
-- 6. PRODUCT VARIANTS
-- ============================================
INSERT INTO product_variants (Id, ProductId, VariantName, Price, Size, Color, Stock, Sku, Status, ImageUrl)
VALUES
    -- iPhone 15 Pro variants
    ('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, '', 'Blue', 50, 'IP15P-128-BLUE', 'Active', 'https://example.com/iphone15-blue.jpg'),
    ('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 256GB - Titanium Blue', 28000000, '', 'White', 30, 'IP15P-256-BLUE', 'Active', 'https://example.com/iphone15-blue.jpg'),
    ('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Black', 25000000, '', 'Black', 40, 'IP15P-128-BLACK', 'Active', 'https://example.com/iphone15-black.jpg'),
    
    -- Samsung Galaxy S24 variants
    ('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 256GB - Phantom Black', 22000000, '', 'White', 35, 'GS24-256-BLACK', 'Active', 'https://example.com/galaxy-s24-black.jpg'),
    ('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 512GB - Phantom Black', 25000000, '', 'Black', 20, 'GS24-512-BLACK', 'Active', 'https://example.com/galaxy-s24-black.jpg'),
    
    -- MacBook Pro variants
    ('40000000-0000-0000-0000-000000000006', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 14" 512GB', 45000000, '14 inch', 'Space White', 15, 'MBP-M3-14-512', 'Active', 'https://example.com/macbook-pro.jpg'),
    ('40000000-0000-0000-0000-000000000007', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 16" 1TB', 55000000, '16 inch', 'Space Gray', 10, 'MBP-M3-16-1TB', 'Active', 'https://example.com/macbook-pro.jpg'),
    
    -- Men Shirt variants
    ('40000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - M', 500000, 'M', 'White', 100, 'ASM-WHITE-M', 'Active', 'https://example.com/ao-so-mi-white.jpg'),
    ('40000000-0000-0000-0000-000000000009', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - L', 500000, 'L', 'White', 80, 'ASM-WHITE-L', 'Active', 'https://example.com/ao-so-mi-white.jpg'),
    ('40000000-0000-0000-0000-000000000010', '30000000-0000-0000-0000-000000000004', 'Men Shirt - Blue - M', 500000, 'M', 'Blue', 90, 'ASM-BLUE-M', 'Active', 'https://example.com/ao-so-mi-blue.jpg'),
    
    -- Women Jeans variants
    ('40000000-0000-0000-0000-000000000011', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 28', 800000, '28', 'Blue', 60, 'QJ-BLUE-28', 'Active', 'https://example.com/quan-jean-blue.jpg'),
    ('40000000-0000-0000-0000-000000000012', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 30', 800000, '30', 'Blue', 70, 'QJ-BLUE-30', 'Active', 'https://example.com/quan-jean-blue.jpg'),
    ('40000000-0000-0000-0000-000000000013', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Black - Size 28', 800000, '28', 'Black', 50, 'QJ-BLACK-28', 'Active', 'https://example.com/quan-jean-black.jpg'),
    
    -- Sports Shoes variants
    ('40000000-0000-0000-0000-000000000014', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - White - Size 40', 1200000, '40', 'White', 45, 'GT-WHITE-40', 'Active', 'https://example.com/giay-white.jpg'),
    ('40000000-0000-0000-0000-000000000015', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - Black - Size 42', 1200000, '42', 'Black', 55, 'GT-BLACK-42', 'Active', 'https://example.com/giay-black.jpg');
GO

-- ============================================
-- 7. CARTS
-- ============================================
INSERT INTO Carts (Id, UserId, Status, CreatedAt)
VALUES
    ('50000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Active', GETDATE()),
    ('50000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Active', GETDATE());
GO

-- ============================================
-- 8. CART ITEMS
-- ============================================
INSERT INTO CartItems (Id, CartId, ProductVariantId, Quantity, CreatedAt)
VALUES
    ('60000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 1, GETDATE()),
    ('60000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 2, GETDATE()),
    ('60000000-0000-0000-0000-000000000003', '50000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000004', 1, GETDATE());
GO

-- ============================================
-- 9. ORDERS
-- ============================================
INSERT INTO orders (Id, UserId, TotalAmount, ShippingAddress, Status, CreatedAt)
VALUES
    ('70000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 25500000, '123 ABC Street, District 1, Ho Chi Minh City', 'Completed', DATEADD(day, -5, GETDATE())),
    ('70000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 800000, '456 XYZ Street, District 2, Ho Chi Minh City', 'Processing', DATEADD(day, -2, GETDATE())),
    ('70000000-0000-0000-0000-000000000003', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 1200000, '789 DEF Street, District 3, Ho Chi Minh City', 'Pending', DATEADD(day, -1, GETDATE()));
GO

-- ============================================
-- 10. ORDER ITEMS
-- ============================================
INSERT INTO order_items (Id, OrderId, ProductVariantId, ProductName, Price, Quantity)
VALUES
    ('80000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, 1),
    ('80000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 'Men Shirt - White - M', 500000, 1),
    
    ('80000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000011', 'Women Jeans - Blue - Size 28', 800000, 1),
    
    ('80000000-0000-0000-0000-000000000004', '70000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000014', 'Sports Shoes - White - Size 40', 1200000, 1);
GO

-- ============================================
-- 11. PAYMENTS
-- ============================================
INSERT INTO payments (Id, OrderId, Method, Amount, Status, TransactionCode, PaidAt)
VALUES
    ('90000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Credit Card', 25500000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-001', DATEADD(day, -5, GETDATE())),
    ('90000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Bank Transfer', 800000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-002', DATEADD(day, -2, GETDATE())),
    ('90000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'COD', 1200000, 'Pending', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-003', GETDATE());
GO

-- ============================================
-- 12. SHIPMENTS
-- ============================================
INSERT INTO shipments (Id, OrderId, Carrier, TrackingCode, Status, UpdatedAt)
VALUES
    ('A0000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Vietnam Post', 'VN123456789', 'Delivered', DATEADD(day, -3, GETDATE())),
    ('A0000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Giao Hang Nhanh', 'GHN987654321', 'In Transit', GETDATE()),
    ('A0000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'Giao Hang Tiet Kiem', 'GHTK555666777', 'Pending', GETDATE());
GO

-- ============================================
-- 13. REVIEWS
-- ============================================
-- Note: Review entity includes ModerationReason, ModeratedAt, ModeratedBy fields
-- These are optional (nullable) fields, so they are set to NULL for approved reviews
INSERT INTO reviews (Id, UserId, ProductId, OrderItemId, Rating, Comment, Status, SpamScore, ToxicityScore, ModerationReason, ModeratedAt, ModeratedBy, CreatedAt)
VALUES
    ('B0000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 5, 'Great product, fast delivery!', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE())),
    ('B0000000-0000-0000-0000-000000000002', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000004', '80000000-0000-0000-0000-000000000002', 4, 'Nice shirt, good quality', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE()));
GO

select * from users
select * from Carts
select * from CartItems
select * from orders where UserId = '2FF60FFD-D586-4E4B-8E2B-049F538919F3'
select * from order_items where OrderId = 'EACDC2EF-376A-4938-B45E-350911AF824F'
select * from payments
select * from Refunds
Update payments set Status = 'PAID' where Id = '9CA8DB48-5D83-4347-8E04-3E71E057DCED'
