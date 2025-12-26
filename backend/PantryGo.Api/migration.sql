CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" character varying(1000),
    "Price" numeric(10,2) NOT NULL,
    "Category" character varying(100) NOT NULL,
    "Stock" integer NOT NULL,
    "ImageUrl" character varying(500),
    "Unit" character varying(50),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "Email" character varying(255) NOT NULL,
    "PasswordHash" character varying(255) NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Phone" character varying(20),
    "Role" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Addresses" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Label" character varying(50) NOT NULL,
    "AddressLine" character varying(500) NOT NULL,
    "City" character varying(100) NOT NULL,
    "Pincode" character varying(10) NOT NULL,
    "IsDefault" boolean NOT NULL,
    CONSTRAINT "PK_Addresses" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Addresses_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "AddressId" uuid,
    "Status" character varying(50) NOT NULL,
    "TotalAmount" numeric(10,2) NOT NULL,
    "PaymentId" character varying(255),
    "RazorpayOrderId" character varying(255),
    "IsPaid" boolean NOT NULL,
    "DeliveryPartnerId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Orders_Addresses_AddressId" FOREIGN KEY ("AddressId") REFERENCES "Addresses" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Orders_Users_DeliveryPartnerId" FOREIGN KEY ("DeliveryPartnerId") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Orders_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderItems" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "Price" numeric(10,2) NOT NULL,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE RESTRICT
);

INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111101', 'Fruits', TIMESTAMPTZ '2025-12-25T18:33:15.848522Z', 'Crisp and sweet red apples', '/images/products/apples.jpg', TRUE, 'Fresh Apples', 120.0, 100, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111102', 'Fruits', TIMESTAMPTZ '2025-12-25T18:33:15.848681Z', 'Ripe yellow bananas', '/images/products/bananas.jpg', TRUE, 'Bananas', 40.0, 150, 'dozen');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111103', 'Fruits', TIMESTAMPTZ '2025-12-25T18:33:15.848684Z', 'Juicy oranges rich in Vitamin C', '/images/products/oranges.jpg', TRUE, 'Oranges', 80.0, 80, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111104', 'Fruits', TIMESTAMPTZ '2025-12-25T18:33:15.848685Z', 'Sweet Alphonso mangoes', '/images/products/mangoes.jpg', TRUE, 'Mangoes', 200.0, 50, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111105', 'Fruits', TIMESTAMPTZ '2025-12-25T18:33:15.848686Z', 'Fresh green seedless grapes', '/images/products/grapes.jpg', TRUE, 'Grapes', 90.0, 60, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111201', 'Vegetables', TIMESTAMPTZ '2025-12-25T18:33:15.848686Z', 'Fresh red tomatoes', '/images/products/tomatoes.jpg', TRUE, 'Tomatoes', 30.0, 200, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111202', 'Vegetables', TIMESTAMPTZ '2025-12-25T18:33:15.848686Z', 'Fresh onions', '/images/products/onions.jpg', TRUE, 'Onions', 35.0, 300, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111203', 'Vegetables', TIMESTAMPTZ '2025-12-25T18:33:15.848687Z', 'Farm fresh potatoes', '/images/products/potatoes.jpg', TRUE, 'Potatoes', 25.0, 250, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111204', 'Vegetables', TIMESTAMPTZ '2025-12-25T18:33:15.848687Z', 'Fresh green spinach leaves', '/images/products/spinach.jpg', TRUE, 'Spinach', 20.0, 100, 'bunch');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111205', 'Vegetables', TIMESTAMPTZ '2025-12-25T18:33:15.848688Z', 'Crunchy orange carrots', '/images/products/carrots.jpg', TRUE, 'Carrots', 45.0, 120, 'kg');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111301', 'Dairy', TIMESTAMPTZ '2025-12-25T18:33:15.848688Z', 'Fresh full cream milk', '/images/products/milk.jpg', TRUE, 'Milk', 60.0, 200, 'liter');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111302', 'Dairy', TIMESTAMPTZ '2025-12-25T18:33:15.848689Z', 'Creamy salted butter', '/images/products/butter.jpg', TRUE, 'Butter', 55.0, 80, '200g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111303', 'Dairy', TIMESTAMPTZ '2025-12-25T18:33:15.848689Z', 'Processed cheese slices', '/images/products/cheese.jpg', TRUE, 'Cheese', 120.0, 60, '200g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111304', 'Dairy', TIMESTAMPTZ '2025-12-25T18:33:15.84869Z', 'Fresh plain yogurt', '/images/products/yogurt.jpg', TRUE, 'Yogurt', 45.0, 100, '400g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111305', 'Dairy', TIMESTAMPTZ '2025-12-25T18:33:15.848691Z', 'Fresh cottage cheese', '/images/products/paneer.jpg', TRUE, 'Paneer', 90.0, 70, '200g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111401', 'Snacks', TIMESTAMPTZ '2025-12-25T18:33:15.848691Z', 'Crispy salted potato chips', '/images/products/chips.jpg', TRUE, 'Potato Chips', 30.0, 150, 'pack');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111402', 'Snacks', TIMESTAMPTZ '2025-12-25T18:33:15.848692Z', 'Cream filled biscuits', '/images/products/biscuits.jpg', TRUE, 'Biscuits', 25.0, 200, 'pack');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111403', 'Snacks', TIMESTAMPTZ '2025-12-25T18:33:15.848692Z', 'Mixed crunchy namkeen', '/images/products/namkeen.jpg', TRUE, 'Namkeen', 40.0, 100, '200g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111404', 'Snacks', TIMESTAMPTZ '2025-12-25T18:33:15.848692Z', 'Milk chocolate bar', '/images/products/chocolate.jpg', TRUE, 'Chocolates', 50.0, 120, 'piece');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111405', 'Snacks', TIMESTAMPTZ '2025-12-25T18:33:15.848693Z', 'Roasted mixed nuts', '/images/products/nuts.jpg', TRUE, 'Nuts Mix', 150.0, 80, '200g');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111501', 'Beverages', TIMESTAMPTZ '2025-12-25T18:33:15.848693Z', 'Fresh orange juice', '/images/products/orange-juice.jpg', TRUE, 'Orange Juice', 80.0, 100, 'liter');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111502', 'Beverages', TIMESTAMPTZ '2025-12-25T18:33:15.848694Z', 'Ready to drink cold coffee', '/images/products/cold-coffee.jpg', TRUE, 'Cold Coffee', 45.0, 120, '250ml');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111503', 'Beverages', TIMESTAMPTZ '2025-12-25T18:33:15.848694Z', 'Natural coconut water', '/images/products/coconut-water.jpg', TRUE, 'Coconut Water', 35.0, 150, 'pack');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111504', 'Beverages', TIMESTAMPTZ '2025-12-25T18:33:15.848694Z', 'Organic green tea bags', '/images/products/green-tea.jpg', TRUE, 'Green Tea', 120.0, 80, 'box');
INSERT INTO "Products" ("Id", "Category", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock", "Unit")
VALUES ('11111111-1111-1111-1111-111111111505', 'Beverages', TIMESTAMPTZ '2025-12-25T18:33:15.848695Z', 'Sparkling lemon soda', '/images/products/soda.jpg', TRUE, 'Soda', 25.0, 200, 'can');

CREATE INDEX "IX_Addresses_UserId" ON "Addresses" ("UserId");

CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

CREATE INDEX "IX_OrderItems_ProductId" ON "OrderItems" ("ProductId");

CREATE INDEX "IX_Orders_AddressId" ON "Orders" ("AddressId");

CREATE INDEX "IX_Orders_DeliveryPartnerId" ON "Orders" ("DeliveryPartnerId");

CREATE INDEX "IX_Orders_Status" ON "Orders" ("Status");

CREATE INDEX "IX_Orders_UserId" ON "Orders" ("UserId");

CREATE INDEX "IX_Products_Category" ON "Products" ("Category");

CREATE INDEX "IX_Products_Name" ON "Products" ("Name");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251225183316_InitialCreate', '9.0.1');

COMMIT;

