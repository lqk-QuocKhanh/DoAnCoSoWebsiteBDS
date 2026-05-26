/*
================================================================================
  VietPropEstate — FULL DATABASE SCHEMA (PostgreSQL)
  Migration: 20260524210825_InitialPostgresIdentity
  Generated from EF Core migrations for Render / local PostgreSQL.

  Includes:
  - All tables (Identity, Properties, Provinces, Wards, VIP, Chat, …)
  - Indexes & foreign keys
  - Seed: PropertyTypes, TransactionTypes, VIPPackages

  NOT included (seeded by WebAPI on startup when tables are empty):
  - AspNetUsers / admin account
  - Provinces / Wards (AddressDataSeeder)
  - Properties (ProductionDataSeeder — 60 listings)

  Usage (empty database):
    psql "$DATABASE_URL" -f full-database-postgres.sql

  Idempotent reference data only:
    psql "$DATABASE_URL" -f seed-reference-data-idempotent.sql
================================================================================
*/

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Agents" (
    "Id" uuid NOT NULL,
    "FullName" character varying(300) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PhoneNumber" character varying(20) NOT NULL,
    "LicenseNumber" character varying(100),
    "AgencyName" character varying(300),
    "AvatarUrl" character varying(2000),
    "Bio" character varying(3000),
    "IsActive" boolean NOT NULL,
    "UserId" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Agents" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
    "Id" text NOT NULL,
    "FirstName" text,
    "LastName" text,
    "AvatarUrl" text,
    "AddressLine" text,
    "ProvinceName" text,
    "WardName" text,
    "ProvinceCode" integer,
    "WardCode" integer,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastLoginAt" timestamp with time zone,
    "IsActive" boolean NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

CREATE TABLE "AuditLogs" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450),
    "Action" integer NOT NULL,
    "EntityName" character varying(200) NOT NULL,
    "EntityId" character varying(450),
    "OldValues" text,
    "NewValues" text,
    "AffectedColumns" text,
    "IpAddress" character varying(45),
    "UserAgent" character varying(500),
    "Timestamp" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("Id")
);

CREATE TABLE "Customers" (
    "Id" uuid NOT NULL,
    "FullName" character varying(300) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PhoneNumber" character varying(20) NOT NULL,
    "CustomerType" integer NOT NULL,
    "Notes" character varying(3000),
    "UserId" text,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Customers" PRIMARY KEY ("Id")
);

CREATE TABLE "Notifications" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Content" character varying(2000) NOT NULL,
    "Type" integer NOT NULL,
    "IsRead" boolean NOT NULL,
    "ReadAt" timestamp with time zone,
    "ReferenceId" character varying(450),
    "ActionUrl" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Notifications" PRIMARY KEY ("Id")
);

CREATE TABLE "PropertyTypes" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_PropertyTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "Provinces" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Code" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Codename" character varying(200) NOT NULL,
    "DivisionType" character varying(100) NOT NULL,
    "PhoneCode" integer NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Provinces" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_Provinces_Code" UNIQUE ("Code")
);

CREATE TABLE "RefreshTokens" (
    "Id" uuid NOT NULL,
    "Token" character varying(500) NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedByIp" character varying(45),
    "IsRevoked" boolean NOT NULL,
    "RevokedAt" timestamp with time zone,
    "RevokedByIp" character varying(45),
    "RevokedReason" character varying(500),
    "ReplacedByToken" character varying(500),
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id")
);

CREATE TABLE "TransactionTypes" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(1000),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_TransactionTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "UserBlocks" (
    "Id" uuid NOT NULL,
    "BlockerId" character varying(450) NOT NULL,
    "BlockedUserId" character varying(450) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_UserBlocks" PRIMARY KEY ("Id")
);

