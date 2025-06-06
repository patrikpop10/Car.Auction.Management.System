using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Api.Endpoints;
using Application.DTOs;
using Application.DTOs.Requests;
using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Services;
using FluentAssertions;
using Infra.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Car.Auction.Management.System;

[TestFixture]
public class ApiComponentTests
{
    private HttpClient _client;
    
    
    [OneTimeSetUp]
    public void Setup()
    {
        // Initialization code
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }
    
    [OneTimeTearDown]
    public void Cleanup()
    {
        _client.Dispose();
    }
    
    private static StringContent AsJson(object o) =>
            new(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    [Test]
    public async Task CanAddAndSearchVehicle()
    {
        var vehicle = new VehicleRequest()
        {
            Id = Guid.NewGuid(),
            Car = new CarDto
            {
                Type = "Sedan",
                Manufacturer = "Toyota",
                Model = "Camry",
                Year = 2024,
                NumberOfDoors = 5
            },
            StartingBid = new MoneyDto
            {
                Amount = 10000,
                Currency = "USD"
            },
        };

        var resp = await _client.PostAsync("/vehicles", AsJson(vehicle));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        
        //starting a new auction
        var startAuctionResponse = await _client.PostAsync($"/auctions/start/{vehicle.Id}", null);
        startAuctionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Search by type
        var searchResponse = await _client.GetAsync("/vehicles/search?type=Sedan");
        var resultJson = await searchResponse.Content.ReadAsStringAsync();
        resultJson.Should().Contain("Toyota");
    }

    [Test]
    public async Task AddingVehicleWithDuplicateIdReturnsError()
    {
        var id = Guid.NewGuid();
        var vehicle1 = new VehicleRequest()
        {
            Id = id,
            Car = new CarDto(){
                Type = "Sedan",
                Manufacturer = "Honda",
                Model = "Accord",
                Year = 2022,
                NumberOfDoors = 5,
                },
            StartingBid = new MoneyDto
            {
                Amount = 5000,
                Currency = "USD"
            },
        };
        var vehicle2 = new VehicleRequest()
        {
            Id = id,
            Car = new CarDto()
            {
                Type = "Hatchback",
                Manufacturer = "Toyota",
                Model = "Yaris",
                Year = 2023,
                NumberOfDoors = 5,
            },
            StartingBid = new MoneyDto
            {
                Amount = 6000,
                Currency = "USD"
            },
        };

        await _client.PostAsync("/vehicles", AsJson(vehicle1));
        var resp = await _client.PostAsync("/vehicles", AsJson(vehicle2));
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CanStartAndCloseAuction()
    {
        var vehicle = new VehicleRequest()
        {
            Id = Guid.NewGuid(),
            Car = new CarDto()
            {
                Type = "Truck",
                Manufacturer = "Ford",
                Model = "F-150",
                Year = 2022,
                NumberOfSeats = 2,
                LoadCapacity = 10.0

            },
            StartingBid = new MoneyDto
            {
                Amount = 30000,
                Currency = "USD"
            },
        };
        
        var addVehicleResponse = await _client.PostAsync("/vehicles", AsJson(vehicle));
        var startAuctionResponse = await _client.PostAsync($"/auctions/start/{vehicle.Id}", null);
        startAuctionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var closeAuctionResponse = await _client.PostAsync($"/auctions/close/{vehicle.Id}", null);
        closeAuctionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task StartingAuctionForNonExistentVehicleReturnsError()
    {
        var response = await _client.PostAsync($"/auctions/start/{Guid.NewGuid()}", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task StartingAuctionWhenAlreadyActiveReturnsError()
    {
        // Arrange
        var vehicle = new VehicleRequest()
        {
            Id = Guid.NewGuid(),
            Car = new CarDto()
            {
                Type = "SUV",
                Manufacturer = "Nissan",
                Model = "Rogue",
                Year = 2021,
                NumberOfSeats = 5
            },
            StartingBid = new MoneyDto
            {
                Amount = 25000,
                Currency = "USD"
            },
        };
        // Act 
        var createVehicleResponse = await _client.PostAsync("/vehicles", AsJson(vehicle));
        var startAuctionResponse = await _client.PostAsync($"/auctions/start/{vehicle.Id}", null);
        var startAuction2Response = await _client.PostAsync($"/auctions/start/{vehicle.Id}", null);
        
        // Assert
        createVehicleResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        startAuctionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        startAuction2Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CanPlaceBidAndEnforceBidRules()
    {
        var vehicle = new VehicleRequest
        {
            Id = Guid.NewGuid(),
            Car = new CarDto
            {
                Type = "Sedan",
                Manufacturer = "Honda",
                Model = "Accord",
                Year = 2022,
                NumberOfDoors = 4
            },
            StartingBid = new MoneyDto
            {
                Amount = 20000, 
                Currency = "USD"
            },
        };
        
        await _client.PostAsync("/vehicles", AsJson(vehicle));
        await _client.PostAsync($"/auctions/start/{vehicle.Id}", null);

        var bid1 = new BidRequest("Alice", new MoneyDto
        {
            Amount = 21000,
            Currency = "USD"
        });
        var bidResp1 = await _client.PostAsync($"/auctions/bid/{vehicle.Id}", AsJson(bid1));
        bidResp1.StatusCode.Should().Be(HttpStatusCode.OK);

        var bid2 = new BidRequest("Bob", new MoneyDto
        {
            Amount = 22000,
            Currency = "USD"
        });
        var bidResponse = await _client.PostAsync($"/auctions/bid/{vehicle.Id}", AsJson(bid2));
        bidResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Try lower bid
        var badBid = new BidRequest("Charlie", new MoneyDto
        {
            Amount = 21500,
            Currency = "USD"
        });
        var badBidResponse = await _client.PostAsync($"/auctions/bid/{vehicle.Id}", AsJson(badBid));
        badBidResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task PlacingBidWithoutActiveAuctionReturnsError()
    {
        var vehicle = new VehicleRequest()
        {
            Id = Guid.NewGuid(),
            Car = new CarDto()
            {
                Type = "Hatchback",
                Manufacturer = "Mazda",
                Model = "3",
                Year = 2021,
                NumberOfDoors = 5
            },
            StartingBid = new MoneyDto
            {
                Amount = 15000,
                Currency = "USD"
            }
        };
        await _client.PostAsync("/vehicles", AsJson(vehicle));
        var bid = new BidRequest("Dave", new MoneyDto
        {
            Amount = 8100,
            Currency = "USD"
        });
        var resp = await _client.PostAsync($"/auctions/bid/{vehicle.Id}", AsJson(bid));
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ClosingAuctionThatIsNotActiveReturnsError()
    {
        var vehicle = new VehicleRequest()
        {
            Id = Guid.NewGuid(),
           Car = new CarDto()
           {
               Type = "SUV",
               Manufacturer = "Ford",
               Model = "Explorer",
               Year = 2023,
               NumberOfSeats = 7

           },
            StartingBid = new MoneyDto
            {
                Amount = 35000,
                Currency = "USD"
            },
        };
        
        await _client.PostAsync("/vehicles", AsJson(vehicle));
        var resp = await _client.PostAsync($"/auctions/close/{vehicle.Id}", null);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task SearchReturnsEmptyWhenNoMatches()
    {
        var resp = await _client.GetAsync("/vehicles/search?manufacturer=Nonexistent");
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Be("[]");
    }

    [Test]
    public async Task AddingVehicleWithInvalidInputReturnsBadRequest()
    {
        var vehicle = new { Id = "not-a-guid", Type = "Sedan", Manufacturer = "Toyota", Model = "Camry", Year = 2024, StartingBid = 10000, NumberOfDoors = 4 };
        var resp = await _client.PostAsync("/vehicles", AsJson(vehicle));
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}
