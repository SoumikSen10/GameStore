using GameStore.Api.Data;
using GameStore.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        // GET /genres
        group.MapGet("/", async (GameStoreContext dbContext) => await dbContext.Genres
            .Select(genre => new GenreDTO(genre.Id, genre.Name))
            .AsNoTracking()
            .ToListAsync()
        );
    }
}
