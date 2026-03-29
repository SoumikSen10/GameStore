using GameStore.Api.Data;
using GameStore.Api.DTOs;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    /* private static readonly List<GameSummaryDTO> games =
    [
        new(
            1,
            "Street Fighter II",
            "Fighting",
            19.99M,
            new DateOnly(1992, 7, 15)),
        new(
            2,
            "Final Fantasy VII Rebirth",
            "RPG",
            69.99M,
            new DateOnly(2024, 2, 29)),
        new(
            3,
            "Astro Bot",
            "Platformer",
            59.99M,
            new DateOnly(2024, 9, 6)),
    ]; */

    public static void MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("/games");

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) 
            => await dbContext.Games
                              .Include(game => game.Genre)
                              .Select(game => new GameSummaryDTO(
                                game.Id,
                                game.Name,
                                game.Genre!.Name,
                                game.Price,
                                game.ReleaseDate
                              ))
                              .AsNoTracking()
                              .ToListAsync());


        // GET /games/1
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            /* var game = games.Find(game => game.Id == id); */

            var game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound() : Results.Ok(new GameDetailsDTO(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate            
            ));

        }).WithName(GetGameEndpointName);

        

        // POST /games
        group.MapPost("/", async (CreateGameDTO newGame,GameStoreContext dbContext) =>
        {

            if (string.IsNullOrEmpty(newGame.Name))
            {
                return Results.BadRequest("Name is required");
            }

            /* GameDTO game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            ); 
            games.Add(game); */

            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate 
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDTO gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );
   

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
        });

        

        // PUT /games/1
        group.MapPut("/{id}", async (int id, UpdateGameDTO updatedGame, GameStoreContext dbContext) =>
        {
            /* var index = games.FindIndex(game => game.Id == id); */

            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            /* games[index] = new GameSummaryDTO(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            ); */

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });


        
        // DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext DbContext) =>
        {
            /* games.RemoveAll(game => game.Id == id); */

            await DbContext.Games
                              .Where(game => game.Id == id)
                              .ExecuteDeleteAsync();

            return Results.NoContent();
        });
    }
}