CREATE TABLE "VIPPackages" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" character varying(2000),
    "DurationDays" integer NOT NULL,
    "PriceAmount" numeric(18,2) NOT NULL,
    "PriceCurrency" character varying(10) NOT NULL,
    "MaxListings" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_VIPPackages" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Wards" (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Code" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Codename" character varying(200) NOT NULL,
    "DivisionType" character varying(100) NOT NULL,
    "ProvinceCode" integer NOT NULL,
    "ProvinceId" integer NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT TRUE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Wards" PRIMARY KEY ("Id"),
    CONSTRAINT "AK_Wards_Code" UNIQUE ("Code"),
    CONSTRAINT "FK_Wards_Provinces_ProvinceId" FOREIGN KEY ("ProvinceId") REFERENCES "Provinces" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "UserVIPPackages" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "VIPPackageId" uuid NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "RemainingListings" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_UserVIPPackages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserVIPPackages_VIPPackages_VIPPackageId" FOREIGN KEY ("VIPPackageId") REFERENCES "VIPPackages" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Properties" (
    "Id" uuid NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Slug" character varying(600) NOT NULL,
    "Description" character varying(5000),
    "PriceAmount" numeric(18,2) NOT NULL,
    "PriceCurrency" character varying(10) NOT NULL,
    "Area" numeric(18,4) NOT NULL,
    "NumberOfBedrooms" integer,
    "NumberOfBathrooms" integer,
    "NumberOfFloors" integer,
    "Status" integer NOT NULL,
    "ListingType" integer NOT NULL,
    "Direction" integer,
    "Street" character varying(500) NOT NULL,
    "AddressWard" character varying(200) NOT NULL,
    "AddressDistrict" character varying(200) NOT NULL,
    "AddressProvince" character varying(200) NOT NULL,
    "AddressCountry" character varying(100) NOT NULL,
    "AddressLatitude" double precision,
    "AddressLongitude" double precision,
    "ViewCount" integer NOT NULL DEFAULT 0,
    "IsFeatured" boolean NOT NULL DEFAULT FALSE,
    "PublishedAt" timestamp with time zone,
    "ExpiresAt" timestamp with time zone,
    "VideoUrl" text,
    "PropertyTypeId" uuid NOT NULL,
    "AgentId" uuid NOT NULL,
    "TransactionTypeId" uuid,
    "ProvinceCode" integer,
    "ProvinceName" character varying(200),
    "WardCode" integer,
    "WardName" character varying(200),
    "FullAddress" character varying(1000),
    "Latitude" double precision,
    "Longitude" double precision,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Properties" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Properties_Agents_AgentId" FOREIGN KEY ("AgentId") REFERENCES "Agents" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Properties_PropertyTypes_PropertyTypeId" FOREIGN KEY ("PropertyTypeId") REFERENCES "PropertyTypes" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Properties_Provinces_ProvinceCode" FOREIGN KEY ("ProvinceCode") REFERENCES "Provinces" ("Code") ON DELETE SET NULL,
    CONSTRAINT "FK_Properties_TransactionTypes_TransactionTypeId" FOREIGN KEY ("TransactionTypeId") REFERENCES "TransactionTypes" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Properties_Wards_WardCode" FOREIGN KEY ("WardCode") REFERENCES "Wards" ("Code") ON DELETE SET NULL
);

CREATE TABLE "Payments" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "UserVIPPackageId" uuid,
    "VIPPackageId" uuid,
    "Amount" numeric(18,2) NOT NULL,
    "Currency" character varying(10) NOT NULL,
    "Status" integer NOT NULL,
    "PaymentReference" character varying(100) NOT NULL,
    "PaymentMethod" character varying(100),
    "OrderInfo" character varying(500),
    "VnpayTransactionId" character varying(200),
    "BankCode" character varying(50),
    "GatewayResponse" text,
    "IpAddress" character varying(45),
    "PaidAt" timestamp with time zone,
    "Notes" character varying(2000),
    "TransactionCode" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Payments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Payments_UserVIPPackages_UserVIPPackageId" FOREIGN KEY ("UserVIPPackageId") REFERENCES "UserVIPPackages" ("Id") ON DELETE SET NULL
);

