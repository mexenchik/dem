namespace LightStepWinForms.Models;

internal sealed class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal PriceAtMoment { get; set; }

    public decimal Sum => Count * PriceAtMoment;

    public OrderItem Clone()
    {
        return (OrderItem)MemberwiseClone();
    }
}
