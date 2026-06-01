namespace LightStepWinForms.App;

internal static class ImageLoader
{
    public static Image? TryLoadLogo()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", "logo.png"),
            Path.Combine(AppContext.BaseDirectory, "logo.png"),
            Path.Combine(Environment.CurrentDirectory, "Assets", "logo.png")
        };

        foreach (var path in candidates)
        {
            if (!File.Exists(path))
            {
                continue;
            }

            using var stream = File.OpenRead(path);
            return Image.FromStream(stream);
        }

        return null;
    }
}
