﻿using Order.API.OrderServices;
using OpenTelemetry.Shared;
using Common.Shared;
using Order.API.Models;
using Microsoft.EntityFrameworkCore;
using Order.API.StockServices;
using Order.API.RedisServices;
using StackExchange.Redis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisService = sp.GetService<RedisService>();
    return redisService!.GetConnectionMultiplexer;

});
builder.Services.AddSingleton(_ =>
{
    return new RedisService(builder.Configuration);
});
builder.Services.AddOpenTelemetryExt(builder.Configuration);


builder.Services.AddHttpClient<StockService>(options =>
{
    options.BaseAddress = new Uri((builder.Configuration.GetSection("ApiServices")["StockApi"])!);
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestAndResponseActivityMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
