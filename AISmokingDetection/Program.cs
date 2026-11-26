using Amazon.Runtime;
using Amazon.S3;
using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Business.Services;
using AISmokingDetection.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

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

            builder.Services.AddScoped<IIncidentService, IncidentService>();
            builder.Services.AddScoped<IEvidenceService, EvidenceService>();
            builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IFaceReferenceImageService, FaceReferenceImageService>();

            var storageProvider = builder.Configuration["Storage:Provider"] ?? "Local";

            if (storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
            }
            else if (storageProvider.Equals("R2", StringComparison.OrdinalIgnoreCase))
            {
                var serviceUrl = builder.Configuration["CloudflareR2:ServiceUrl"];
                var accessKey = builder.Configuration["CloudflareR2:AccessKey"];
                var secretKey = builder.Configuration["CloudflareR2:SecretKey"];
                var bucketName = builder.Configuration["CloudflareR2:BucketName"];

                if (string.IsNullOrWhiteSpace(serviceUrl) ||
                    string.IsNullOrWhiteSpace(accessKey) ||
                    string.IsNullOrWhiteSpace(secretKey) ||
                    string.IsNullOrWhiteSpace(bucketName))
                {
                    throw new InvalidOperationException(
                        "Cloudflare R2 configuration is incomplete.");
                }

                builder.Services.AddSingleton<IAmazonS3>(_ =>
                    new AmazonS3Client(
                        new BasicAWSCredentials(accessKey, secretKey),
                        new AmazonS3Config
                        {
                            ServiceURL = serviceUrl,
                            ForcePathStyle = true,
                            AuthenticationRegion = "auto"
                        }));

                builder.Services.AddScoped<IFileStorageService, R2FileStorageService>();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Unsupported storage provider: {storageProvider}");
            }
            var jwtKey = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(jwtIssuer) ||
                string.IsNullOrWhiteSpace(jwtAudience))
            {
                throw new InvalidOperationException(
                    "JWT configuration is missing. Check User Secrets.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Convert.FromBase64String(jwtKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AISmokingDetection API",
                    Version = "v1",
                    Description = "API documentation for AISmokingDetection"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

                options.AddSecurityRequirement(document =>
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "AISmokingDetection API v1");

                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
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
