namespace GameStore.Api.DTOs;

public record GameDetailsDTO(
    int Id,
    string Name,
    int GenreId,
    decimal PriceDecimal,
    DateOnly ReleaseDate
);
