using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<GameStoreContext>(); //retrieving instance of database context

        dbContext.Database.Migrate(); // accessing the database using the database instance and executes the migration
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStore"); // fetching connection string from appsettings.json

        // DbContext has a Scoped service lifetime because :
       //  1. It ensures that a new instance of DbContext is created per request
       //  2. DB connections are a limited and expensive resource
       //  3. DbContext is not thhread-safe. Scoped avoids to concurrency issues
       //  4. Makes it easier to manage transcations and ensure data consistency.
       //  5. Reusing a DbContext instance can lead to increased memory usuage.
        builder.Services.AddScoped<GameStoreContext>();

        builder.Services.AddSqlite<GameStoreContext>(
            connString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Fighting" },
                        new Genre { Name = "RPG" },
                        new Genre { Name = "Platformer" },
                        new Genre { Name = "Racing" },
                        new Genre { Name = "Sports" }
                    );

                    context.SaveChanges();
                }
            })
        );
    }
}
