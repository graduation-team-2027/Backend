
using AISmokingDetection.Data;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new
            {
                status = "Healthy",
                service = "AISmokingDetection"
            }));

            app.Lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("AISmokingDetection started.");
            });

            await using (var scope = app.Services.CreateAsyncScope())
            {
                var context = scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                await DbInitializer.SeedAsync(context);
            }

            await app.RunAsync();
        }
    }
}
