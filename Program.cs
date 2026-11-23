using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AISmokingDetection.API.Business.Services;
using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.Business.Services;
using AISmokingDetection.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();
builder.Services.AddScoped<IEvidenceService, EvidenceService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AIsmokingCorsPolicy",
        Policy =>
        {
            Policy.
            WithOrigins("https://localhost:7288","http://localhost:5129").
            AllowAnyHeader().
            AllowAnyMethod();
        });
});
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
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();



}

app.UseHttpsRedirection();
app.UseCors("AIsmokingCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
