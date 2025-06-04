using System.Threading.Channels;
using Api.Endpoints;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Infra.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Register dependencies in DI
builder.Services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
builder.Services.AddSingleton<IAuctionRepository, InMemoryAuctionRepository>();

var channel = Channel.CreateUnbounded<Auction>(new UnboundedChannelOptions
{
    SingleReader = true,
    SingleWriter = false
});

builder.Services.AddSingleton(channel.Reader);
builder.Services.AddSingleton(channel.Writer);

builder.Services.AddScoped<IAuctionService, AuctionService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddSingleton<IAuctionMonitor, AuctionMonitor>();


var app = builder.Build();

app.MapVehiclesEndpoints();
app.MapAuctionsEndpoints();

app.Run();

public sealed record BidRequest(string Bidder, MoneyDto Amount);