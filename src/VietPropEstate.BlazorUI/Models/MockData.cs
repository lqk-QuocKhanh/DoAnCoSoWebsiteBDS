using VietPropEstate.Application.Features.Properties.DTOs;
using VietPropEstate.Application.Features.Payments.DTOs;
using VietPropEstate.Domain.Enums;

namespace VietPropEstate.BlazorUI.Models;

/// <summary>
/// Realistic Vietnamese mock data for development and offline mode.
/// Remove or disable this class when switching to live API.
/// </summary>
public static class MockData
{
    public static readonly List<PropertyDto> Properties = [
        new PropertyDto
        {
            Id = new Guid("11111111-1111-1111-1111-111111111111"),
            Title = "Căn hộ cao cấp Vinhomes Central Park, view sông Sài Gòn",
            Slug = "can-ho-cao-cap-vinhomes-central-park-11111111",
            PriceAmount = 8_500_000_000,
            PriceCurrency = "VND",
            Area = 95,
            NumberOfBedrooms = 3,
            NumberOfBathrooms = 2,
            ListingType = ListingType.ForSale,
            Status = PropertyStatus.Active,
            ProvinceName = "TP. Hồ Chí Minh",
            WardName = "Phường 22, Bình Thạnh",
            FullAddress = "Vinhomes Central Park, Phường 22, Bình Thạnh, TP. HCM",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&q=80",
            IsFeatured = true,
            ViewCount = 2847,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            PropertyTypeName = "Căn hộ",
            TransactionTypeName = "Bán",
            AgentName = "Nguyễn Văn Minh"
        },
        new PropertyDto
        {
            Id = new Guid("22222222-2222-2222-2222-222222222222"),
            Title = "Nhà phố liên kế Times City, 5 tầng, nội thất đầy đủ",
            Slug = "nha-pho-lien-ke-times-city-22222222",
            PriceAmount = 12_800_000_000,
            PriceCurrency = "VND",
            Area = 72,
            NumberOfBedrooms = 4,
            NumberOfBathrooms = 3,
            ListingType = ListingType.ForSale,
            Status = PropertyStatus.Active,
            ProvinceName = "Hà Nội",
            WardName = "Phường Hoàng Văn Thụ, Hoàng Mai",
            FullAddress = "Times City, Phường Hoàng Văn Thụ, Hoàng Mai, Hà Nội",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1568605114967-8130f3a36994?w=800&q=80",
            IsFeatured = true,
            ViewCount = 1253,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            PropertyTypeName = "Nhà phố",
            TransactionTypeName = "Bán",
            AgentName = "Trần Thị Hoa"
        },
        new PropertyDto
        {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            Title = "Biệt thự song lập Ecopark, hồ bơi riêng, sân vườn 500m²",
            Slug = "biet-thu-song-lap-ecopark-33333333",
            PriceAmount = 35_000_000_000,
            PriceCurrency = "VND",
            Area = 350,
            NumberOfBedrooms = 5,
            NumberOfBathrooms = 4,
            ListingType = ListingType.ForSale,
            Status = PropertyStatus.Active,
            ProvinceName = "Hà Nội",
            WardName = "Phường Xuân Quan, Văn Giang",
            FullAddress = "Ecopark, Xuân Quan, Văn Giang, Hưng Yên",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1613977257363-707ba9348227?w=800&q=80",
            IsFeatured = true,
            ViewCount = 892,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            PropertyTypeName = "Biệt thự",
            TransactionTypeName = "Bán",
            AgentName = "Lê Quốc Hùng"
        },
        new PropertyDto
        {
            Id = new Guid("44444444-4444-4444-4444-444444444444"),
            Title = "Cho thuê căn hộ studio Masteri Thảo Điền, full nội thất",
            Slug = "cho-thue-can-ho-studio-masteri-44444444",
            PriceAmount = 15_000_000,
            PriceCurrency = "VND",
            Area = 45,
            NumberOfBedrooms = 1,
            NumberOfBathrooms = 1,
            ListingType = ListingType.ForRent,
            Status = PropertyStatus.Active,
            ProvinceName = "TP. Hồ Chí Minh",
            WardName = "Phường Thảo Điền, Quận 2",
            FullAddress = "Masteri Thảo Điền, Phường Thảo Điền, TP. Thủ Đức",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800&q=80",
            IsFeatured = false,
            ViewCount = 3421,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            PropertyTypeName = "Căn hộ",
            TransactionTypeName = "Cho thuê",
            AgentName = "Phạm Thanh Tùng"
        },
        new PropertyDto
        {
            Id = new Guid("55555555-5555-5555-5555-555555555555"),
            Title = "Đất nền khu đô thị FPT City Đà Nẵng, lô góc 2 mặt tiền",
            Slug = "dat-nen-khu-do-thi-fpt-city-da-nang-55555555",
            PriceAmount = 4_200_000_000,
            PriceCurrency = "VND",
            Area = 120,
            NumberOfBedrooms = null,
            NumberOfBathrooms = null,
            ListingType = ListingType.ForSale,
            Status = PropertyStatus.Active,
            ProvinceName = "Đà Nẵng",
            WardName = "Phường Hòa Hải, Ngũ Hành Sơn",
            FullAddress = "FPT City, Hòa Hải, Ngũ Hành Sơn, Đà Nẵng",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1464082354059-27db6ce50048?w=800&q=80",
            IsFeatured = false,
            ViewCount = 654,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            PropertyTypeName = "Đất nền",
            TransactionTypeName = "Bán",
            AgentName = "Võ Minh Đức"
        },
        new PropertyDto
        {
            Id = new Guid("66666666-6666-6666-6666-666666666666"),
            Title = "Văn phòng cho thuê tòa nhà Bitexco Financial Tower, tầng 18",
            Slug = "van-phong-cho-thue-bitexco-66666666",
            PriceAmount = 80_000_000,
            PriceCurrency = "VND",
            Area = 200,
            NumberOfBedrooms = null,
            NumberOfBathrooms = 2,
            ListingType = ListingType.ForRent,
            Status = PropertyStatus.Active,
            ProvinceName = "TP. Hồ Chí Minh",
            WardName = "Phường Bến Nghé, Quận 1",
            FullAddress = "Bitexco Financial Tower, Bến Nghé, Quận 1, TP. HCM",
            PrimaryImageUrl = "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&q=80",
            IsFeatured = false,
            ViewCount = 987,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            PropertyTypeName = "Văn phòng",
            TransactionTypeName = "Cho thuê",
            AgentName = "Ngô Thu Hương"
        }
    ];

