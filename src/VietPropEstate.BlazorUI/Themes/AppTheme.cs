using MudBlazor;

namespace VietPropEstate.BlazorUI.Themes;

public static class AppTheme
{
    public static readonly MudTheme Theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#B91C1C",
            PrimaryLighten = "#FEE2E2",
            PrimaryDarken = "#7F1D1D",
            Secondary = "#D97706",
            SecondaryLighten = "#FEF3C7",
            SecondaryDarken = "#92400E",
            Tertiary = "#F59E0B",
            Info = "#2563EB",
            Success = "#16A34A",
            Warning = "#F59E0B",
            Error = "#DC2626",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#0F172A",
            Background = "#F8FAFC",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#0F172A",
            DrawerIcon = "#64748B",
            TextPrimary = "#0F172A",
            TextSecondary = "#64748B",
            TextDisabled = "#94A3B8",
            ActionDefault = "#64748B",
            ActionDisabled = "#CBD5E1",
            ActionDisabledBackground = "#F1F5F9",
            Divider = "#E2E8F0",
            DividerLight = "#F1F5F9",
            TableLines = "#E2E8F0",
            LinesDefault = "#E2E8F0",
            LinesInputs = "#CBD5E1",
            White = "#FFFFFF",
            Black = "#0F172A",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            AppbarHeight = "72px",
            DrawerWidthLeft = "280px",
            DrawerWidthRight = "280px",
            DrawerMiniWidthLeft = "56px",
        },

        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Inter", "system-ui", "-apple-system", "sans-serif"],
                FontSize = "16px",
                FontWeight = "400",
                LineHeight = "1.5",
                LetterSpacing = "0",
            },
            Button = new ButtonTypography
            {
                FontSize = "15px",
                FontWeight = "600",
                LetterSpacing = "0.01em",
                TextTransform = "none",
            },
        },
    };
}
