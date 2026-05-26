using Microsoft.JSInterop;

namespace VietPropEstate.BlazorUI.Services;

public enum AppLanguage
{
    Vietnamese,
    English
}

public sealed class LocaleState : IDisposable
{
    private const string StorageKey = "vpe-language";
    private readonly IJSRuntime _js;

    public LocaleState(IJSRuntime js) => _js = js;

    public AppLanguage Language { get; private set; } = AppLanguage.Vietnamese;

    public bool IsVietnamese => Language == AppLanguage.Vietnamese;

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (stored == "en")
                Language = AppLanguage.English;
        }
        catch
        {
            // Ignore storage errors during prerender or private browsing.
        }
    }

    public async Task SetLanguageAsync(AppLanguage language)
    {
        Language = language;

        try
        {
            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                StorageKey,
                language == AppLanguage.English ? "en" : "vi");
        }
        catch
        {
            // Ignore storage errors.
        }

        OnChange?.Invoke();
    }

    public string T(string vietnamese, string english) =>
        IsVietnamese ? vietnamese : english;

    public void Dispose() => OnChange = null;
}