CREATE TABLE "Conversations" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "BuyerId" character varying(450) NOT NULL,
    "SellerId" character varying(450) NOT NULL,
    "IsClosed" boolean NOT NULL,
    "ClosedAt" timestamp with time zone,
    "Subject" character varying(500),
    "LastMessageAt" timestamp with time zone,
    "LastMessagePreview" character varying(150),
    "BuyerUnreadCount" integer NOT NULL DEFAULT 0,
    "SellerUnreadCount" integer NOT NULL DEFAULT 0,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Conversations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Conversations_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Favorites" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "PropertyId" uuid NOT NULL,
    "Note" character varying(1000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Favorites" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Favorites_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PropertyImages" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "Url" character varying(2000) NOT NULL,
    "Caption" character varying(500),
    "DisplayOrder" integer NOT NULL DEFAULT 0,
    "IsPrimary" boolean NOT NULL DEFAULT FALSE,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_PropertyImages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PropertyImages_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PropertyViews" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "UserId" character varying(450),
    "IpAddress" character varying(45),
    "UserAgent" character varying(500),
    "SessionId" character varying(200),
    "ViewedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_PropertyViews" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PropertyViews_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Transactions" (
    "Id" uuid NOT NULL,
    "PropertyId" uuid NOT NULL,
    "CustomerId" uuid NOT NULL,
    "AgentId" uuid NOT NULL,
    "TransactionType" integer NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Currency" character varying(10) NOT NULL,
    "CommissionAmount" numeric(18,2),
    "CommissionCurrency" character varying(10),
    "Status" integer NOT NULL,
    "CompletedAt" timestamp with time zone,
    "Notes" character varying(3000),
    "ContractNumber" character varying(100),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Transactions_Agents_AgentId" FOREIGN KEY ("AgentId") REFERENCES "Agents" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transactions_Customers_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Customers" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Transactions_Properties_PropertyId" FOREIGN KEY ("PropertyId") REFERENCES "Properties" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "Messages" (
    "Id" uuid NOT NULL,
    "ConversationId" uuid NOT NULL,
    "SenderId" character varying(450) NOT NULL,
    "Content" character varying(4000) NOT NULL,
    "Status" integer NOT NULL,
    "ReadAt" timestamp with time zone,
    "AttachmentUrl" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_Messages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Messages_Conversations_ConversationId" FOREIGN KEY ("ConversationId") REFERENCES "Conversations" ("Id") ON DELETE CASCADE
);

CREATE TABLE "UserConversationSettings" (
    "Id" uuid NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "ConversationId" uuid NOT NULL,
    "IsPinned" boolean NOT NULL,
    "IsMuted" boolean NOT NULL,
    "PinnedAt" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text,
    "LastModifiedAt" timestamp with time zone,
    "LastModifiedBy" text,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "DeletedBy" text,
    "RowVersion" bytea NOT NULL DEFAULT BYTEA E'\\x',
    CONSTRAINT "PK_UserConversationSettings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserConversationSettings_Conversations_ConversationId" FOREIGN KEY ("ConversationId") REFERENCES "Conversations" ("Id") ON DELETE CASCADE
);

INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('11111111-1111-1111-1111-111111111111', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Detached or semi-detached house', TRUE, FALSE, NULL, NULL, 'House', BYTEA E'\\x');
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('22222222-2222-2222-2222-222222222222', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Commercial office space', TRUE, FALSE, NULL, NULL, 'Office', BYTEA E'\\x');
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('33333333-3333-3333-3333-333333333333', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Motel / mini-hotel / boarding house', TRUE, FALSE, NULL, NULL, 'Motel', BYTEA E'\\x');
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Condominium / Apartment unit', TRUE, FALSE, NULL, NULL, 'Apartment', BYTEA E'\\x');
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('b2c3d4e5-f6a7-8901-bcde-f12345678901', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Standalone villa with garden', TRUE, FALSE, NULL, NULL, 'Villa', BYTEA E'\\x');
INSERT INTO "PropertyTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('d4e5f6a7-b8c9-0123-def0-234567890123', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Land plot / undeveloped lot', TRUE, FALSE, NULL, NULL, 'Land', BYTEA E'\\x');

INSERT INTO "TransactionTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('44444444-4444-4444-4444-444444444444', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Property is listed for sale', TRUE, FALSE, NULL, NULL, 'Sell', BYTEA E'\\x');
INSERT INTO "TransactionTypes" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "Name", "RowVersion")
VALUES ('55555555-5555-5555-5555-555555555555', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Property is listed for rent', TRUE, FALSE, NULL, NULL, 'Rent', BYTEA E'\\x');

INSERT INTO "VIPPackages" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationDays", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "MaxListings", "Name", "RowVersion", "PriceAmount", "PriceCurrency")
VALUES ('60000000-0000-0000-0000-000000000001', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Đăng tối đa 3 tin rao trong 30 ngày. Phù hợp cho cá nhân.', 30, TRUE, FALSE, NULL, NULL, 3, 'Cơ Bản', BYTEA E'\\x', 500000.0, 'VND');
INSERT INTO "VIPPackages" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationDays", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "MaxListings", "Name", "RowVersion", "PriceAmount", "PriceCurrency")
VALUES ('60000000-0000-0000-0000-000000000002', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Đăng tối đa 10 tin rao trong 30 ngày. Phù hợp cho môi giới cá nhân.', 30, TRUE, FALSE, NULL, NULL, 10, 'Tiêu Chuẩn', BYTEA E'\\x', 1000000.0, 'VND');
INSERT INTO "VIPPackages" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationDays", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "MaxListings", "Name", "RowVersion", "PriceAmount", "PriceCurrency")
VALUES ('60000000-0000-0000-0000-000000000003', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Đăng tối đa 30 tin rao trong 90 ngày. Ưu tiên hiển thị trên trang chủ.', 90, TRUE, FALSE, NULL, NULL, 30, 'Cao Cấp', BYTEA E'\\x', 2500000.0, 'VND');
INSERT INTO "VIPPackages" ("Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationDays", "IsActive", "IsDeleted", "LastModifiedAt", "LastModifiedBy", "MaxListings", "Name", "RowVersion", "PriceAmount", "PriceCurrency")
VALUES ('60000000-0000-0000-0000-000000000004', TIMESTAMPTZ '2024-01-01T00:00:00Z', NULL, NULL, NULL, 'Không giới hạn tin rao trong 180 ngày. Được gắn nhãn VIP nổi bật.', 180, TRUE, FALSE, NULL, NULL, 99, 'Gold', BYTEA E'\\x', 5000000.0, 'VND');

CREATE UNIQUE INDEX "IX_Agents_Email" ON "Agents" ("Email");

CREATE INDEX "IX_Agents_UserId" ON "Agents" ("UserId");

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_AuditLogs_Action" ON "AuditLogs" ("Action");

CREATE INDEX "IX_AuditLogs_EntityName_EntityId" ON "AuditLogs" ("EntityName", "EntityId");

CREATE INDEX "IX_AuditLogs_Timestamp" ON "AuditLogs" ("Timestamp");

CREATE INDEX "IX_AuditLogs_UserId" ON "AuditLogs" ("UserId");

CREATE INDEX "IX_Conversations_BuyerId" ON "Conversations" ("BuyerId");

CREATE INDEX "IX_Conversations_LastMessageAt" ON "Conversations" ("LastMessageAt");

CREATE UNIQUE INDEX "IX_Conversations_PropertyId_BuyerId_SellerId" ON "Conversations" ("PropertyId", "BuyerId", "SellerId");

CREATE INDEX "IX_Conversations_SellerId" ON "Conversations" ("SellerId");

CREATE INDEX "IX_Customers_Email" ON "Customers" ("Email");

CREATE INDEX "IX_Customers_UserId" ON "Customers" ("UserId");

CREATE INDEX "IX_Favorites_PropertyId" ON "Favorites" ("PropertyId");

CREATE UNIQUE INDEX "IX_Favorites_UserId_PropertyId" ON "Favorites" ("UserId", "PropertyId");

CREATE INDEX "IX_Messages_ConversationId" ON "Messages" ("ConversationId");

CREATE INDEX "IX_Messages_SenderId" ON "Messages" ("SenderId");

CREATE INDEX "IX_Messages_Status" ON "Messages" ("Status");

CREATE INDEX "IX_Notifications_Type" ON "Notifications" ("Type");

CREATE INDEX "IX_Notifications_UserId" ON "Notifications" ("UserId");

CREATE INDEX "IX_Notifications_UserId_IsRead" ON "Notifications" ("UserId", "IsRead");

CREATE INDEX "IX_Payments_CreatedAt" ON "Payments" ("CreatedAt");

CREATE UNIQUE INDEX "IX_Payments_PaymentReference" ON "Payments" ("PaymentReference");

CREATE INDEX "IX_Payments_Status" ON "Payments" ("Status");

CREATE INDEX "IX_Payments_TransactionCode" ON "Payments" ("TransactionCode");

CREATE INDEX "IX_Payments_UserId" ON "Payments" ("UserId");

CREATE INDEX "IX_Payments_UserVIPPackageId" ON "Payments" ("UserVIPPackageId");

CREATE INDEX "IX_Payments_VIPPackageId" ON "Payments" ("VIPPackageId");

CREATE INDEX "IX_Properties_AgentId" ON "Properties" ("AgentId");

CREATE INDEX "IX_Properties_CreatedAt" ON "Properties" ("CreatedAt");

CREATE INDEX "IX_Properties_IsFeatured" ON "Properties" ("IsFeatured");

CREATE INDEX "IX_Properties_ListingType" ON "Properties" ("ListingType");

CREATE INDEX "IX_Properties_PropertyTypeId" ON "Properties" ("PropertyTypeId");

CREATE INDEX "IX_Properties_ProvinceCode" ON "Properties" ("ProvinceCode");

CREATE INDEX "IX_Properties_ProvinceCode_Status" ON "Properties" ("ProvinceCode", "Status");

CREATE INDEX "IX_Properties_PublishedAt" ON "Properties" ("PublishedAt");

CREATE UNIQUE INDEX "IX_Properties_Slug" ON "Properties" ("Slug");

CREATE INDEX "IX_Properties_Status" ON "Properties" ("Status");

CREATE INDEX "IX_Properties_TransactionTypeId" ON "Properties" ("TransactionTypeId");

CREATE INDEX "IX_Properties_WardCode" ON "Properties" ("WardCode");

CREATE INDEX "IX_PropertyImages_DisplayOrder" ON "PropertyImages" ("DisplayOrder");

CREATE INDEX "IX_PropertyImages_PropertyId_IsPrimary" ON "PropertyImages" ("PropertyId", "IsPrimary");

CREATE UNIQUE INDEX "IX_PropertyTypes_Name" ON "PropertyTypes" ("Name");

CREATE INDEX "IX_PropertyViews_PropertyId" ON "PropertyViews" ("PropertyId");

CREATE INDEX "IX_PropertyViews_UserId" ON "PropertyViews" ("UserId");

CREATE INDEX "IX_PropertyViews_ViewedAt" ON "PropertyViews" ("ViewedAt");

CREATE UNIQUE INDEX "IX_Provinces_Code" ON "Provinces" ("Code");

CREATE INDEX "IX_Provinces_Codename" ON "Provinces" ("Codename");

CREATE INDEX "IX_Provinces_IsActive" ON "Provinces" ("IsActive");

CREATE INDEX "IX_RefreshTokens_ExpiresAt" ON "RefreshTokens" ("ExpiresAt");

CREATE INDEX "IX_RefreshTokens_IsRevoked" ON "RefreshTokens" ("IsRevoked");

CREATE UNIQUE INDEX "IX_RefreshTokens_Token" ON "RefreshTokens" ("Token");

CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");

CREATE INDEX "IX_Transactions_AgentId" ON "Transactions" ("AgentId");

CREATE INDEX "IX_Transactions_CustomerId" ON "Transactions" ("CustomerId");

CREATE INDEX "IX_Transactions_PropertyId" ON "Transactions" ("PropertyId");

CREATE INDEX "IX_Transactions_Status" ON "Transactions" ("Status");

CREATE UNIQUE INDEX "IX_TransactionTypes_Name" ON "TransactionTypes" ("Name");

CREATE INDEX "IX_UserBlocks_BlockedUserId" ON "UserBlocks" ("BlockedUserId");

CREATE UNIQUE INDEX "IX_UserBlocks_BlockerId_BlockedUserId" ON "UserBlocks" ("BlockerId", "BlockedUserId") WHERE "IsDeleted" = false;

CREATE INDEX "IX_UserConversationSettings_ConversationId" ON "UserConversationSettings" ("ConversationId");

CREATE UNIQUE INDEX "IX_UserConversationSettings_UserId_ConversationId" ON "UserConversationSettings" ("UserId", "ConversationId") WHERE "IsDeleted" = false;

CREATE INDEX "IX_UserConversationSettings_UserId_IsPinned" ON "UserConversationSettings" ("UserId", "IsPinned");

CREATE INDEX "IX_UserVIPPackages_EndDate" ON "UserVIPPackages" ("EndDate");

CREATE INDEX "IX_UserVIPPackages_UserId" ON "UserVIPPackages" ("UserId");

CREATE INDEX "IX_UserVIPPackages_UserId_IsActive" ON "UserVIPPackages" ("UserId", "IsActive");

CREATE INDEX "IX_UserVIPPackages_VIPPackageId" ON "UserVIPPackages" ("VIPPackageId");

CREATE INDEX "IX_VIPPackages_IsActive" ON "VIPPackages" ("IsActive");

CREATE UNIQUE INDEX "IX_VIPPackages_Name" ON "VIPPackages" ("Name");

CREATE UNIQUE INDEX "IX_Wards_Code" ON "Wards" ("Code");

CREATE INDEX "IX_Wards_Codename" ON "Wards" ("Codename");

CREATE INDEX "IX_Wards_ProvinceCode" ON "Wards" ("ProvinceCode");

CREATE INDEX "IX_Wards_ProvinceId_IsActive" ON "Wards" ("ProvinceId", "IsActive");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260524210825_InitialPostgresIdentity', '9.0.16');

COMMIT;

