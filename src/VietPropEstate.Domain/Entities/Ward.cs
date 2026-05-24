namespace VietPropEstate.Domain.Entities;

/// <summary>
/// Vietnamese ward / commune / town (phường / xã / thị trấn).
/// After the 2025 administrative merger, wards belong directly to Provinces
/// (the district level has been abolished).
/// </summary>
public class Ward
{
    public int Id { get; private set; }

    /// <summary>Official government numeric code.</summary>
    public int Code { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Codename { get; private set; } = string.Empty;
    public string DivisionType { get; private set; } = string.Empty;

    /// <summary>Numeric code of the parent Province.</summary>
    public int ProvinceCode { get; private set; }

    /// <summary>Database ID of the parent Province.</summary>
    public int ProvinceId { get; private set; }

    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Province Province { get; private set; } = null!;

    private Ward() { }

    public static Ward Create(
        int code,
        string name,
        string codename,
        string divisionType,
        int provinceCode,
        int provinceId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(codename);

        return new Ward
        {
            Code = code,
            Name = name.Trim(),
            Codename = codename.Trim(),
            DivisionType = divisionType.Trim(),
            ProvinceCode = provinceCode,
            ProvinceId = provinceId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string codename, string divisionType)
    {
        Name = name.Trim();
        Codename = codename.Trim();
        DivisionType = divisionType.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
