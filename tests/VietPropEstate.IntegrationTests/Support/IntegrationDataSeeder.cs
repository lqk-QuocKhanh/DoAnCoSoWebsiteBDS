using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VietPropEstate.Domain.Entities;
using VietPropEstate.Domain.Enums;
using VietPropEstate.Domain.ValueObjects;
using VietPropEstate.Infrastructure.Identity;
using VietPropEstate.Infrastructure.Persistence;

namespace VietPropEstate.IntegrationTests.Support;

internal static class IntegrationDataSeeder
{
    private static readonly Guid HouseTypeId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SellTransactionId = new("44444444-4444-4444-4444-444444444444");

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (await db.Properties.AnyAsync())
            return;

        var admin = await userManager.FindByEmailAsync("admin@vietpropestate.vn")
            ?? throw new InvalidOperationException("Admin user was not seeded.");

        var agent = await db.Set<Agent>().FirstOrDefaultAsync(a => a.UserId == admin.Id);
        if (agent is null)
        {
            agent = Agent.Create(
                admin.FullName,
                admin.Email ?? "admin@vietpropestate.vn",
                admin.PhoneNumber ?? "0900000000",
                licenseNumber: "INT-ADMIN",
                agencyName: "Integration Test Agency",
                userId: admin.Id);
            await db.Set<Agent>().AddAsync(agent);
            await db.SaveChangesAsync();
        }

        var property = Property.Create(
            "Integration Test Listing",
            "Property used by integration tests",
            new Money(5_000_000_000m, "VND"),
            90m,
            ListingType.ForSale,
            new Address("1 Test Street", "Phường Test", "Quận Test", "TP.HCM"),
            HouseTypeId,
            agent.Id,
            3,
            2,
            2,
            PropertyDirection.South,
            SellTransactionId);

        property.SubmitForApproval();
        property.Publish();
        property.AddImage(PropertyImage.Create(
            property.Id,
            "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=800&q=80",
            displayOrder: 0,
            isPrimary: true));

        await db.Properties.AddAsync(property);
        await db.SaveChangesAsync();
    }
}
