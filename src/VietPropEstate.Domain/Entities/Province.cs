namespace VietPropEstate.Domain.Entities;

/// <summary>
/// Vietnamese province / centrally administered city.
/// Uses the post-2025 administrative structure (63 → 34 units after mergers).
/// </summary>
public class Province
{
    public int Id { get; private set; }

    /// <summary>Official government numeric code (e.g. 1 = Hà Nội, 79 = TP. HCM).</summary>
    public int Code { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Codename { get; private set; } = string.Empty;
    public string DivisionType { get; private set; } = string.Empty;
    public int PhoneCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Ward> _wards = [];
    public IReadOnlyCollection<Ward> Wards => _wards.AsReadOnly();

    private Province() { }

    public static Province Create(
        int code,
        string name,
        string codename,
        string divisionType,
        int phoneCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(codename);

        return new Province
        {
            Code = code,
            Name = name.Trim(),
            Codename = codename.Trim(),
            DivisionType = divisionType.Trim(),
            PhoneCode = phoneCode,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string codename, string divisionType, int phoneCode)
    {
        Name = name.Trim();
        Codename = codename.Trim();
        DivisionType = divisionType.Trim();
        PhoneCode = phoneCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
