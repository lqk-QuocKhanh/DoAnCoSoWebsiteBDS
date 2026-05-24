using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.ValueObjects;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.Infrastructure.Persistence;

namespace VietPropEstate.Infrastructure.Services;

/// <summary>
/// Seeds deterministic demo data for manual / integration testing.
/// Idempotent — skips when properties with slug prefix "test-" already exist.
/// </summary>
public sealed class TestDataSeeder
{
    private const string SeedMarkerTitle = "Căn hộ 2PN cao cấp Vinhomes Central Park";
    private const string RegionalSeedMarkerTitle = "Penthouse Landmark 81 view toàn cảnh Sài Gòn";
    private const string BrokerTeamSeedMarkerTitle = "Căn hộ Masteri Thảo Điền view sông";
    private const string DefaultPassword = "Test@123";

    // Stable property type / transaction IDs (match EF migrations)
    private static readonly Guid ApartmentTypeId = new("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
    private static readonly Guid HouseTypeId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid VillaTypeId = new("B2C3D4E5-F6A7-8901-BCDE-F12345678901");
    private static readonly Guid LandTypeId = new("D4E5F6A7-B8C9-0123-DEF0-234567890123");
    private static readonly Guid OfficeTypeId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid MotelTypeId = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid SellTransactionId = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid RentTransactionId = new("55555555-5555-5555-5555-555555555555");

    private static readonly string[][] CoreListingImages =
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
            "https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&q=80",
            "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&q=80",
            "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800&q=80"
        ],
        [
            "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800&q=80",
            "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=800&q=80",
            "https://images.unsplash.com/photo-1560185127-6ed189bf02f4?w=800&q=80"
        ]
    ];

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestDataSeeder> _logger;

    public TestDataSeeder(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<TestDataSeeder> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_configuration.GetValue("TestSeed:Enabled", false))
        {
            _logger.LogDebug("TestSeed disabled — skipping demo data.");
            return;
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var password = _configuration["TestSeed:DefaultPassword"] ?? DefaultPassword;

        var broker = await EnsureUserAsync(userManager, "broker@test.vietpropestate.vn", "Môi", "Giới", "Broker", password);
        var customer = await EnsureUserAsync(userManager, "customer@test.vietpropestate.vn", "Khách", "Hàng", "Customer", password);
        await EnsureUserAsync(userManager, "staff@test.vietpropestate.vn", "Nhân", "Viên", "Staff", password);

        var admin = await userManager.FindByEmailAsync(_configuration["AdminSeed:Email"] ?? "admin@vietpropestate.vn");
        var brokerAgent = await EnsureAgentAsync(db, broker, "Công ty BĐS VietProp Broker", cancellationToken);
        var adminAgent = admin is not null
            ? await EnsureAgentAsync(db, admin, "VietPropEstate Admin", cancellationToken)
            : brokerAgent;

        if (!await db.Properties.AnyAsync(p => p.Title == SeedMarkerTitle, cancellationToken))
        {
            var properties = BuildProperties(brokerAgent, adminAgent);
            await db.Properties.AddRangeAsync(properties, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            if (customer is not null && properties.Count > 0)
            {
                var favorites = new[]
                {
                    Favorite.Create(customer.Id, properties[0].Id, "Quan tâm căn hộ Q1"),
                    Favorite.Create(customer.Id, properties[1].Id, "Để so sánh giá")
                };
                await db.Set<Favorite>().AddRangeAsync(favorites, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInformation(
                "Core test data seeded: {PropertyCount} properties (password: {Password}).",
                properties.Count, password);
        }
        else
        {
            _logger.LogInformation("Core test data already present ('{Title}'). Skipping.", SeedMarkerTitle);
        }

        if (!await db.Properties.AnyAsync(p => p.Title == RegionalSeedMarkerTitle, cancellationToken))
        {
            var regional = BuildRegionalProperties(brokerAgent, adminAgent);
            await db.Properties.AddRangeAsync(regional, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Regional demo listings seeded: {PropertyCount} properties across Vietnam.",
                regional.Count);
        }
        else
        {
            _logger.LogInformation("Regional demo data already present ('{Title}'). Skipping.", RegionalSeedMarkerTitle);
        }

        await SeedBrokerTeamAsync(db, userManager, password, cancellationToken);
    }

    private async Task SeedBrokerTeamAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        string password,
        CancellationToken cancellationToken)
    {
        if (await db.Properties.AnyAsync(p => p.Title == BrokerTeamSeedMarkerTitle, cancellationToken))
        {
            _logger.LogInformation("Broker team demo data already present ('{Title}'). Skipping.", BrokerTeamSeedMarkerTitle);
            return;
        }

        var brokerMinh = await EnsureUserAsync(userManager, "broker.minh@vietpropestate.vn",
            "Nguyễn Văn", "Minh", "Broker", password, "0911000001");
        var brokerHoa = await EnsureUserAsync(userManager, "broker.hoa@vietpropestate.vn",
            "Trần Thị", "Hoa", "Broker", password, "0911000002");
        var brokerBao = await EnsureUserAsync(userManager, "broker.bao@vietpropestate.vn",
            "Lê Quốc", "Bảo", "Broker", password, "0911000003");

        var agentMinh = await EnsureAgentAsync(db, brokerMinh, "Saigon Home Realty", cancellationToken);
        var agentHoa = await EnsureAgentAsync(db, brokerHoa, "Hà Nội Land Pro", cancellationToken);
        var agentBao = await EnsureAgentAsync(db, brokerBao, "Đà Nẵng Coastal Realty", cancellationToken);

        var listings = BuildBrokerTeamListings(agentMinh, agentHoa, agentBao);
        await db.Properties.AddRangeAsync(listings, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Broker team seeded: 3 brokers, {ListingCount} listings (password: {Password}).",
            listings.Count, password);
    }

    private static List<Property> BuildBrokerTeamListings(Agent agentMinh, Agent agentHoa, Agent agentBao)
    {
        var items = new List<Property>();

        // Nguyễn Văn Minh — TP.HCM
        items.Add(CreateListing(
            BrokerTeamSeedMarkerTitle,
            "Căn hộ 2PN view sông Saigon, nội thất đầy đủ, tiện ích hồ bơi gym, gần Metro.",
            5_200_000_000m, 78m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            agentMinh.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "159 Xa lộ Hà Nội", 2, 2, 22, PropertyDirection.SouthEast,
            10.8025, 106.7408, featured: true, views: 145,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Nhà phố Phú Mỹ Hưng Quận 7",
            "Nhà phố 3 tầng, sân thượng, khu dân cư an ninh, sổ hồng chính chủ.",
            9_800_000_000m, 105m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            agentMinh.Id, 79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới",
            "12 Đường Nguyễn Lương Bằng", 4, 3, 3, PropertyDirection.East,
            10.7290, 106.7185, views: 88,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Căn hộ cho thuê Sala Đại Quang Minh",
            "Căn hộ 1PN full nội thất, view công viên, phù hợp expat.",
            16_000_000m, 52m, ListingType.ForRent, ApartmentTypeId, RentTransactionId,
            agentMinh.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "10 Mai Chí Thọ", 1, 1, 18, PropertyDirection.South,
            10.7712, 106.7223, views: 62,
            afterCreate: ActivateForDemo));

        // Trần Thị Hoa — Hà Nội
        items.Add(CreateListing(
            "Chung cư Golden Westlake Hà Nội",
            "Căn hộ 3PN view hồ Tây, nội thất cao cấp, bàn giao ngay.",
            7_500_000_000m, 98m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            agentHoa.Id, 1, "Thành phố Hà Nội", 4, "Phường Ba Đình",
            "162 Thụy Khuê", 3, 2, 25, PropertyDirection.West,
            21.0458, 105.8189, featured: true, views: 112,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Biệt thự Vinhomes Riverside Long Biên",
            "Biệt thự song lập, sân vườn 200m2, khu compound cao cấp.",
            18_500_000_000m, 280m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            agentHoa.Id, 1, "Thành phố Hà Nội", 8, "Phường Ngọc Hà",
            "Lô BT12 Vinhomes Riverside", 5, 4, 3, PropertyDirection.NorthEast,
            21.0450, 105.9100, views: 76,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Văn phòng cho thuê tòa Keangnam Hà Nội",
            "Văn phòng 85m2, view panorama, có sẵn điều hòa và trần thạch cao.",
            28_000_000m, 85m, ListingType.ForRent, OfficeTypeId, RentTransactionId,
            agentHoa.Id, 1, "Thành phố Hà Nội", 4, "Phường Ba Đình",
            "Tầng 18 Keangnam", null, 2, 18, PropertyDirection.North,
            21.0134, 105.7834, views: 41,
            afterCreate: ActivateForDemo));

        // Lê Quốc Bảo — Đà Nẵng
        items.Add(CreateListing(
            "Căn hộ Mizuki Park Đà Nẵng",
            "Căn hộ 2PN view công viên, tiện ích đầy đủ, gần biển Mỹ Khê.",
            3_600_000_000m, 70m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            agentBao.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "Lô A2 Mizuki Park", 2, 2, 20, PropertyDirection.East,
            16.0543, 108.2432, featured: true, views: 93,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Shophouse An Thượng gần biển",
            "Shophouse 4 tầng, kinh doanh cafe homestay, dòng khách du lịch ổn định.",
            6_200_000_000m, 88m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            agentBao.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "45 An Thượng 29", 3, 3, 4, PropertyDirection.South,
            16.0489, 108.2456, views: 58,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Biệt thự nghỉ dưỡng Hội An view sông",
            "Biệt thự 4PN, hồ bơi riêng, cách phố cổ 10 phút lái xe.",
            14_000_000_000m, 320m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            agentBao.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "Khu villa Cam Thanh", 4, 4, 2, PropertyDirection.NorthWest,
            15.9050, 108.3350, views: 67,
            afterCreate: ActivateForDemo));

        for (var i = 0; i < items.Count; i++)
            AttachImages(items[i], CoreListingImages[i % CoreListingImages.Length]);

        return items;
    }

    private static List<Property> BuildProperties(Agent brokerAgent, Agent adminAgent)
    {
        var items = new List<Property>();

        items.Add(CreateListing(
            "Căn hộ 2PN cao cấp Vinhomes Central Park",
            "Căn hộ view sông, nội thất cao cấp, tiện ích hồ bơi gym.",
            4_500_000_000m, 72m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "208 Nguyễn Hữu Cảnh", 2, 2, 25, PropertyDirection.SouthEast,
            10.7952, 106.7215, featured: true, views: 320,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Nhà phố 4 tầng mặt tiền 6m Thủ Đức",
            "Nhà phố kinh doanh, sổ hồng chính chủ, gần Metro.",
            8_200_000_000m, 120m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới",
            "45 Đường 30", 4, 3, 4, PropertyDirection.East,
            10.8311, 106.7337, featured: true, views: 185,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Biệt thự view biển Non Nước Đà Nẵng",
            "Biệt thự nghỉ dưỡng, hoàn thiện cao cấp, cách biển 200m.",
            12_500_000_000m, 350m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            brokerAgent.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "Lô B2 Khu biệt thự", 5, 5, 3, PropertyDirection.NorthEast,
            16.0392, 108.2510, views: 97,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Văn phòng hạng A tòa Landmark Hà Nội",
            "Văn phòng view hồ, thiết kế mở, phù hợp startup/công ty.",
            45_000_000m, 95m, ListingType.ForRent, OfficeTypeId, RentTransactionId,
            adminAgent.Id, 1, "Thành phố Hà Nội", 4, "Phường Ba Đình",
            "12 Liễu Giai", null, 2, 20, PropertyDirection.West,
            21.0340, 105.8145, views: 64,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Phòng trọ full nội thất gần ĐHQG TP.HCM",
            "Phòng 25m2, có máy lạnh, giường tủ, wifi tốc độ cao.",
            4_500_000m, 25m, ListingType.ForRent, MotelTypeId, RentTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "Khu phố 6", 1, 1, 1, null,
            10.8700, 106.8030, views: 210,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Đất nền 100m2 Bình Dương sổ riêng",
            "Đất thổ cư, mặt tiền 5m, cách QL13 500m.",
            2_800_000_000m, 100m, ListingType.ForSale, LandTypeId, SellTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 25760, "Phường Bình Dương",
            "Đường DT743", null, null, null, PropertyDirection.South,
            10.9800, 106.6500, views: 45,
            afterCreate: ActivateForDemo));

        items.Add(CreateListing(
            "Căn hộ Studio chờ duyệt (Draft)",
            "Tin nháp để test luồng duyệt admin.",
            2_100_000_000m, 48m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới",
            "88 Đường số 2", 1, 1, 15, PropertyDirection.North,
            10.8200, 106.7400, views: 0));

        items.Add(CreateListing(
            "Nhà phố Hà Nội bản nháp",
            "Tin draft thứ hai cho dashboard.",
            6_500_000_000m, 80m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 1, "Thành phố Hà Nội", 8, "Phường Ngọc Hà",
            "15 Ngọc Hà", 3, 2, 4, PropertyDirection.South,
            21.0400, 105.8300, views: 0));

        items.Add(CreateListing(
            "Căn hộ cho thuê đã ẩn",
            "Tin đã withdraw — test filter dashboard.",
            18_000_000m, 65m, ListingType.ForRent, ApartmentTypeId, RentTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "99 Võ Văn Ngân", 2, 1, 18, PropertyDirection.West,
            10.8500, 106.7700, views: 55,
            afterCreate: p => { ActivateForDemo(p); p.Withdraw(); }));

        items.Add(CreateListing(
            "Căn hộ Quận 1 đã bán",
            "Tin sold — test trạng thái đã bán.",
            5_800_000_000m, 68m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            adminAgent.Id, 79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới",
            "55 Nguyễn Huệ", 2, 2, 20, PropertyDirection.East,
            10.7730, 106.7040, views: 890,
            afterCreate: p => { ActivateForDemo(p); p.MarkAsSold(); }));

        items.Add(CreateListing(
            "Căn hộ dịch vụ đã cho thuê",
            "Tin rented — test trạng thái cho thuê.",
            12_000_000m, 55m, ListingType.ForRent, ApartmentTypeId, RentTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 26824, "Phường Thủ Đức",
            "12 Đinh Bộ Lĩnh", 1, 1, 12, null,
            10.8100, 106.7100, views: 120,
            afterCreate: p => { ActivateForDemo(p); p.MarkAsRented(); }));

        items.Add(CreateListing(
            "Shophouse đang thương lượng",
            "Tin under offer — test trạng thái đang deal.",
            9_500_000_000m, 90m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 79, "Thành phố Hồ Chí Minh", 25760, "Phường Bình Dương",
            "20 Đại lộ Bình Dương", 3, 2, 3, PropertyDirection.NorthWest,
            10.9600, 106.7100, views: 75,
            afterCreate: p => { ActivateForDemo(p); p.PlaceUnderOffer(); }));

        for (var i = 0; i < items.Count; i++)
            AttachImages(items[i], CoreListingImages[i % CoreListingImages.Length]);

        return items;
    }

    private static List<Property> BuildRegionalProperties(Agent brokerAgent, Agent adminAgent)
    {
        var items = new List<Property>();

        items.Add(CreateListing(
            "Penthouse Landmark 81 view toàn cảnh Sài Gòn",
            "Penthouse 3PN tầng cao, trần cao 4m, nội thất Italian, view 360 độ sông Sài Gòn.",
            28_000_000_000m, 145m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            adminAgent.Id, 79, "Thành phố Hồ Chí Minh", 26911, "Phường Bình Quới",
            "720A Điện Biên Phủ", 3, 3, 48, PropertyDirection.South,
            10.7718, 106.7042, featured: true, views: 520,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1512917774080-9991a1b4b750?w=800&q=80",
            "https://images.unsplash.com/photo-1600607687644-c7171b42498f?w=800&q=80",
            "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?w=800&q=80");

        items.Add(CreateListing(
            "Căn hộ duplex view Hồ Tây West Lake",
            "Duplex 2 tầng, ban công rộng hướng hồ, gần phố cổ Trúc Bạch.",
            12_800_000_000m, 98m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            brokerAgent.Id, 1, "Thành phố Hà Nội", 4, "Phường Ba Đình",
            "25 Quảng An", 3, 2, 22, PropertyDirection.West,
            21.0580, 105.8230, featured: true, views: 280,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1605276374102-de8778310a17?w=800&q=80",
            "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=800&q=80",
            "https://images.unsplash.com/photo-1618221195710-dd6b41faeca6?w=800&q=80");

        items.Add(CreateListing(
            "Nhà mặt phố Hải Phòng gần cảng Lê Chân",
            "Nhà 4 tầng mặt tiền 5m, phù hợp kinh doanh, sổ đỏ chính chủ.",
            9_500_000_000m, 110m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 31, "Thành phố Hải Phòng", 10507, "Phường Thành Đông",
            "88 Lê Chân", 4, 3, 4, PropertyDirection.East,
            20.8449, 106.6881, views: 145,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800&q=80",
            "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&q=80",
            "https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=800&q=80");

        items.Add(CreateListing(
            "Biệt thự view Vịnh Hạ Long Quảng Ninh",
            "Biệt thự 3 tầng view biển, sân vườn 200m2, cách bãi tắm Bãi Cháy 5 phút.",
            18_500_000_000m, 320m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            adminAgent.Id, 22, "Tỉnh Quảng Ninh", 6652, "Phường Hà Tu",
            "Khu biệt thự Bãi Cháy", 5, 4, 3, PropertyDirection.NorthEast,
            20.9550, 107.0420, featured: true, views: 190,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?w=800&q=80",
            "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800&q=80",
            "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?w=800&q=80");

        items.Add(CreateListing(
            "Nhà vườn vùng cao Cao Bằng",
            "Nhà gỗ 2 tầng trong không gian xanh, không khí trong lành, phù hợp nghỉ dưỡng.",
            2_200_000_000m, 180m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 4, "Tỉnh Cao Bằng", 1273, "Phường Thục Phán",
            "Thôn Pác Bó", 3, 2, 2, PropertyDirection.South,
            22.6650, 106.2570, views: 68,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=800&q=80",
            "https://images.unsplash.com/photo-1518780664697-55e3ad933bea?w=800&q=80",
            "https://images.unsplash.com/photo-1605146769289-440113cc3d00?w=800&q=80");

        items.Add(CreateListing(
            "Nhà phố phố cổ Huế bên sông Hương",
            "Nhà 3 tầng kiến trúc Huế, cách cầu Tràng Tiền 800m, view sông.",
            7_800_000_000m, 95m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 46, "Thành phố Huế", 19753, "Phường Phú Xuân",
            "12 Nguyễn Sinh Cung", 3, 2, 3, PropertyDirection.East,
            16.4637, 107.5909, views: 112,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800&q=80",
            "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800&q=80",
            "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?w=800&q=80");

        items.Add(CreateListing(
            "Căn hộ biển Mỹ Khê Đà Nẵng",
            "Căn hộ 2PN hướng biển, full nội thất, tiện ích resort 5 sao.",
            6_500_000_000m, 78m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            brokerAgent.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "36 Võ Nguyên Giáp", 2, 2, 28, PropertyDirection.East,
            16.0678, 108.2450, featured: true, views: 240,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1523217582562-09d0def993a6?w=800&q=80",
            "https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=800&q=80",
            "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&q=80");

        items.Add(CreateListing(
            "Villa nghỉ dưỡng Nha Trang Khánh Hòa",
            "Villa 4PN có hồ bơi riêng, cách biển Trần Phú 300m, cho thuê ngắn hạn được.",
            15_000_000_000m, 280m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            adminAgent.Id, 56, "Tỉnh Khánh Hòa", 22333, "Phường Bắc Nha Trang",
            "Lô A Vinpearl", 4, 4, 2, PropertyDirection.SouthEast,
            12.2431, 109.1943, featured: true, views: 310,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1613977257363-707ba9348227?w=800&q=80",
            "https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=800&q=80",
            "https://images.unsplash.com/photo-1600566753086-00f18fb576b9?w=800&q=80");

        items.Add(CreateListing(
            "Biệt thự Đà Lạt view rừng thông",
            "Biệt thự phong cách chalet, sân BBQ, gần Hồ Xuân Hương 10 phút lái xe.",
            11_200_000_000m, 260m, ListingType.ForSale, VillaTypeId, SellTransactionId,
            brokerAgent.Id, 68, "Tỉnh Lâm Đồng", 24787, "Phường Cam Ly - Đà Lạt",
            "Đường Trần Hưng Đạo", 4, 3, 2, PropertyDirection.North,
            11.9404, 108.4583, views: 175,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1518780664697-55e3ad933bea?w=800&q=80",
            "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=800&q=80",
            "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800&q=80");

        items.Add(CreateListing(
            "Shophouse khu Biên Hòa Đồng Nai",
            "Shophouse 1 trệt 2 lầu, khu dân cư đông, gần KCN Amata.",
            5_500_000_000m, 85m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 75, "Tỉnh Đồng Nai", 25195, "Phường Bình Phước",
            "45 Phạm Văn Thuận", 3, 2, 3, PropertyDirection.SouthWest,
            10.9447, 106.8243, views: 98,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800&q=80",
            "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=800&q=80",
            "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800&q=80");

        items.Add(CreateListing(
            "Nhà phố ven sông Hậu Cần Thơ",
            "Nhà 3 tầng mặt tiền sông, thoáng mát, gần chợ nổi Cái Răng.",
            4_200_000_000m, 100m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 92, "Thành phố Cần Thơ", 31120, "Phường Cái Khế",
            "18 Đại lộ Hòa Bình", 3, 2, 3, PropertyDirection.West,
            10.0452, 105.7469, views: 130,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&q=80",
            "https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=800&q=80",
            "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?w=800&q=80");

        items.Add(CreateListing(
            "Nhà vườn miệt vườn An Giang",
            "Nhà vườn 5000m2, cây ăn trái trĩu quả, đường xe tải vào tận nơi.",
            3_500_000_000m, 220m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            brokerAgent.Id, 91, "Tỉnh An Giang", 30292, "Phường Mỹ Long",
            "Ấp Mỹ Khánh", 4, 2, 1, PropertyDirection.South,
            10.3750, 105.4200, views: 82,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1605146769289-440113cc3d00?w=800&q=80",
            "https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=800&q=80",
            "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?w=800&q=80");

        items.Add(CreateListing(
            "Đất mặt tiền Buôn Ma Thuột Đắk Lắk",
            "Lô đất 120m2 mặt tiền 6m, vị trí trung tâm, phù hợp quán cafe homestay.",
            1_800_000_000m, 120m, ListingType.ForSale, LandTypeId, SellTransactionId,
            brokerAgent.Id, 66, "Tỉnh Đắk Lắk", 22015, "Phường Tân Lập",
            "Đường Nguyễn Tất Thành", null, null, null, PropertyDirection.East,
            12.6667, 108.0500, views: 55,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=800&q=80",
            "https://images.unsplash.com/photo-1464146072230-91cabc968266?w=800&q=80",
            "https://images.unsplash.com/photo-1416879595882-3373a0480b5b?w=800&q=80");

        items.Add(CreateListing(
            "Căn hộ cho thuê Pleiku Gia Lai",
            "Căn hộ 2PN mới, nội thất cơ bản, gần trung tâm hành chính tỉnh.",
            8_500_000m, 70m, ListingType.ForRent, ApartmentTypeId, RentTransactionId,
            brokerAgent.Id, 52, "Tỉnh Gia Lai", 21553, "Phường Hội Phú",
            "25 Trần Phú", 2, 1, 15, PropertyDirection.North,
            13.9833, 108.0000, views: 42,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=800&q=80",
            "https://images.unsplash.com/photo-1560185127-6ed189bf02f4?w=800&q=80",
            "https://images.unsplash.com/photo-1618221195710-dd6b41faeca6?w=800&q=80");

        items.Add(CreateListing(
            "Nhà liền kề Bắc Ninh gần KCN Samsung",
            "Nhà 3 tầng khu dân cư an ninh, cách KCN Yên Phong 15 phút.",
            3_200_000_000m, 90m, ListingType.ForSale, HouseTypeId, SellTransactionId,
            adminAgent.Id, 24, "Tỉnh Bắc Ninh", 7210, "Phường Võ Cường",
            "Khu đô thị mới", 3, 2, 3, PropertyDirection.SouthEast,
            21.1861, 106.0763, views: 88,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=800&q=80",
            "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?w=800&q=80",
            "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=800&q=80");

        items.Add(CreateListing(
            "Đất nền ven biển Cà Mau",
            "Lô đất 150m2 gần biển Đốc, tiềm năng du lịch sinh thái.",
            900_000_000m, 150m, ListingType.ForSale, LandTypeId, SellTransactionId,
            brokerAgent.Id, 96, "Tỉnh Cà Mau", 31825, "Phường Tân Thành",
            "Đường ven biển", null, null, null, PropertyDirection.South,
            8.6864, 105.1860, views: 36,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=800&q=80",
            "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?w=800&q=80",
            "https://images.unsplash.com/photo-1464146072230-91cabc968266?w=800&q=80");

        items.Add(CreateListing(
            "Căn hộ view sông Sa Đéc Đồng Tháp",
            "Căn hộ mini 1PN, view sông Tiền, yên tĩnh, phù hợp nghỉ cuối tuần.",
            2_300_000_000m, 55m, ListingType.ForSale, ApartmentTypeId, SellTransactionId,
            brokerAgent.Id, 82, "Tỉnh Đồng Tháp", 28249, "Phường Sa Đéc",
            "12 Nguyễn Huệ", 1, 1, 12, PropertyDirection.West,
            10.2900, 105.7560, views: 64,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80",
            "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&q=80",
            "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&q=80");

        items.Add(CreateListing(
            "Văn phòng cho thuê trung tâm Đà Nẵng",
            "Văn phòng 120m2 open space, view sông Hàn, có thang máy riêng.",
            35_000_000m, 120m, ListingType.ForRent, OfficeTypeId, RentTransactionId,
            adminAgent.Id, 48, "Thành phố Đà Nẵng", 20194, "Phường Hải Vân",
            "88 Bach Dang", null, 2, 18, PropertyDirection.North,
            16.0750, 108.2230, views: 76,
            afterCreate: ActivateForDemo));
        AttachImages(items[^1],
            "https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&q=80",
            "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&q=80",
            "https://images.unsplash.com/photo-1484154218962-a197022b5858?w=800&q=80");

        return items;
    }

    private static void ActivateForDemo(Property property)
    {
        if (property.Status == PropertyStatus.Draft)
            property.SubmitForApproval();
        if (property.Status == PropertyStatus.PendingApproval)
            property.Publish();
    }

    private static void AttachImages(Property property, params string[] urls)
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

    private static Property CreateListing(
        string title,
        string description,
        decimal price,
        decimal area,
        ListingType listingType,
        Guid propertyTypeId,
        Guid transactionTypeId,
        Guid agentId,
        int provinceCode,
        string provinceName,
        int wardCode,
        string wardName,
        string street,
        int? bedrooms,
        int? bathrooms,
        int? floors,
        PropertyDirection? direction,
        double latitude,
        double longitude,
        bool featured = false,
        int views = 0,
        Action<Property>? afterCreate = null)
    {
        var address = new Address(street, wardName, "", provinceName, latitude: latitude, longitude: longitude);
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
            provinceCode,
            provinceName,
            wardCode,
            wardName,
            latitude,
            longitude);

        afterCreate?.Invoke(property);

        if (featured && property.Status == PropertyStatus.Active)
            property.SetFeatured(true);

        for (var i = 0; i < views; i++)
            property.IncrementViewCount();

        return property;
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string firstName,
        string lastName,
        string role,
        string password,
        string? phoneNumber = null)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            if (!await userManager.IsInRoleAsync(existing, role))
                await userManager.AddToRoleAsync(existing, role);
            return existing;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            IsActive = true,
            PhoneNumber = phoneNumber ?? "0901234567",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create test user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await userManager.AddToRoleAsync(user, role);
        return user;
    }

    private static async Task<Agent> EnsureAgentAsync(
        ApplicationDbContext db,
        ApplicationUser user,
        string agencyName,
        CancellationToken cancellationToken)
    {
        var existing = await db.Set<Agent>()
            .FirstOrDefaultAsync(a => a.UserId == user.Id && !a.IsDeleted, cancellationToken);

        if (existing is not null)
            return existing;

        var agent = Agent.Create(
            user.FullName,
            user.Email ?? user.UserName ?? "agent@test.vietpropestate.vn",
            user.PhoneNumber ?? "0901234567",
            licenseNumber: $"BRK-{user.Id[..8].ToUpperInvariant()}",
            agencyName: agencyName,
            userId: user.Id);

        await db.Set<Agent>().AddAsync(agent, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return agent;
    }
}
