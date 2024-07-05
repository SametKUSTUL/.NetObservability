using OpenTelemetry.Shared;
using System.Diagnostics;

namespace Order.API.OrderServices;

public class OrderService
{
    public Task CreateAsync(OrderCreateRequestDto requestDto)
    {

        Activity.Current?.SetTag("AspNetCore(instrumentation) tag1", "AspNetCore(instrumentation) tag1 value1");

        using var activity = ActivitySourceProvider.Source.StartActivity();
        activity?.AddEvent(new System.Diagnostics.ActivityEvent("Sipariş süreci başladı"));

        // Veri tabanı işleri

        activity?.SetTag("order user id",requestDto.UserId);


        activity?.AddEvent(new("Sipariş süreci tamamlandi"));
        

        return Task.CompletedTask;
    }
}
