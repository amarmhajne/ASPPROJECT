CREATE DATABASE IF NOT EXISTS sales_db;
USE sales_db;

-- טבלה 1: עובדים (כולל שדה Role)
CREATE TABLE IF NOT EXISTS Employees (
  EmployeeId INT AUTO_INCREMENT PRIMARY KEY,
  FirstName NVARCHAR(50) NOT NULL,
  LastName NVARCHAR(50) NOT NULL,
  Username VARCHAR(50) NOT NULL UNIQUE,
  PasswordHash VARCHAR(255) NOT NULL,
  Role ENUM('Admin','Sales') NOT NULL,
  Phone VARCHAR(20),
  Email VARCHAR(100),
  HireDate DATE,
  IsActive BOOLEAN DEFAULT TRUE
);

-- טבלה 2: ספקים
CREATE TABLE IF NOT EXISTS Suppliers (
  SupplierId INT AUTO_INCREMENT PRIMARY KEY,
  CompanyName NVARCHAR(100) NOT NULL,
  ContactName NVARCHAR(100),
  Phone VARCHAR(20),
  Email VARCHAR(100),
  Address NVARCHAR(200),
  IsActive BOOLEAN DEFAULT TRUE
);

-- טבלה 3: מוצרים
CREATE TABLE IF NOT EXISTS Products (
  ProductId INT AUTO_INCREMENT PRIMARY KEY,
  ProductName NVARCHAR(100) NOT NULL,
  SupplierId INT,
  Category NVARCHAR(50),
  UnitPrice DECIMAL(10,2) NOT NULL,
  IsActive BOOLEAN DEFAULT TRUE,
  FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
);

-- טבלה 4: מלאי
CREATE TABLE IF NOT EXISTS Inventory (
  InventoryId INT AUTO_INCREMENT PRIMARY KEY,
  ProductId INT NOT NULL,
  Quantity INT NOT NULL DEFAULT 0,
  MinQuantity INT DEFAULT 5,
  LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- טבלה 5: לקוחות
CREATE TABLE IF NOT EXISTS Customers (
  CustomerId INT AUTO_INCREMENT PRIMARY KEY,
  FirstName NVARCHAR(50) NOT NULL,
  LastName NVARCHAR(50) NOT NULL,
  Phone VARCHAR(20),
  Email VARCHAR(100),
  Address NVARCHAR(200),
  CreatedDate DATE DEFAULT (CURDATE()),
  IsActive BOOLEAN DEFAULT TRUE
);

-- טבלה 6: הזמנות
CREATE TABLE IF NOT EXISTS Orders (
  OrderId INT AUTO_INCREMENT PRIMARY KEY,
  CustomerId INT NOT NULL,
  EmployeeId INT NOT NULL,
  OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  Status ENUM('Pending','Processing','Completed','Cancelled') DEFAULT 'Pending',
  TotalAmount DECIMAL(10,2),
  Notes NVARCHAR(500),
  FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
  FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

-- טבלה 7: פריטי הזמנה
CREATE TABLE IF NOT EXISTS OrderItems (
  OrderItemId INT AUTO_INCREMENT PRIMARY KEY,
  OrderId INT NOT NULL,
  ProductId INT NOT NULL,
  Quantity INT NOT NULL,
  UnitPrice DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
  FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- טבלה 8: מכירות
CREATE TABLE IF NOT EXISTS Sales (
  SaleId INT AUTO_INCREMENT PRIMARY KEY,
  OrderId INT NOT NULL,
  EmployeeId INT NOT NULL,
  SaleDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  AmountPaid DECIMAL(10,2) NOT NULL,
  PaymentMethod ENUM('Cash','CreditCard','BankTransfer') NOT NULL,
  FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
  FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId)
);

-- הכנס משתמש Admin ראשוני (סיסמה: Admin123)
INSERT INTO Employees (FirstName, LastName, Username, PasswordHash, Role)
VALUES ('מנהל', 'ראשי', 'admin', SHA2('Admin123', 256), 'Admin')
ON DUPLICATE KEY UPDATE Username = 'admin';
