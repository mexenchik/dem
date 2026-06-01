namespace LightStepWinForms.Models;

internal sealed class ProductRow
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Article { get; init; } = "";
    public string Category { get; init; } = "";
    public string Brand { get; init; } = "";
    public string Supplier { get; init; } = "";
    public decimal Price { get; init; }
    public int DiscountPercent { get; init; }
    public decimal FinalPrice { get; init; }
    public string Size { get; init; } = "";
    public int Quantity { get; init; }
}
