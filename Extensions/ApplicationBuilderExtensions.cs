﻿using Lms.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Lms.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedDataAsync(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var db = serviceProvider.GetRequiredService<LmsApiContext>();

                //db.Database.EnsureDeleted(); // Om en ny seedning ska göras vid varje uppstart.
                //db.Database.Migrate();
                //db.Database.EnsureCreated();

                try
                {
                    await SeedData.InitAsync(db);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

        }
    }
}
