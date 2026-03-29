namespace GameStore.Api.DTOs;

// A DTO(Data Transfer Object) is a contract between the client and server since it represents a shared agreement about how data will be transferred and used.

public record GameSummaryDTO(
    int Id,
    string Name,
    string Genre,
    decimal PriceDecimal,
    DateOnly ReleaseDate
);
