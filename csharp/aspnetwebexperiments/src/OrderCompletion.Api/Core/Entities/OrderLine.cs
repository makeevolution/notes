namespace OrderCompletion.Api.Core.Entities;

public class OrderLine
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int OrderedQuantity { get; set; }
    public int? DeliveredQuantity { get; set; }
}