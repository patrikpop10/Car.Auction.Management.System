using Api;
using Api.Endpoints;
using Application;
using Infra;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();

builder.Services
    .AddApplicationServices()
    .AddValidators()
    .AddInfraServices()
    .AddExceptionHandler<ExceptionHandler>();

var app = builder.Build();

app.MapVehiclesEndpoints()
    .MapAuctionsEndpoints();

app.Run();