namespace Order.API.OrderServices;

public class Order
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public DateTime Created { get; set; }
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; }
}

public enum OrderStatus : byte
{
    Success = 1, Failed = 0
}

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal UnitPrice { get; set; }
}
