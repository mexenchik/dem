namespace LightStepWinForms.Models;

internal sealed class OrderRow
{
    public int Id { get; init; }
    public DateTime OrderDate { get; init; }
    public string ClientName { get; init; } = "";
    public string Status { get; init; } = "";
    public decimal TotalAmount { get; init; }
    public int ItemsCount { get; init; }
}
