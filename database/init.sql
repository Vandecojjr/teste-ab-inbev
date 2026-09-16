CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                                                       "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
    );

START TRANSACTION;

CREATE TABLE "Users" (
                         "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
                         "Username" character varying(50) NOT NULL,
                         "Password" character varying(100) NOT NULL,
                         "Phone" character varying(20) NOT NULL,
                         "Email" character varying(100) NOT NULL,
                         "Status" character varying(20) NOT NULL,
                         "Role" character varying(20) NOT NULL,
                         CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241014011203_InitialMigrations', '8.0.10');

COMMIT;

START TRANSACTION;

ALTER TABLE "Users" ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Users" ADD "UpdatedAt" timestamp with time zone;

CREATE TABLE "Sales" (
                         "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
                         "SaleNumber" character varying(50) NOT NULL,
                         "SaleDate" timestamp with time zone NOT NULL,
                         "CustomerId" integer NOT NULL,
                         "CustomerName" character varying(100) NOT NULL,
                         "BranchId" integer NOT NULL,
                         "BranchName" character varying(100) NOT NULL,
                         "IsCancelled" boolean NOT NULL,
                         CONSTRAINT "PK_Sales" PRIMARY KEY ("Id")
);

CREATE TABLE "SaleItems" (
                             "Id" uuid NOT NULL DEFAULT (gen_random_uuid()),
                             "SaleId" uuid NOT NULL,
                             "ProductId" integer NOT NULL,
                             "ProductName" character varying(100) NOT NULL,
                             "Quantity" integer NOT NULL,
                             "UnitPrice" numeric(18,2) NOT NULL,
                             "DiscountPercentage" numeric(5,2) NOT NULL,
                             "IsCancelled" boolean NOT NULL,
                             CONSTRAINT "PK_SaleItems" PRIMARY KEY ("Id"),
                             CONSTRAINT "FK_SaleItems_Sales_SaleId" FOREIGN KEY ("SaleId") REFERENCES "Sales" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_SaleItems_SaleId" ON "SaleItems" ("SaleId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260916055618_AddSales', '8.0.10');

COMMIT;

START TRANSACTION;

DO $$ 
DECLARE
v_sale_id uuid;
BEGIN
    -- Seed User
    IF NOT EXISTS (SELECT 1 FROM "Users" WHERE "Email" = 'admin@teste.com') THEN
        INSERT INTO "Users" ("Id", "Username", "Email", "Phone", "Password", "Role", "Status", "CreatedAt")
        VALUES (gen_random_uuid(), 'admin', 'admin@teste.com', '11999999999', '$2a$11$3WM9tet6zsAij.5gj6fPz.SH3JeDfWFSn3gHhYVpmgseJIGQD1ray', 'Admin', 'Active', NOW());
END IF;

    -- Seed Sales
    IF NOT EXISTS (SELECT 1 FROM "Sales") THEN
        FOR i IN 1..10 LOOP
            v_sale_id := gen_random_uuid();

INSERT INTO "Sales" ("Id", "SaleNumber", "SaleDate", "CustomerId", "CustomerName", "BranchId", "BranchName", "IsCancelled")
VALUES (v_sale_id, 'SALE-000' || i, NOW() - (i || ' days')::interval, 100 + i, 'Customer ' || i, 10 + i, 'Branch ' || i, false);

INSERT INTO "SaleItems" ("Id", "SaleId", "ProductId", "ProductName", "Quantity", "UnitPrice", "DiscountPercentage", "IsCancelled")
VALUES (gen_random_uuid(), v_sale_id, 200 + i, 'Product ' || i, 2, 50.00, 0, false);

INSERT INTO "SaleItems" ("Id", "SaleId", "ProductId", "ProductName", "Quantity", "UnitPrice", "DiscountPercentage", "IsCancelled")
VALUES (gen_random_uuid(), v_sale_id, 300 + i, 'Product ' || (i+10), 10, 100.00, 0.20, false);
END LOOP;
END IF;
END $$;

COMMIT;
