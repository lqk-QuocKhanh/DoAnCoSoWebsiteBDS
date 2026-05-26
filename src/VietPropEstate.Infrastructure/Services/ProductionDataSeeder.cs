using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietPropEstate.Domain.Common;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.ValueObjects;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.Infrastructure.Persistence;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Production-safe initial data seeding. Runs after migrations when reference tables are empty.
/// Idempotent — skips tables that already contain data.
/// </summary>
public sealed class ProductionDataSeeder
{
    public const string SeedSlugPrefix = "prod-seed-";

    private static readonly Guid ApartmentTypeId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
    private static readonly Guid HouseTypeId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SellTransactionId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid RentTransactionId = new("55555555-5555-5555-5555-555555555555");
    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static readonly (int ProvinceCode, string ProvinceName, int WardCode, string WardName, double Lat, double Lng)[] Locations =
    [
        (79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức", 10.8025, 106.7408),
        (79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới", 10.7290, 106.7185),
        (79, "Thành phố Hồ Chí Minh", 26882, "Phường Bến Nghé", 10.7769, 106.7009),
        (1, "Thành phố Hà Nội", 4, "Phường Ba Đình", 21.0458, 105.8189),
        (1, "Thành phố Hà Nội", 8, "Phường Ngọc Hà", 21.0450, 105.9100),
        (48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân", 16.0543, 108.2432),
        (48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân", 16.0489, 108.2456),
        (31, "Thành phố Hải Phòng", 30341, "Phường Lê Chân", 20.8449, 106.6881),
        (92, "Thành phố Cần Thơ", 31108, "Phường Ninh Kiều", 10.0452, 105.7469),
        (56, "Tỉnh Khánh Hòa", 22363, "Phường Vĩnh Hòa", 12.2388, 109.1967)
    ];

    private static readonly string[][] ImageSets =
    [
        [
            "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&q=80",
            "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80",
            "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&q=80"
        ],
        [
            "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800&q=80",
            "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800&q=80",
            "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800&q=80"
        ],
        [
            "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800&q=80",
            "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?w=800&q=80",
            "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800&q=80"
        ],
        [
            "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800&q=80",
            "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=800&q=80",
            "https://images.unsplash.com/photo-1560185127-6ed189bf02f4?w=800&q=80"
        ]
    ];

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<ProductionDataSeeder> _logger;

    public ProductionDataSeeder(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<ProductionDataSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var enabled = _configuration.GetValue(
            "ProductionSeed:Enabled",
            _environment.IsProduction());

        if (!enabled)
        {
            _logger.LogDebug("ProductionSeed disabled — skipping.");
            return;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var status = await GetDatabaseStatusAsync(db, userManager, cancellationToken);
        _logger.LogInformation(
            "Production seed check — Properties: {Properties}, Categories: {Categories}, Provinces: {Provinces}, Wards: {Wards}, Users: {Users}",
            status.PropertiesCount,
            status.CategoriesCount,
            status.ProvincesCount,
            status.WardsCount,
            status.UsersCount);

        await SeedReferenceDataIfEmptyAsync(db, cancellationToken);

        if (status.PropertiesCount > 0)
        {
            _logger.LogInformation("Properties table is not empty — skipping production property seed.");
            return;
        }

        if (status.UsersCount == 0)
        {
            _logger.LogWarning("Users table is empty — admin must be seeded by RoleSeeder before property seed.");
            return;
        }

        var adminEmail = _configuration["AdminSeed:Email"] ?? "admin@vietpropestate.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            _logger.LogWarning("Admin user {Email} not found — skipping production property seed.", adminEmail);
            return;
        }

        var agent = await EnsureAgentAsync(db, admin, "VietPropEstate Official", cancellationToken);
        var properties = BuildProductionProperties(agent.Id);

        await db.Properties.AddRangeAsync(properties, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Production seed completed: {Apartments} apartments, {Houses} houses, {Rentals} rentals.",
            20, 20, 20);
    }

    private async Task<DatabaseSeedStatus> GetDatabaseStatusAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        return new DatabaseSeedStatus
        {
            PropertiesCount = await db.Properties.CountAsync(cancellationToken),
            CategoriesCount = await db.PropertyTypes.CountAsync(cancellationToken),
            ProvincesCount = await db.Provinces.CountAsync(cancellationToken),
            WardsCount = await db.Wards.CountAsync(cancellationToken),
            UsersCount = await userManager.Users.CountAsync(cancellationToken)
        };
    }

    private async Task SeedReferenceDataIfEmptyAsync(
        ApplicationDbContext db,
        CancellationToken cancellationToken)
    {
        if (!await db.PropertyTypes.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("PropertyTypes (categories) empty — inserting reference categories.");
            await db.PropertyTypes.AddRangeAsync(
            [
                CreatePropertyType(ApartmentTypeId, "Apartment", "Condominium / Apartment unit"),
                CreatePropertyType(HouseTypeId, "House", "Detached or semi-detached house"),
                CreatePropertyType(new Guid("B2C3D4E5-F6A7-8901-BCDE-F12345678901"), "Villa", "Standalone villa with garden"),
                CreatePropertyType(new Guid("D4E5F6A7-B8C9-0123-DEF0-234567890123"), "Land", "Land plot / undeveloped lot"),
                CreatePropertyType(new Guid("22222222-2222-2222-2222-222222222222"), "Office", "Commercial office space"),
                CreatePropertyType(new Guid("33333333-3333-3333-3333-333333333333"), "Motel", "Motel / mini-hotel / boarding house")
            ], cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        if (!await db.TransactionTypes.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("TransactionTypes empty — inserting Sell/Rent.");
            await db.TransactionTypes.AddRangeAsync(
            [
                CreateTransactionType(SellTransactionId, "Sell", "Property is listed for sale"),
                CreateTransactionType(RentTransactionId, "Rent", "Property is listed for rent")
            ], cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private List<Property> BuildProductionProperties(Guid agentId)
    {
        var items = new List<Property>(60);

        for (var i = 1; i <= 20; i++)
        {
            var loc = Locations[(i - 1) % Locations.Length];
            var bedrooms = 1 + (i % 4);
            var property = CreateListing(
                $"Căn hộ cao cấp {bedrooms}PN — {loc.ProvinceName} #{i}",
                $"Căn hộ {bedrooms} phòng ngủ, nội thất hiện đại, view đẹp, an ninh 24/7, gần tiện ích tại {loc.WardName}.",
                2_000_000_000m + (i * 180_000_000m),
                55m + (i * 2.5m),
                ListingType.ForSale,
                ApartmentTypeId,
                SellTransactionId,
                agentId,
                loc,
                $"{120 + i} Nguyễn Văn Linh",
                bedrooms,
                Math.Max(1, bedrooms - 1),
                15 + (i % 20),
                PropertyDirection.SouthEast,
                featured: i % 5 == 0,
                views: 20 + i);
            AttachImages(property, ImageSets[i % ImageSets.Length]);
            items.Add(property);
        }

        for (var i = 1; i <= 20; i++)
        {
            var loc = Locations[(i + 3) % Locations.Length];
            var floors = 2 + (i % 4);
            var property = CreateListing(
                $"Nhà phố {floors} tầng mặt tiền — {loc.WardName} #{i}",
                $"Nhà phố {floors} tầng, sổ hồng chính chủ, phù hợp kinh doanh hoặc ở tại {loc.ProvinceName}.",
                5_000_000_000m + (i * 250_000_000m),
                80m + (i * 3m),
                ListingType.ForSale,
                HouseTypeId,
                SellTransactionId,
                agentId,
                loc,
                $"{45 + i} Lê Lợi",
                3 + (i % 3),
                2 + (i % 2),
                floors,
                PropertyDirection.East,
                featured: i % 6 == 0,
                views: 30 + i);
            AttachImages(property, ImageSets[(i + 1) % ImageSets.Length]);
            items.Add(property);
        }

        for (var i = 1; i <= 20; i++)
        {
            var loc = Locations[(i + 6) % Locations.Length];
            var isApartment = i % 2 == 0;
            var property = CreateListing(
                isApartment
                    ? $"Căn hộ cho thuê {loc.WardName} #{i}"
                    : $"Nhà nguyên căn cho thuê {loc.ProvinceName} #{i}",
                isApartment
                    ? "Căn hộ full nội thất, dọn vào ở ngay, hợp đồng linh hoạt 6–12 tháng."
                    : "Nhà nguyên căn rộng rãi, sân riêng, phù hợp gia đình, thanh toán theo tháng.",
                8_000_000m + (i * 500_000m),
                isApartment ? 45m + i : 90m + (i * 2m),
                ListingType.ForRent,
                isApartment ? ApartmentTypeId : HouseTypeId,
                RentTransactionId,
                agentId,
                loc,
                $"{200 + i} Trần Phú",
                isApartment ? 1 + (i % 3) : 3 + (i % 2),
                isApartment ? 1 : 2 + (i % 2),
                isApartment ? 12 + (i % 10) : 2 + (i % 3),
                PropertyDirection.North,
                featured: i % 7 == 0,
                views: 10 + i);
            AttachImages(property, ImageSets[(i + 2) % ImageSets.Length]);
            items.Add(property);
        }

        return items;
    }

    private static Property CreateListing(
        string title,
        string description,
        decimal price,
        decimal area,
        ListingType listingType,
        Guid propertyTypeId,
        Guid transactionTypeId,
        Guid agentId,
        (int ProvinceCode, string ProvinceName, int WardCode, string WardName, double Lat, double Lng) location,
        string street,
        int? bedrooms,
        int? bathrooms,
        int? floors,
        PropertyDirection? direction,
        bool featured = false,
        int views = 0)
    {
        var address = new Address(
            street,
            location.WardName,
            "",
            location.ProvinceName,
            latitude: location.Lat,
            longitude: location.Lng);

        var property = Property.Create(
            title,
            description,
            new Money(price, "VND"),
            area,
            listingType,
            address,
            propertyTypeId,
            agentId,
            bedrooms,
            bathrooms,
            floors,
            direction,
            transactionTypeId,
            location.ProvinceCode,
            location.ProvinceName,
            location.WardCode,
            location.WardName,
            location.Lat,
            location.Lng);

        Activate(property);

        if (featured && property.Status == PropertyStatus.Active)
            property.SetFeatured(true);

        for (var i = 0; i < views; i++)
            property.IncrementViewCount();

        return property;
    }

    private static void Activate(Property property)
    {
        if (property.Status == PropertyStatus.Draft)
            property.SubmitForApproval();
        if (property.Status == PropertyStatus.PendingApproval)
            property.Publish();
    }

    private static void AttachImages(Property property, string[] urls)
    {
        var captions = new[] { "Ảnh chính", "Không gian", "View" };
        for (var i = 0; i < urls.Length; i++)
        {
            property.AddImage(PropertyImage.Create(
                property.Id,
                urls[i],
                captions[Math.Min(i, captions.Length - 1)],
                i,
                i == 0));
        }
    }

    private static PropertyType CreatePropertyType(Guid id, string name, string? description)
    {
        var entity = PropertyType.Create(name, description);
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!
            .SetValue(entity, id);
        entity.CreatedAt = SeedDate;
        return entity;
    }

    private static TransactionType CreateTransactionType(Guid id, string name, string? description)
    {
        var entity = TransactionType.Create(name, description);
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!
            .SetValue(entity, id);
        entity.CreatedAt = SeedDate;
        return entity;
    }

    private static async Task<Agent> EnsureAgentAsync(
        ApplicationDbContext db,
        ApplicationUser user,
        string agencyName,
        CancellationToken cancellationToken)
    {
        var existing = await db.Agents
            .FirstOrDefaultAsync(a => a.UserId == user.Id && !a.IsDeleted, cancellationToken);

        if (existing is not null)
            return existing;

        var agent = Agent.Create(
            user.FullName,
            user.Email ?? user.UserName ?? "admin@vietpropestate.com",
            user.PhoneNumber ?? "0901234567",
            licenseNumber: $"ADM-{user.Id[..8].ToUpperInvariant()}",
            agencyName: agencyName,
            userId: user.Id);

        await db.Agents.AddAsync(agent, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return agent;
    }

    private sealed class DatabaseSeedStatus
    {
        public int PropertiesCount { get; init; }
        public int CategoriesCount { get; init; }
        public int ProvincesCount { get; init; }
        public int WardsCount { get; init; }
        public int UsersCount { get; init; }
    }
}
