# Project Implementation Overview: Car.Auction.Management.System

## Architectural Layers

The project is organized into four primary layers:
- **Domain Layer:** Contains core business logic and entities (e.g., `Auction`, `Vehicle`), as well as shared value objects and domain services.
- **Application Layer:** Implements use cases and orchestrates business processes through services (such as `AuctionService`). It defines interfaces (`IAuctionService`) and DTOs for requests and responses.
- **Infrastructure Layer:** Provides concrete implementations for repository interfaces and handles data access, persistence, and integration with external systems.
- **API Layer:** Exposes endpoints using minimal APIs, handles HTTP requests, validation, and translates domain/application results into HTTP responses.

---

## Key Design Decisions

### 1. Use of `Task<Result>` in Repositories

- **Pattern:** All repository interfaces (e.g., `IVehicleRepository`, `IAuctionRepository`) return `Task<Result<T>>` or `Task<Result>` for their methods.
- **Rationale:**
  - **Asynchronous Operations:** Using `Task` enables non-blocking, asynchronous operations, preparing for future scalability (e.g., swapping in a database).
  - **Unified Error Handling:** The `Result`/`Result<T>` class encapsulates both success and failure, along with error information (`Problem` object). This avoids exceptions for control flow and enables single-point error management.
  - **Testability & Consistency:** All operations follow a consistent pattern, making them easy to test and reason about.

#### Example
```csharp
public interface IVehicleRepository {
    Task<Result> Add(Vehicle vehicle);
    Task<Result<Vehicle>> GetById(VehicleId id);
    // ...
}
```

---

### 2. How the Result Pattern Supports the Liskov Substitution Principle (LSP)

The **Liskov Substitution Principle (LSP)** states that objects of a superclass (or interface) should be replaceable with objects of a subclass (or implementation) without affecting the correctness of the program.

- **Consistent Contract:** By enforcing all repository methods to return `Task<Result<T>>` or `Task<Result>`, both the interface and its implementations guarantee the same contract for success/failure and data access.
- **Safe Substitution:** Any implementation (e.g., in-memory, database, mock) can be substituted for another. The caller can always expect a `Result` and handle errors or successes in the same way, without worrying about exceptions or other side effects.
- **No Behavioral Surprises:** The `Result` pattern ensures that error information is consistently propagated through the return value, so swapping an in-memory repository for a database-backed one will not introduce unexpected behaviors such as throwing exceptions or returning nulls.
- **Extensibility:** New repository implementations can add additional logic (e.g., logging, caching, retry policies) without breaking the expected contract, as long as they keep returning `Task<Result<T>>`.

#### Example LSP-supporting scenario
```csharp
IVehicleRepository repo = new InMemoryVehicleRepository();
// Later swapped with
repo = new SqlVehicleRepository();
// Both support the same Result-based contract and can be used interchangeably.
```

---

### 3. Domain Layer Decisions

- **Domain Entities:** Entities such as `Vehicle`, `Auction` are rich domain models with their own invariants and behaviors.
- **Value Objects:** Types like `VehicleId` are used for strong typing and avoiding primitive obsession.
- **Result/Problem Pattern:** Domain operations return `Result` objects, promoting explicit handling of errors and successful outcomes.

---

### 4. Application Layer Decisions

- **Service Layer:** Implements orchestration logic, coordinates between repositories and domain logic, and ensures workflow correctness (e.g., starting or closing auctions).
- **DTOs & Validation:** Uses FluentValidation to validate requests before processing, ensuring that only valid data reaches the domain.
- **ChannelWriter:** For auction monitoring and events, a channel is used to stream auction changes (e.g., for SSE endpoints).

#### Example
```csharp
public class AuctionService : IAuctionService {
    // Coordinates auction start, close, and bid operations,
    // always returns Task<Result<TResponse>>
}
```

---

### 5. Infrastructure Layer Decisions

- **Repository Implementations:** The infrastructure layer provides in-memory implementations (`InMemoryAuctionRepository`, `InMemoryVehicleRepository`) of repository interfaces. These are designed for quick testing and prototyping without external dependencies.
- **Persistence Strategy:** Uses simple collections such as `List` and `Dictionary` to emulate persistent storage. Methods are implemented asynchronously to align with repository interfaces and to allow easy replacement with real database calls in the future.
- **Error Handling:** Matches the domain contract by returning `Result` objects, ensuring error propagation is consistent across the stack.
- **Dependency Injection:** Infrastructure services are registered in the API composition root, allowing seamless injection into application services.

#### Example
```csharp
public class InMemoryVehicleRepository : IVehicleRepository {
    private readonly Dictionary<VehicleId, Vehicle> _vehicles = new();

    public Task<Result> Add(Vehicle vehicle) {
        var result = _vehicles.TryAdd(vehicle.Id, vehicle);
        return Task.FromResult(result ? Result.Success() : Result.Failure(Problem.DuplicateVehicle(vehicle.Id)));
    }
    //...
}
```

---

## API Layer Decisions

- **Minimal API:** Uses minimal APIs for endpoints, grouping operations by resource (e.g., `/vehicles`, `/auctions`).
- **Result Translation:** The `ResultExtensions` class maps domain/application results into appropriate HTTP responses (`200 OK`, `400 Bad Request`, etc.) using the `ToApiResult()` extension methods.
- **Validation:** Integrates FluentValidation for request DTOs, returning validation errors before calling the application layer.

---

## Building and Running with Docker

### 1. Build the Docker Image

From the root directory of the repository, run:
```bash
docker build -t car-auction-management .
```

### 2. Run the Docker Container

To start the application and bind the API to port 5000 (adjust as needed):
```bash
docker run -p 5000:80 car-auction-management
```

- This maps port 80 inside the container to port 5000 on your host.
- The API will be available at `http://localhost:5000/`.

### 3. (Optional) Build and Run in One Command

```bash
docker build -t car-auction-management . && docker run -p 5000:80 car-auction-management
```

---

## Assumptions Made by the Code

- **In-Memory Repositories:** The current implementation uses in-memory collections (`List`, `Dictionary`) for persistence. It's assumed this will be swapped for a real database in production.
- **Unique IDs:** Vehicle and auction IDs are assumed to be unique and valid GUIDs.
- **Auction Rules:** Only one active auction per vehicle is allowed at a time.
- **Error Handling:** All errors are expected to be communicated via the `Problem` object in the `Result`, not through exceptions.
- **Validation:** It is assumed that all API input is validated at the API layer before reaching the application/domain logic.
- **Concurrency:** The in-memory repositories do not handle concurrent access or race conditions.

---

## Summary

The project is designed with clean architecture principles, separating concerns between API, application, infrastructure, and domain. The use of `Task<Result>` in repositories and services enables asynchronous, robust, and explicit error handling throughout the stack. The Result pattern specifically supports the Liskov Substitution Principle by ensuring all implementations follow a uniform and predictable contract, making the system easy to extend, test, and maintain.
