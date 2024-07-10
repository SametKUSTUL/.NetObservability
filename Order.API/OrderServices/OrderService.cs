using Common.Shared.DTOs;
using OpenTelemetry.Shared;
using Order.API.Models;
using Order.API.StockServices;
using System.Diagnostics;

namespace Order.API.OrderServices;

public class OrderService
{

    private readonly AppDbContext _context;
    private readonly StockService _stockService;

    public OrderService(AppDbContext context, StockService stockService)
    {
        _context = context;
        _stockService = stockService;
    }

    public async Task<ResponseDto<OrderCreateResponseDto>> CreateAsync(OrderCreateRequestDto request)
    {

        Activity.Current?.SetTag("AspNetCore(instrumentation) tag1", "AspNetCore(instrumentation) tag1 value1");

        using var activity = ActivitySourceProvider.Source.StartActivity();
        activity?.AddEvent(new System.Diagnostics.ActivityEvent("Sipariş süreci başladı"));


        var newOrder = new Order()
        {
            Created = DateTime.Now,
            OrderCode = Guid.NewGuid().ToString(),
            Status = OrderStatus.Success,
            UserId = request.UserId,
            Items = request.Items.Select(x => new OrderItem()
            {
                Count = x.Count,
                ProductId = x.ProductId,
                UnitPrice = x.UnitPrice,
            }).ToList()
        };

        _context.Orders.Add(newOrder);
        _context.SaveChanges();


        var (isSuccess, failMessage)= await _stockService.CheckStockAndPaymentStartAsync(new Common.Shared.DTOs.StockCheckAndPaymentProcessRequestDto
        {
            OrderCode=newOrder.OrderCode,
            OrderItems=request.Items,
        });

        if (!isSuccess)
        {
            return ResponseDto<OrderCreateResponseDto>.Fail(StatusCodes.Status500InternalServerError,failMessage);
        }

        activity?.SetTag("order user id", request.UserId);
        activity?.AddEvent(new("Sipariş süreci tamamlandi"));

        return ResponseDto<OrderCreateResponseDto>.Success(StatusCodes.Status200OK, new OrderCreateResponseDto()
        {
            Id = newOrder.Id,
        });
    }
}