    public static readonly List<VIPPackageDto> VipPackages = [
        new VIPPackageDto { Id = Guid.NewGuid(), Name = "Cơ Bản", Price = 500_000, Currency = "VND", DurationDays = 30, MaxListings = 3, IsActive = true, Description = "Đăng tối đa 3 tin trong 30 ngày" },
        new VIPPackageDto { Id = Guid.NewGuid(), Name = "Tiêu Chuẩn", Price = 1_000_000, Currency = "VND", DurationDays = 30, MaxListings = 10, IsActive = true, Description = "Đăng tối đa 10 tin trong 30 ngày. Hiển thị ưu tiên" },
        new VIPPackageDto { Id = Guid.NewGuid(), Name = "Cao Cấp", Price = 2_500_000, Currency = "VND", DurationDays = 90, MaxListings = 30, IsActive = true, Description = "Đăng tối đa 30 tin trong 90 ngày. Nổi bật trang chủ" },
        new VIPPackageDto { Id = Guid.NewGuid(), Name = "Gold", Price = 5_000_000, Currency = "VND", DurationDays = 180, MaxListings = 99, IsActive = true, Description = "Không giới hạn tin trong 180 ngày. VIP đặc biệt" }
    ];

    public static readonly AddressProvince[] Provinces = [
        new AddressProvince { Code = 79, Name = "TP. Hồ Chí Minh", Codename = "thanh_pho_ho_chi_minh", DivisionType = "thành phố trung ương" },
        new AddressProvince { Code = 1,  Name = "Hà Nội",           Codename = "ha_noi",                DivisionType = "thành phố trung ương" },
        new AddressProvince { Code = 48, Name = "Đà Nẵng",          Codename = "da_nang",               DivisionType = "thành phố trung ương" },
        new AddressProvince { Code = 74, Name = "Bình Dương",        Codename = "binh_duong",            DivisionType = "tỉnh" },
        new AddressProvince { Code = 75, Name = "Đồng Nai",          Codename = "dong_nai",              DivisionType = "tỉnh" },
        new AddressProvince { Code = 92, Name = "Cần Thơ",           Codename = "can_tho",               DivisionType = "thành phố trung ương" },
        new AddressProvince { Code = 56, Name = "Khánh Hòa",         Codename = "khanh_hoa",             DivisionType = "tỉnh" },
        new AddressProvince { Code = 77, Name = "Bà Rịa-Vũng Tàu",  Codename = "ba_ria_vung_tau",       DivisionType = "tỉnh" }
    ];
}
