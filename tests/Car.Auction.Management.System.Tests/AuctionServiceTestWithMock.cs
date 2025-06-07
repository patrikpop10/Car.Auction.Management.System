using System.Threading.Channels;
using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Services;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Car.Auction.Management.System;

[TestFixture]
public class AuctionServiceTestsWithMock {
    private AuctionService _service;
    private IVehicleRepository _vehicleRepo;
    private IAuctionRepository _auctionRepo;
    private VehicleService _vehicleService;
    private ILogger<AuctionService> _logger;
    private ILogger<VehicleService> _loggerVehicle;
    private ChannelWriter<AuctionMonitoringResponse> _channelWriter;

    [SetUp]
    public void Setup() {
        _vehicleRepo = Substitute.For<IVehicleRepository>();
        _auctionRepo = Substitute.For<IAuctionRepository>();
        _logger = Substitute.For<ILogger<AuctionService>>();
        _loggerVehicle = Substitute.For<ILogger<VehicleService>>();
        _channelWriter = Substitute.For<ChannelWriter<AuctionMonitoringResponse>>();

        _service = new AuctionService(_vehicleRepo, _auctionRepo, _channelWriter, _logger);
        _vehicleService = new VehicleService(_vehicleRepo, _auctionRepo, _loggerVehicle);
    }

    [Test]
    public async Task AddVehicle_ShouldAddSuccessfully() {
        var vehicle = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Corolla", 2022, new Money(5000, CurrencyType.USD), 4);

        _vehicleRepo.Exists(vehicle.Id).Returns(false);

        var result = await _vehicleService.AddVehicle(vehicle);

        Assert.That(result.IsSuccess, Is.True);
        await _vehicleRepo.Received(1).Add(vehicle);
    }

    [Test]
    public async Task AddVehicle_DuplicateId_ReturnsError() {
        var id = new VehicleId(Guid.NewGuid());
        var vehicle2 = new Hatchback(id, "Honda", "Fit", 2023, new Money(3000, CurrencyType.USD), 5);

        _vehicleRepo.Exists(id).Returns(true);

        var result = await _vehicleService.AddVehicle(vehicle2);

        Assert.That(result.Problem!.Status, Is.EqualTo(409));
        Assert.That(result.Problem.Title, Is.EqualTo("DuplicateVehicle"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"A vehicle with the ID {id.Id} already exists."));
    }

    [Test]
    public async Task SearchVehicles_ByTypeAndManufacturer_ReturnsCorrectResults() {
        var v2 = new Hatchback(new VehicleId(Guid.NewGuid()), "Honda", "Fit", 2023, new Money(3000, CurrencyType.USD), 5);

        _vehicleRepo.Search("Hatchback", "Honda").Returns(new[] { v2 });
        _auctionRepo.IsAuctionForVehicleActive(v2.Id).Returns(true);

        var results = await _vehicleService.SearchVehicles(type: "Hatchback", manufacturer: "Honda");

        var vehicleRequests = results.ToList();
        Assert.That(vehicleRequests, Has.Count.EqualTo(1));
        Assert.That(vehicleRequests.First().Vehicle.Model, Is.EqualTo("Fit"));
    }

    [Test]
    public async Task StartAuction_Success() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.IsAuctionForVehicleActive(v.Id).Returns(false);

        var result = await _service.StartAuction(v.Id);

        Assert.That(result.IsSuccess, Is.True);
        await _auctionRepo.Received(1).Add(Arg.Is<Domain.Entities.Auction.Auction>(a => a.VehicleId == v.Id));
    }

    [Test]
    public async Task StartAuction_VehicleNotFound_ReturnsError() {
        var vehicleId = new VehicleId(Guid.NewGuid());

        _vehicleRepo.GetById(vehicleId).Returns((Vehicle)null!);

        var result = await _service.StartAuction(vehicleId);

        Assert.That(result.Problem!.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("NotFound"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"The requested vehicle with Id {vehicleId} was not found."));
    }

    [Test]
    public async Task StartAuction_AlreadyActive_ReturnsError() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.IsAuctionForVehicleActive(v.Id).Returns(true);

        var result = await _service.StartAuction(v.Id);

        Assert.That(result.Problem!.Status, Is.EqualTo(409));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionAlreadyActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"An auction is already active for the vehicle with ID {v.Id.Id}."));
    }

    [Test]
    public async Task PlaceBid_Success() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        var auction = new Domain.Entities.Auction.Auction(v.Id);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.GetActiveByVehicleId(v.Id).Returns(auction);

        var bidDto1 = new BidRequest("Alice", new MoneyDto { Amount = 5000, Currency = "USD" });
        var result = await _service.PlaceBid(bidDto1, v.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(auction.Bids.Count, Is.EqualTo(1));
        Assert.That(auction.Bids.First().Bidder, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task PlaceBid_BelowCurrentHighest_ReturnsError() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        var auction = new Domain.Entities.Auction.Auction(v.Id);
        auction.PlaceBid(new Bid("Alice", new Money(5000, CurrencyType.USD)), v);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.GetActiveByVehicleId(v.Id).Returns(auction);

        var bidDto2 = new BidRequest("Bob", new MoneyDto { Amount = 5000, Currency = "USD" });
        var result = await _service.PlaceBid(bidDto2, v.Id);

        Assert.That(result.Problem!.Status, Is.EqualTo(400));
        Assert.That(result.Problem.Title, Is.EqualTo("InvalidBidAmount"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo("The bid amount must be greater than the current highest bid."));
    }

    [Test]
    public async Task PlaceBid_NoActiveAuction_ReturnsError() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.GetActiveByVehicleId(v.Id).Returns((Domain.Entities.Auction.Auction)null!);

        var bid = new BidRequest("Bob", new MoneyDto { Amount = 5000, Currency = "USD" });

        var result = await _service.PlaceBid(bid, v.Id);

        Assert.That(result.Problem!.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionNotActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"No active auction found for the vehicle with ID {v.Id.Id}."));
    }

    [Test]
    public async Task CloseAuction_Success() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);
        var auction = new Domain.Entities.Auction.Auction(v.Id);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.GetActiveByVehicleId(v.Id).Returns(auction);

        var result = await _service.CloseAuction(v.Id);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(auction.IsActive, Is.False);
    }

    [Test]
    public async Task CloseAuction_NoActiveAuction_ReturnsError() {
        var v = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2022, new Money(5000, CurrencyType.USD), 4);

        _vehicleRepo.GetById(v.Id).Returns(v);
        _auctionRepo.GetActiveByVehicleId(v.Id).Returns((Domain.Entities.Auction.Auction)null!);

        var result = await _service.CloseAuction(v.Id);

        Assert.That(result.Problem!.Status, Is.EqualTo(404));
        Assert.That(result.Problem.Title, Is.EqualTo("AuctionNotActive"));
        Assert.That(result.Problem.ErrorMessage, Is.EqualTo($"No active auction found for the vehicle with ID {v.Id.Id}."));
    }
}