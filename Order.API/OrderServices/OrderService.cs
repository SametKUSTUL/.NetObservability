using OpenTelemetry.Shared;
using Order.API.Models;
using System.Diagnostics;

namespace Order.API.OrderServices;

public class OrderService
{

    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderCreateResponseDto> CreateAsync(OrderCreateRequestDto requestDto)
    {

        Activity.Current?.SetTag("AspNetCore(instrumentation) tag1", "AspNetCore(instrumentation) tag1 value1");

        using var activity = ActivitySourceProvider.Source.StartActivity();
        activity?.AddEvent(new System.Diagnostics.ActivityEvent("Sipariş süreci başladı"));


        var newOrder = new Order()
        {
            Created = DateTime.Now,
            OrderCode = Guid.NewGuid().ToString(),
            Status = OrderStatus.Success,
            UserId = requestDto.UserId,
            Items = requestDto.Items.Select(x => new OrderItem()
            {
                Count = x.Count,
                ProductId = x.ProductId,
                UnitPrice = x.UnitPrice,
            }).ToList()
        };

        _context.Orders.Add(newOrder);
        _context.SaveChanges();

        activity?.SetTag("order user id", requestDto.UserId);


        activity?.AddEvent(new("Sipariş süreci tamamlandi"));


        return new OrderCreateResponseDto()
        {
            Id = newOrder.Id,
        };

    }
}
