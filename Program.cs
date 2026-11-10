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
    options.AddPolicy("AIsomkingCorsPolicy",
        Policy =>
        {
            Policy.
            WithOrigins("https://localhost:7288","http://localhost:5129").
            AllowAnyHeader().
            AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();



}

app.UseHttpsRedirection();
app.UseCors("AIsmokingCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
