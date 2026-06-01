namespace LightStepWinForms.App;

internal static class AppTheme
{
    public static readonly Color MainBackground = Color.White;
    public static readonly Color SecondaryBackground = ColorTranslator.FromHtml("#7FFF00");
    public static readonly Color Accent = ColorTranslator.FromHtml("#00FA9A");
    public static readonly Color DiscountBackground = ColorTranslator.FromHtml("#2E8B57");
    public static readonly Color PanelBackground = Color.FromArgb(245, 250, 245);
    public static readonly Color Border = Color.FromArgb(210, 225, 210);
    public static readonly Color Text = Color.FromArgb(28, 38, 28);
    public static readonly Color MutedText = Color.FromArgb(95, 105, 95);
    public static readonly Color Danger = Color.FromArgb(190, 60, 60);

    public static readonly Font TitleFont = new("Times New Roman", 18F, FontStyle.Bold);
    public static readonly Font HeaderFont = new("Times New Roman", 13F, FontStyle.Bold);
    public static readonly Font RegularFont = new("Times New Roman", 11F, FontStyle.Regular);
    public static readonly Font SmallFont = new("Times New Roman", 9.5F, FontStyle.Regular);

    public static void ApplyForm(Form form)
    {
        form.Font = RegularFont;
        form.BackColor = MainBackground;
        form.StartPosition = FormStartPosition.CenterScreen;
    }

    public static void ApplyPrimaryButton(Button button)
    {
        button.BackColor = Accent;
        button.ForeColor = Text;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Color.FromArgb(0, 170, 110);
        button.Font = HeaderFont;
    }

    public static void ApplySecondaryButton(Button button)
    {
        button.BackColor = Color.White;
        button.ForeColor = Text;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Border;
    }

    public static void ApplyDangerButton(Button button)
    {
        button.BackColor = Color.FromArgb(255, 235, 235);
        button.ForeColor = Danger;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderColor = Color.FromArgb(230, 160, 160);
    }
}
