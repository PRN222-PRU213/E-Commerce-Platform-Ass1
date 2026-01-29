-- ============================================
-- E-Commerce Platform - Seed Data Script
-- ============================================
-- This script creates sample data for the local database
-- Run this script after running migrations

USE [ECommercePlatformDB]; -- Database name from appsettings.json
GO

SELECT * FROM EKycVerifications

SELECT * FROM users

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
UPDATE users
SET PasswordHash = '123456'
WHERE Email = 'admin@example.com';

Update users SET PasswordHash = '123456' WHERE Email = 'seller1@example.com';

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
UPDATE products SET Status = 'active' WHERE Status = 'Active';
SELECT * FROM products
INSERT INTO products (Id, ShopId, CategoryId, Name, Description, BasePrice, Status, AvgRating, ImageUrl, CreatedAt)
VALUES
    -- Electronics (Tech Store)
    ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'iPhone 15 Pro', 'Premium smartphone with A17 Pro chip, titanium design, and advanced camera system', 25000000, 'Active', 4.5, 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Samsung Galaxy S24', 'Android flagship phone with AI features and stunning display', 22000000, 'Active', 4.3, 'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'MacBook Pro M3', 'Professional laptop with M3 chip for ultimate performance', 45000000, 'Active', 4.8, 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Sony WH-1000XM5 Headphones', 'Industry-leading noise cancellation wireless headphones', 8500000, 'Active', 4.7, 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800', GETDATE()),
    
    -- Fashion (Fashion Shop)
    ('30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Men Shirt', 'Premium office shirt with high-quality cotton fabric', 500000, 'Active', 4.2, 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Women Jeans', 'Fashion jeans with comfortable stretch fit', 800000, 'Active', 4.0, 'https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Sports Shoes', 'Multi-purpose running shoes with breathable mesh', 1200000, 'Active', 4.5, 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Leather Jacket', 'Classic leather jacket with premium genuine leather', 2500000, 'Active', 4.6, 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000009', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Summer Dress', 'Elegant summer dress with floral patterns', 650000, 'Active', 4.3, 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=800', GETDATE()),
    
    -- Home & Kitchen (Tech Store - expanded to Home products)
    ('30000000-0000-0000-0000-000000000010', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000003', 'Coffee Maker', 'Automatic drip coffee maker with programmable timer', 1800000, 'Active', 4.4, 'https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000011', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000003', 'Air Fryer', 'Digital air fryer with 5L capacity and multiple cooking modes', 2200000, 'Active', 4.5, 'https://images.unsplash.com/photo-1648655686227-9c5a418d8f1c?w=800', GETDATE()),
    
    -- Books (Fashion Shop - expanded to Books)
    ('30000000-0000-0000-0000-000000000012', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Clean Code - Robert C. Martin', 'A handbook of agile software craftsmanship', 450000, 'Active', 4.9, 'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=800', GETDATE());
GO

-- ============================================
-- 6. PRODUCT VARIANTS
-- ============================================
INSERT INTO product_variants (Id, ProductId, VariantName, Price, Size, Color, Stock, Sku, Status, ImageUrl)
VALUES
    -- iPhone 15 Pro variants
    ('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, '128GB', 'Blue', 50, 'IP15P-128-BLUE', 'Active', 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800'),
    ('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 256GB - Titanium White', 28000000, '256GB', 'White', 30, 'IP15P-256-WHITE', 'Active', 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800'),
    ('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Black', 25000000, '128GB', 'Black', 40, 'IP15P-128-BLACK', 'Active', 'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=800'),
    
    -- Samsung Galaxy S24 variants
    ('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 256GB - Phantom White', 22000000, '256GB', 'White', 35, 'GS24-256-WHITE', 'Active', 'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=800'),
    ('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 512GB - Phantom Black', 25000000, '512GB', 'Black', 20, 'GS24-512-BLACK', 'Active', 'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=800'),
    
    -- MacBook Pro variants
    ('40000000-0000-0000-0000-000000000006', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 14" 512GB', 45000000, '14 inch', 'Silver', 15, 'MBP-M3-14-512', 'Active', 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800'),
    ('40000000-0000-0000-0000-000000000007', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 16" 1TB', 55000000, '16 inch', 'Space Gray', 10, 'MBP-M3-16-1TB', 'Active', 'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=800'),
    
    -- Sony WH-1000XM5 Headphones variants
    ('40000000-0000-0000-0000-000000000016', '30000000-0000-0000-0000-000000000007', 'Sony WH-1000XM5 - Black', 8500000, 'One Size', 'Black', 25, 'SONY-XM5-BLACK', 'Active', 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800'),
    ('40000000-0000-0000-0000-000000000017', '30000000-0000-0000-0000-000000000007', 'Sony WH-1000XM5 - Silver', 8500000, 'One Size', 'Silver', 20, 'SONY-XM5-SILVER', 'Active', 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=800'),
    
    -- Men Shirt variants
    ('40000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - M', 500000, 'M', 'White', 100, 'ASM-WHITE-M', 'Active', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800'),
    ('40000000-0000-0000-0000-000000000009', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - L', 500000, 'L', 'White', 80, 'ASM-WHITE-L', 'Active', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800'),
    ('40000000-0000-0000-0000-000000000010', '30000000-0000-0000-0000-000000000004', 'Men Shirt - Blue - M', 500000, 'M', 'Blue', 90, 'ASM-BLUE-M', 'Active', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800'),
    
    -- Women Jeans variants
    ('40000000-0000-0000-0000-000000000011', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 28', 800000, '28', 'Blue', 60, 'QJ-BLUE-28', 'Active', 'https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=800'),
    ('40000000-0000-0000-0000-000000000012', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 30', 800000, '30', 'Blue', 70, 'QJ-BLUE-30', 'Active', 'https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=800'),
    ('40000000-0000-0000-0000-000000000013', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Black - Size 28', 800000, '28', 'Black', 50, 'QJ-BLACK-28', 'Active', 'https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=800'),
    
    -- Sports Shoes variants
    ('40000000-0000-0000-0000-000000000014', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - White - Size 40', 1200000, '40', 'White', 45, 'GT-WHITE-40', 'Active', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'),
    ('40000000-0000-0000-0000-000000000015', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - Black - Size 42', 1200000, '42', 'Black', 55, 'GT-BLACK-42', 'Active', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'),
    
    -- Leather Jacket variants
    ('40000000-0000-0000-0000-000000000018', '30000000-0000-0000-0000-000000000008', 'Leather Jacket - Black - M', 2500000, 'M', 'Black', 30, 'LJ-BLACK-M', 'Active', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800'),
    ('40000000-0000-0000-0000-000000000019', '30000000-0000-0000-0000-000000000008', 'Leather Jacket - Black - L', 2500000, 'L', 'Black', 25, 'LJ-BLACK-L', 'Active', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800'),
    ('40000000-0000-0000-0000-000000000020', '30000000-0000-0000-0000-000000000008', 'Leather Jacket - Brown - M', 2600000, 'M', 'Brown', 20, 'LJ-BROWN-M', 'Active', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800'),
    
    -- Summer Dress variants
    ('40000000-0000-0000-0000-000000000021', '30000000-0000-0000-0000-000000000009', 'Summer Dress - Floral - S', 650000, 'S', 'Pink', 40, 'SD-FLORAL-S', 'Active', 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=800'),
    ('40000000-0000-0000-0000-000000000022', '30000000-0000-0000-0000-000000000009', 'Summer Dress - Floral - M', 650000, 'M', 'Pink', 35, 'SD-FLORAL-M', 'Active', 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=800'),
    ('40000000-0000-0000-0000-000000000023', '30000000-0000-0000-0000-000000000009', 'Summer Dress - Blue - M', 680000, 'M', 'Blue', 30, 'SD-BLUE-M', 'Active', 'https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=800'),
    
    -- Coffee Maker variants
    ('40000000-0000-0000-0000-000000000024', '30000000-0000-0000-0000-000000000010', 'Coffee Maker - 12 Cup - Black', 1800000, '12 Cup', 'Black', 20, 'CM-12-BLACK', 'Active', 'https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6?w=800'),
    ('40000000-0000-0000-0000-000000000025', '30000000-0000-0000-0000-000000000010', 'Coffee Maker - 12 Cup - Silver', 1900000, '12 Cup', 'Silver', 15, 'CM-12-SILVER', 'Active', 'https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6?w=800'),
    
    -- Air Fryer variants
    ('40000000-0000-0000-0000-000000000026', '30000000-0000-0000-0000-000000000011', 'Air Fryer - 5L - Black', 2200000, '5L', 'Black', 25, 'AF-5L-BLACK', 'Active', 'https://images.unsplash.com/photo-1648655686227-9c5a418d8f1c?w=800'),
    ('40000000-0000-0000-0000-000000000027', '30000000-0000-0000-0000-000000000011', 'Air Fryer - 7L - Black', 2800000, '7L', 'Black', 15, 'AF-7L-BLACK', 'Active', 'https://images.unsplash.com/photo-1648655686227-9c5a418d8f1c?w=800'),
    
    -- Clean Code Book variants
    ('40000000-0000-0000-0000-000000000028', '30000000-0000-0000-0000-000000000012', 'Clean Code - Paperback', 450000, 'Paperback', 'N/A', 100, 'BOOK-CC-PB', 'Active', 'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=800'),
    ('40000000-0000-0000-0000-000000000029', '30000000-0000-0000-0000-000000000012', 'Clean Code - Hardcover', 650000, 'Hardcover', 'N/A', 50, 'BOOK-CC-HC', 'Active', 'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=800');
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
