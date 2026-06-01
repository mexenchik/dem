namespace LightStepWinForms.Models;

internal sealed class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalAmount { get; set; }

    public Order Clone()
    {
        return (Order)MemberwiseClone();
    }
}
