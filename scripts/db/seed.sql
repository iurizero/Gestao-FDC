INSERT INTO Categories (Name, Description)
SELECT 'Salgados', 'Salgados fritos e assados'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Salgados');

INSERT INTO Categories (Name, Description)
SELECT 'Bebidas', 'Sucos, refrigerantes e aguas'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Bebidas');

INSERT INTO Categories (Name, Description)
SELECT 'Doces', 'Sobremesas e doces variados'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Doces');

INSERT INTO Categories (Name, Description)
SELECT 'Lanches', 'Hamburgueres e sanduiches'
WHERE NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Lanches');

INSERT INTO Products (Name, Description, Price, CategoryId, StockQuantity, TrackStock)
SELECT 'Coxinha de Frango', 'Massa leve com recheio de frango', 5.50, Id, 50, 1
FROM Categories
WHERE Name = 'Salgados'
  AND NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Coxinha de Frango');

INSERT INTO Products (Name, Description, Price, CategoryId, StockQuantity, TrackStock)
SELECT 'Coca-Cola 350ml', 'Refrigerante lata', 5.00, Id, 100, 1
FROM Categories
WHERE Name = 'Bebidas'
  AND NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Coca-Cola 350ml');

INSERT INTO Products (Name, Description, Price, CategoryId, StockQuantity, TrackStock)
SELECT 'Brigadeiro Gourmet', 'Doce artesanal', 4.50, Id, 40, 1
FROM Categories
WHERE Name = 'Doces'
  AND NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Brigadeiro Gourmet');

INSERT INTO InventoryItems (Name, Quantity, Unit, MinQuantity, UnitCost)
SELECT 'Farinha de Trigo', 10, 'kg', 2, 4.50
WHERE NOT EXISTS (SELECT 1 FROM InventoryItems WHERE Name = 'Farinha de Trigo');

INSERT INTO InventoryItems (Name, Quantity, Unit, MinQuantity, UnitCost)
SELECT 'Oleo de Soja', 5, 'l', 1, 7.20
WHERE NOT EXISTS (SELECT 1 FROM InventoryItems WHERE Name = 'Oleo de Soja');

INSERT INTO InventoryItems (Name, Quantity, Unit, MinQuantity, UnitCost)
SELECT 'Frango Desfiado', 3, 'kg', 1, 15.00
WHERE NOT EXISTS (SELECT 1 FROM InventoryItems WHERE Name = 'Frango Desfiado');
