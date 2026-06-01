namespace LightStepWinForms.Models;

internal sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Article { get; set; } = "";
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Supplier { get; set; } = "";
    public decimal Price { get; set; }
    public string Size { get; set; } = "";
    public string Unit { get; set; } = "пара";
    public int Quantity { get; set; }
    public int DiscountPercent { get; set; }
    public string? ImagePath { get; set; }

    public decimal FinalPrice => Math.Round(Price * (100 - DiscountPercent) / 100m, 2);

    public Product Clone()
    {
        return (Product)MemberwiseClone();
    }
}
