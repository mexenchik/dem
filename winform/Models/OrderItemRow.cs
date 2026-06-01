namespace LightStepWinForms.Models;

internal sealed class OrderItemRow
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = "";
    public string Article { get; init; } = "";
    public int Count { get; init; }
    public decimal PriceAtMoment { get; init; }
    public decimal Sum { get; init; }
}
