namespace Application.DTOs.Responses;

public record StartAuctionResponse(MoneyDto StartingBid, 
                                   DateTime StartTime, 
                                   Guid AuctionId, 
                                   VehicleResponse Vehicle);