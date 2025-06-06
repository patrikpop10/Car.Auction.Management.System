using System.Threading.Channels;
using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Services;
using Domain.Entities;
using Domain.Entities.Vehicles;
using Infra.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Car.Auction.Management.System;

[TestFixture]
public class AuctionServiceTests {
    private AuctionService _service;
    private InMemoryVehicleRepository _vehicleRepo;
    private InMemoryAuctionRepository _auctionRepo;
    private VehicleService _vehicleService;

    [SetUp]
    public void Setup() {
        _vehicleRepo = new InMemoryVehicleRepository();
        _auctionRepo = new InMemoryAuctionRepository();
        var logger = Substitute.For<ILogger<AuctionService>>();
        var loggerVehicle = Substitute.For<ILogger<VehicleService>>();
        _service = new AuctionService(_vehicleRepo, _auctionRepo, Channel.CreateUnbounded<AuctionMonitoringResponse>().Writer, logger);
        _vehicleService = new VehicleService(_vehicleRepo, _auctionRepo, loggerVehicle);
    }

    [Test]
    public async Task AddVehicle_ShouldAddSuccessfully()
    {
        var vehicle = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Corolla", 2022, new Money(5000, CurrencyType.USD), 4);
        var x = await _vehicleService.AddVehicle(vehicle);
        Assert.That( await _vehicleRepo.Exists(vehicle.Id), Is.True);
    }

    [Test]
    public async Task AddVehicle_DuplicateId_ThrowsException()
    {
        var id = new VehicleId(Guid.NewGuid());
        var vehicle1 = new Sedan(id, "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        var vehicle2 = new Hatchback(id, "Honda", "Fit", 2023, new Money(3000, CurrencyType.USD), 5);
        
        await _vehicleService.AddVehicle(vehicle1);
        var result = await _vehicleService.AddVehicle(vehicle2);
        
        Assert.That(result.Problem.Status, Is.EqualTo(409));
        Assert.That(result.Problem.Title, Is.EqualTo("DuplicateVehicle"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"A vehicle with the ID {id.Id} already exists."));
        Assert.That(await _vehicleRepo.Exists(id), Is.True);
    }

    [Test]
    public async Task SearchVehicles_ByTypeAndManufacturer_ReturnsCorrectResults()
    {
        var v1 = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        var v2 = new Hatchback(new VehicleId(Guid.NewGuid()), "Honda", "Fit", 2023, new Money(3000, CurrencyType.USD), 5);
        await _vehicleService.AddVehicle(v1);
        await _vehicleService.AddVehicle(v2);
        
        // starting an auction for the hatchback
        
        await _service.StartAuction(v2.Id);

        var results = await _vehicleService.SearchVehicles(type: "Hatchback", manufacturer: "Honda");
        Assert.That(1, Is.EqualTo(results.Count()));
        Assert.That("Fit", Is.EqualTo(results.First().Vehicle.Model));
    }

    [Test]
    public async Task StartAuction_Success()
    {
        var v = new Sedan( new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        await _vehicleService.AddVehicle(v);
        await _service.StartAuction(v.Id);
        Assert.That( await _auctionRepo.IsAuctionActive(v.Id), Is.True);
    }

    [Test]
    public async Task StartAuction_VehicleNotFound_Throws()
    {
        var vehicleId = new VehicleId(Guid.NewGuid());
        var result = await _service.StartAuction(vehicleId);
        Assert.That(result!.Problem!.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("NotFound"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"The requested vehicle with Id {vehicleId} was not found."));
    }

    [Test]
    public async Task StartAuction_AlreadyActive_Throws()
    {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        
        await _vehicleService.AddVehicle(v);
        await _service.StartAuction(v.Id);
        var result = await _service.StartAuction(v.Id);
        
        Assert.That(result.Problem.Status, Is.EqualTo(409));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionAlreadyActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"An auction is already active for the vehicle with ID {v.Id.Id}."));
    }

    [Test]
    public async Task PlaceBid_Success()
    {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);

        await _vehicleService.AddVehicle(v);
        await _service.StartAuction(v.Id);
        var bidDto1 = new BidRequest("Alice", new MoneyDto { Amount = 5000, Currency ="USD"} );
       
        await _service.PlaceBid(bidDto1, v.Id);

        var bidDto2 = new BidRequest("Bob", new MoneyDto { Amount = 5100, Currency ="USD"} );

    await _service.PlaceBid(bidDto2, v.Id);
        var auction =  await _auctionRepo.GetActiveByVehicleId(v.Id);
        
        Assert.That(2, Is.EqualTo(auction.Bids.Count));
        Assert.That("Bob", Is.EqualTo(auction.Bids.Last().Bidder));
    }

    [Test]
    public async Task PlaceBid_BelowCurrentHighest_ReturnsError()
    {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        
        await _vehicleService.AddVehicle(v);
        await _service.StartAuction(v.Id);

        var bidDto1 = new BidRequest("Alice", new MoneyDto { Amount = 5000, Currency = "USD" });
        await _service.PlaceBid(bidDto1, v.Id);

        var bidDto2 = new BidRequest("Bob", new MoneyDto { Amount = 4900, Currency = "USD" });
        var result = await _service.PlaceBid(bidDto2, v.Id);
        
        Assert.That(result.Problem.Status, Is.EqualTo(400));
        Assert.That(result.Problem.Title, Is.EqualTo("InvalidBidAmount"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo("The bid amount must be greater than the current highest bid."));
    }

    [Test]
    public async Task PlaceBid_NoActiveAuction_Throws()
    {
        var v = new Sedan( new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        
        await _vehicleService.AddVehicle(v);

        var bid = new BidRequest("Alice", new MoneyDto { Amount = 5000, Currency ="USD"} );
        var result = await _service.PlaceBid(bid, v.Id);
        
        Assert.That(result.Problem.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionNotActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"No active auction found for the vehicle with ID {v.Id.Id}."));
    }

    [Test]
    public async Task CloseAuction_Success()
    {
        var v = new Sedan( new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        
        await _vehicleService.AddVehicle(v);
        await _service.StartAuction(v.Id);
        await _service.CloseAuction(v.Id);
        
        var auction = await _auctionRepo.GetByVehicleId(v.Id);
        Assert.That(auction.IsActive, Is.False);
    }

    [Test]
    public async Task CloseAuction_NoActiveAuction_Throws()
    {
        var v = new Sedan( new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        
        await _vehicleService.AddVehicle(v);
        var result = await _service.CloseAuction(v.Id);
        
        Assert.That(result.Problem!.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionNotActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"No active auction found for the vehicle with ID {v.Id.Id}."));
    }
}