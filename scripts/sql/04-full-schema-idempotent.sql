IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Agents] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(300) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [LicenseNumber] nvarchar(100) NULL,
        [AgencyName] nvarchar(300) NULL,
        [AvatarUrl] nvarchar(2000) NULL,
        [Bio] nvarchar(3000) NULL,
        [IsActive] bit NOT NULL,
        [UserId] nvarchar(450) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Agents] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NULL,
        [Action] int NOT NULL,
        [EntityName] nvarchar(200) NOT NULL,
        [EntityId] nvarchar(450) NULL,
        [OldValues] nvarchar(max) NULL,
        [NewValues] nvarchar(max) NULL,
        [AffectedColumns] nvarchar(max) NULL,
        [IpAddress] nvarchar(45) NULL,
        [UserAgent] nvarchar(500) NULL,
        [Timestamp] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(300) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [PhoneNumber] nvarchar(20) NOT NULL,
        [CustomerType] int NOT NULL,
        [Notes] nvarchar(3000) NULL,
        [UserId] nvarchar(450) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Notifications] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Title] nvarchar(500) NOT NULL,
        [Content] nvarchar(2000) NOT NULL,
        [Type] int NOT NULL,
        [IsRead] bit NOT NULL,
        [ReadAt] datetime2 NULL,
        [ReferenceId] nvarchar(450) NULL,
        [ActionUrl] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [PropertyTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PropertyTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Provinces] (
        [Id] int NOT NULL IDENTITY,
        [Code] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Codename] nvarchar(200) NOT NULL,
        [DivisionType] nvarchar(100) NOT NULL,
        [PhoneCode] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Provinces] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Provinces_Code] UNIQUE ([Code])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL,
        [Token] nvarchar(500) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedByIp] nvarchar(45) NULL,
        [IsRevoked] bit NOT NULL,
        [RevokedAt] datetime2 NULL,
        [RevokedByIp] nvarchar(45) NULL,
        [RevokedReason] nvarchar(500) NULL,
        [ReplacedByToken] nvarchar(500) NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [TransactionTypes] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_TransactionTypes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [VIPPackages] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [DurationDays] int NOT NULL,
        [PriceAmount] decimal(18,2) NOT NULL,
        [PriceCurrency] nvarchar(10) NOT NULL,
        [MaxListings] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_VIPPackages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Wards] (
        [Id] int NOT NULL IDENTITY,
        [Code] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Codename] nvarchar(200) NOT NULL,
        [DivisionType] nvarchar(100) NOT NULL,
        [ProvinceCode] int NOT NULL,
        [ProvinceId] int NOT NULL,
        [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Wards] PRIMARY KEY ([Id]),
        CONSTRAINT [AK_Wards_Code] UNIQUE ([Code]),
        CONSTRAINT [FK_Wards_Provinces_ProvinceId] FOREIGN KEY ([ProvinceId]) REFERENCES [Provinces] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [UserVIPPackages] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [VIPPackageId] uniqueidentifier NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [RemainingListings] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_UserVIPPackages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserVIPPackages_VIPPackages_VIPPackageId] FOREIGN KEY ([VIPPackageId]) REFERENCES [VIPPackages] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Properties] (
        [Id] uniqueidentifier NOT NULL,
        [Title] nvarchar(500) NOT NULL,
        [Description] nvarchar(max) NULL,
        [PriceAmount] decimal(18,2) NOT NULL,
        [PriceCurrency] nvarchar(10) NOT NULL,
        [Area] decimal(18,4) NOT NULL,
        [NumberOfBedrooms] int NULL,
        [NumberOfBathrooms] int NULL,
        [NumberOfFloors] int NULL,
        [Status] int NOT NULL,
        [ListingType] int NOT NULL,
        [Direction] int NULL,
        [Street] nvarchar(500) NOT NULL,
        [AddressWard] nvarchar(200) NOT NULL,
        [AddressDistrict] nvarchar(200) NOT NULL,
        [AddressProvince] nvarchar(200) NOT NULL,
        [AddressCountry] nvarchar(100) NOT NULL,
        [AddressLatitude] float NULL,
        [AddressLongitude] float NULL,
        [ViewCount] int NOT NULL DEFAULT 0,
        [IsFeatured] bit NOT NULL DEFAULT CAST(0 AS bit),
        [PublishedAt] datetime2 NULL,
        [ExpiresAt] datetime2 NULL,
        [VideoUrl] nvarchar(max) NULL,
        [PropertyTypeId] uniqueidentifier NOT NULL,
        [AgentId] uniqueidentifier NOT NULL,
        [TransactionTypeId] uniqueidentifier NULL,
        [ProvinceCode] int NULL,
        [ProvinceName] nvarchar(200) NULL,
        [WardCode] int NULL,
        [WardName] nvarchar(200) NULL,
        [FullAddress] nvarchar(1000) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Properties] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Properties_Agents_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [Agents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Properties_PropertyTypes_PropertyTypeId] FOREIGN KEY ([PropertyTypeId]) REFERENCES [PropertyTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Properties_Provinces_ProvinceCode] FOREIGN KEY ([ProvinceCode]) REFERENCES [Provinces] ([Code]) ON DELETE SET NULL,
        CONSTRAINT [FK_Properties_TransactionTypes_TransactionTypeId] FOREIGN KEY ([TransactionTypeId]) REFERENCES [TransactionTypes] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Properties_Wards_WardCode] FOREIGN KEY ([WardCode]) REFERENCES [Wards] ([Code]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [UserVIPPackageId] uniqueidentifier NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(10) NOT NULL,
        [Status] int NOT NULL,
        [TransactionCode] nvarchar(500) NULL,
        [PaymentMethod] nvarchar(100) NULL,
        [GatewayResponse] nvarchar(4000) NULL,
        [PaidAt] datetime2 NULL,
        [Notes] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_UserVIPPackages_UserVIPPackageId] FOREIGN KEY ([UserVIPPackageId]) REFERENCES [UserVIPPackages] ([Id]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Conversations] (
        [Id] uniqueidentifier NOT NULL,
        [PropertyId] uniqueidentifier NOT NULL,
        [BuyerId] nvarchar(450) NOT NULL,
        [SellerId] nvarchar(450) NOT NULL,
        [IsClosed] bit NOT NULL,
        [ClosedAt] datetime2 NULL,
        [Subject] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Conversations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Conversations_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Favorites] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [PropertyId] uniqueidentifier NOT NULL,
        [Note] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Favorites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Favorites_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [PropertyImages] (
        [Id] uniqueidentifier NOT NULL,
        [PropertyId] uniqueidentifier NOT NULL,
        [Url] nvarchar(2000) NOT NULL,
        [Caption] nvarchar(500) NULL,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [IsPrimary] bit NOT NULL DEFAULT CAST(0 AS bit),
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_PropertyImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PropertyImages_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [PropertyViews] (
        [Id] uniqueidentifier NOT NULL,
        [PropertyId] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NULL,
        [IpAddress] nvarchar(45) NULL,
        [UserAgent] nvarchar(500) NULL,
        [SessionId] nvarchar(200) NULL,
        [ViewedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PropertyViews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PropertyViews_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Transactions] (
        [Id] uniqueidentifier NOT NULL,
        [PropertyId] uniqueidentifier NOT NULL,
        [CustomerId] uniqueidentifier NOT NULL,
        [AgentId] uniqueidentifier NOT NULL,
        [TransactionType] int NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Currency] nvarchar(10) NOT NULL,
        [CommissionAmount] decimal(18,2) NULL,
        [CommissionCurrency] nvarchar(10) NULL,
        [Status] int NOT NULL,
        [CompletedAt] datetime2 NULL,
        [Notes] nvarchar(3000) NULL,
        [ContractNumber] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Transactions_Agents_AgentId] FOREIGN KEY ([AgentId]) REFERENCES [Agents] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transactions_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Transactions_Properties_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [Properties] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE TABLE [Messages] (
        [Id] uniqueidentifier NOT NULL,
        [ConversationId] uniqueidentifier NOT NULL,
        [SenderId] nvarchar(450) NOT NULL,
        [Content] nvarchar(4000) NOT NULL,
        [Status] int NOT NULL,
        [ReadAt] datetime2 NULL,
        [AttachmentUrl] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [LastModifiedAt] datetime2 NULL,
        [LastModifiedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        [DeletedBy] nvarchar(max) NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_Conversations_ConversationId] FOREIGN KEY ([ConversationId]) REFERENCES [Conversations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[PropertyTypes]'))
        SET IDENTITY_INSERT [PropertyTypes] ON;
    EXEC(N'INSERT INTO [PropertyTypes] ([Id], [CreatedAt], [CreatedBy], [DeletedAt], [DeletedBy], [Description], [IsActive], [IsDeleted], [LastModifiedAt], [LastModifiedBy], [Name])
    VALUES (''11111111-1111-1111-1111-111111111111'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Detached or semi-detached house'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''House''),
    (''22222222-2222-2222-2222-222222222222'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Commercial office space'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Office''),
    (''33333333-3333-3333-3333-333333333333'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Motel / mini-hotel / boarding house'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Motel''),
    (''a1b2c3d4-e5f6-7890-abcd-ef1234567890'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Condominium / Apartment unit'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Apartment''),
    (''b2c3d4e5-f6a7-8901-bcde-f12345678901'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Standalone villa with garden'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Villa''),
    (''d4e5f6a7-b8c9-0123-def0-234567890123'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Land plot / undeveloped lot'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Land'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[PropertyTypes]'))
        SET IDENTITY_INSERT [PropertyTypes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[TransactionTypes]'))
        SET IDENTITY_INSERT [TransactionTypes] ON;
    EXEC(N'INSERT INTO [TransactionTypes] ([Id], [CreatedAt], [CreatedBy], [DeletedAt], [DeletedBy], [Description], [IsActive], [IsDeleted], [LastModifiedAt], [LastModifiedBy], [Name])
    VALUES (''44444444-4444-4444-4444-444444444444'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Property is listed for sale'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Sell''),
    (''55555555-5555-5555-5555-555555555555'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Property is listed for rent'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Rent'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'Name') AND [object_id] = OBJECT_ID(N'[TransactionTypes]'))
        SET IDENTITY_INSERT [TransactionTypes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Agents_Email] ON [Agents] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Agents_UserId] ON [Agents] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Action] ON [AuditLogs] ([Action]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityName_EntityId] ON [AuditLogs] ([EntityName], [EntityId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_Timestamp] ON [AuditLogs] ([Timestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Conversations_BuyerId] ON [Conversations] ([BuyerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Conversations_PropertyId_BuyerId_SellerId] ON [Conversations] ([PropertyId], [BuyerId], [SellerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Conversations_SellerId] ON [Conversations] ([SellerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Customers_UserId] ON [Customers] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Favorites_PropertyId] ON [Favorites] ([PropertyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Favorites_UserId_PropertyId] ON [Favorites] ([UserId], [PropertyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Messages_ConversationId] ON [Messages] ([ConversationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Messages_Status] ON [Messages] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Notifications_Type] ON [Notifications] ([Type]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Notifications_UserId_IsRead] ON [Notifications] ([UserId], [IsRead]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Payments_Status] ON [Payments] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Payments_TransactionCode] ON [Payments] ([TransactionCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Payments_UserVIPPackageId] ON [Payments] ([UserVIPPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_AgentId] ON [Properties] ([AgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_CreatedAt] ON [Properties] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_IsFeatured] ON [Properties] ([IsFeatured]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_ListingType] ON [Properties] ([ListingType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_PropertyTypeId] ON [Properties] ([PropertyTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_ProvinceCode] ON [Properties] ([ProvinceCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_ProvinceCode_Status] ON [Properties] ([ProvinceCode], [Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_PublishedAt] ON [Properties] ([PublishedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_Status] ON [Properties] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_TransactionTypeId] ON [Properties] ([TransactionTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Properties_WardCode] ON [Properties] ([WardCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_PropertyImages_DisplayOrder] ON [PropertyImages] ([DisplayOrder]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_PropertyImages_PropertyId_IsPrimary] ON [PropertyImages] ([PropertyId], [IsPrimary]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PropertyTypes_Name] ON [PropertyTypes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_PropertyViews_PropertyId] ON [PropertyViews] ([PropertyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_PropertyViews_UserId] ON [PropertyViews] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_PropertyViews_ViewedAt] ON [PropertyViews] ([ViewedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Provinces_Code] ON [Provinces] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Provinces_Codename] ON [Provinces] ([Codename]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Provinces_IsActive] ON [Provinces] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ExpiresAt] ON [RefreshTokens] ([ExpiresAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_IsRevoked] ON [RefreshTokens] ([IsRevoked]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Transactions_AgentId] ON [Transactions] ([AgentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Transactions_CustomerId] ON [Transactions] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Transactions_PropertyId] ON [Transactions] ([PropertyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Transactions_Status] ON [Transactions] ([Status]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TransactionTypes_Name] ON [TransactionTypes] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_UserVIPPackages_EndDate] ON [UserVIPPackages] ([EndDate]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_UserVIPPackages_UserId] ON [UserVIPPackages] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_UserVIPPackages_UserId_IsActive] ON [UserVIPPackages] ([UserId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_UserVIPPackages_VIPPackageId] ON [UserVIPPackages] ([VIPPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_VIPPackages_IsActive] ON [VIPPackages] ([IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_VIPPackages_Name] ON [VIPPackages] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Wards_Code] ON [Wards] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Wards_Codename] ON [Wards] ([Codename]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Wards_ProvinceCode] ON [Wards] ([ProvinceCode]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    CREATE INDEX [IX_Wards_ProvinceId_IsActive] ON [Wards] ([ProvinceId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519023815_AddVietnamAddressInfrastructure'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519023815_AddVietnamAddressInfrastructure', N'9.0.16');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [AvatarUrl] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [FirstName] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastLoginAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [LastName] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519030232_AddApplicationUserProfile'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519030232_AddApplicationUserProfile', N'9.0.16');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519031747_AddPropertySlugAndEnhancements'
)
BEGIN
    ALTER TABLE [Properties] ADD [Slug] nvarchar(600) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519031747_AddPropertySlugAndEnhancements'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Properties_Slug] ON [Properties] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519031747_AddPropertySlugAndEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519031747_AddPropertySlugAndEnhancements', N'9.0.16');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    ALTER TABLE [Conversations] ADD [BuyerUnreadCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    ALTER TABLE [Conversations] ADD [LastMessageAt] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    ALTER TABLE [Conversations] ADD [LastMessagePreview] nvarchar(150) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    ALTER TABLE [Conversations] ADD [SellerUnreadCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    CREATE INDEX [IX_Conversations_LastMessageAt] ON [Conversations] ([LastMessageAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519033012_AddRealtimeChatEnhancements'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519033012_AddRealtimeChatEnhancements', N'9.0.16');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'GatewayResponse');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [Payments] ALTER COLUMN [GatewayResponse] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [BankCode] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [IpAddress] nvarchar(45) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [OrderInfo] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [PaymentReference] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [VIPPackageId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    ALTER TABLE [Payments] ADD [VnpayTransactionId] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'DurationDays', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'MaxListings', N'Name', N'PriceAmount', N'PriceCurrency') AND [object_id] = OBJECT_ID(N'[VIPPackages]'))
        SET IDENTITY_INSERT [VIPPackages] ON;
    EXEC(N'INSERT INTO [VIPPackages] ([Id], [CreatedAt], [CreatedBy], [DeletedAt], [DeletedBy], [Description], [DurationDays], [IsActive], [IsDeleted], [LastModifiedAt], [LastModifiedBy], [MaxListings], [Name], [PriceAmount], [PriceCurrency])
    VALUES (''60000000-0000-0000-0000-000000000001'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Đăng tối đa 3 tin rao trong 30 ngày. Phù hợp cho cá nhân.'', 30, CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, 3, N''Cơ Bản'', 500000.0, N''VND''),
    (''60000000-0000-0000-0000-000000000002'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Đăng tối đa 10 tin rao trong 30 ngày. Phù hợp cho môi giới cá nhân.'', 30, CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, 10, N''Tiêu Chuẩn'', 1000000.0, N''VND''),
    (''60000000-0000-0000-0000-000000000003'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Đăng tối đa 30 tin rao trong 90 ngày. Ưu tiên hiển thị trên trang chủ.'', 90, CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, 30, N''Cao Cấp'', 2500000.0, N''VND''),
    (''60000000-0000-0000-0000-000000000004'', ''2024-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, N''Không giới hạn tin rao trong 180 ngày. Được gắn nhãn VIP nổi bật.'', 180, CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, 99, N''Gold'', 5000000.0, N''VND'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DeletedAt', N'DeletedBy', N'Description', N'DurationDays', N'IsActive', N'IsDeleted', N'LastModifiedAt', N'LastModifiedBy', N'MaxListings', N'Name', N'PriceAmount', N'PriceCurrency') AND [object_id] = OBJECT_ID(N'[VIPPackages]'))
        SET IDENTITY_INSERT [VIPPackages] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    CREATE INDEX [IX_Payments_CreatedAt] ON [Payments] ([CreatedAt]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Payments_PaymentReference] ON [Payments] ([PaymentReference]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    CREATE INDEX [IX_Payments_VIPPackageId] ON [Payments] ([VIPPackageId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519034021_AddVnPayPaymentFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519034021_AddVnPayPaymentFields', N'9.0.16');
END;

COMMIT;
GO

