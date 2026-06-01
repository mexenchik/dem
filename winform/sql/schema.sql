-- SQL Server schema for the LightStep demo exam task.
-- Use this script in SSMS if the exam requires a real DB script.

IF DB_ID(N'LightStepExam') IS NOT NULL
BEGIN
    ALTER DATABASE LightStepExam SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LightStepExam;
END;
GO

CREATE DATABASE LightStepExam;
GO

USE LightStepExam;
GO

CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(64) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    RoleId INT NOT NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Article NVARCHAR(40) NOT NULL UNIQUE,
    Category NVARCHAR(80) NOT NULL,
    Description NVARCHAR(300) NULL,
    Brand NVARCHAR(80) NOT NULL,
    Supplier NVARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Size NVARCHAR(20) NOT NULL,
    Unit NVARCHAR(20) NOT NULL DEFAULT N'пара',
    Quantity INT NOT NULL,
    DiscountPercent INT NOT NULL DEFAULT 0,
    ImagePath NVARCHAR(260) NULL,
    CONSTRAINT CK_Products_Price CHECK (Price > 0),
    CONSTRAINT CK_Products_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CK_Products_Discount CHECK (DiscountPercent BETWEEN 0 AND 100)
);

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    Status NVARCHAR(40) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT CK_Orders_Total CHECK (TotalAmount >= 0)
);

CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Count INT NOT NULL,
    PriceAtMoment DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_OrderItems_Count CHECK (Count > 0),
    CONSTRAINT CK_OrderItems_Price CHECK (PriceAtMoment > 0)
);

INSERT INTO Roles (RoleName)
VALUES (N'Client'), (N'Manager'), (N'Admin');

INSERT INTO Users (Login, PasswordHash, FullName, RoleId)
VALUES
    (N'client', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'client123'), 2), N'Иван Клиентов', 1),
    (N'manager', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'manager123'), 2), N'Мария Менеджерова', 2),
    (N'admin', CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', 'admin123'), 2), N'Алексей Админов', 3);

INSERT INTO Products (Name, Article, Category, Description, Brand, Supplier, Price, Size, Unit, Quantity, DiscountPercent, ImagePath)
VALUES
    (N'Кроссовки городские', N'SH-1001', N'Кроссовки', N'Повседневная обувь для города', N'EasyStep', N'Поставка Север', 5200.00, N'42', N'пара', 12, 5, NULL),
    (N'Ботинки зимние', N'SH-1002', N'Ботинки', N'Утепленная зимняя модель', N'NordWay', N'Поставка Север', 8900.00, N'43', N'пара', 4, 20, NULL),
    (N'Туфли классические', N'SH-1003', N'Туфли', N'Офисная обувь', N'FormalLine', N'ГородСклад', 7400.00, N'41', N'пара', 0, 0, NULL),
    (N'Кеды текстильные', N'SH-1004', N'Кеды', N'Легкая летняя обувь', N'StreetGo', N'ГородСклад', 3100.00, N'39', N'пара', 25, 10, NULL),
    (N'Сапоги демисезонные', N'SH-1005', N'Сапоги', N'Женская демисезонная модель', N'EasyStep', N'Мир обуви', 6700.00, N'38', N'пара', 7, 18, NULL);

INSERT INTO Orders (UserId, OrderDate, Status, TotalAmount)
VALUES
    (1, '2026-05-20T10:00:00', N'Новый', 5200.00),
    (1, '2026-05-21T12:30:00', N'Выдан', 6200.00);

INSERT INTO OrderItems (OrderId, ProductId, Count, PriceAtMoment)
VALUES
    (1, 1, 1, 5200.00),
    (2, 4, 2, 3100.00);

SELECT
    Products.Id,
    Products.Name,
    Products.Article,
    Products.Brand,
    Products.Price,
    Products.Quantity,
    Products.DiscountPercent
FROM Products
ORDER BY Products.Name;
