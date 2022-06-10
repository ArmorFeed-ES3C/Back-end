
using ArmorFeedApi.Shared.Domain.Repositories;
using ArmorFeedApi.Shared.Mapping;
using ArmorFeedApi.Shared.Persistence.Contexts;
using ArmorFeedApi.Shared.Persistence.Repositories;
using ArmorFeedApi.Shipments.Domain.Repositories;
using ArmorFeedApi.Shipments.Domain.Services;
using ArmorFeedApi.Shipments.Persistence.Repositories;
using ArmorFeedApi.Shipments.Services;
using ArmorFeedApi.Vehicles.Domain.Repositories;
using ArmorFeedApi.Vehicles.Domain.Services;
using ArmorFeedApi.Vehicles.Persistence.Repositories;
using ArmorFeedApi.Vehicles.Services;
using Microsoft.EntityFrameworkCore;


using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        // Add API Documentation Information
        
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ArmorFeed API",
            Description = "ArmorFeed RESTful API",
            
        });
        options.EnableAnnotations();
    });

// Add Database Connection

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Database Connection with Sensitive and Detailed Information and Errors enabled
//
// builder.Services.AddDbContext<AppDbContext>(
//     options => options.UseMySQL(connectionString)
//         .LogTo(Console.WriteLine, LogLevel.Information)
//         .EnableSensitiveDataLogging()
//         .EnableDetailedErrors());

// Database Connection with Standard Level for Information and Errors

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString != null) options.UseMySQL(connectionString);
});

// Add lowercase routes

builder.Services.AddRouting(options => 
    options.LowercaseUrls = true);

// Dependency Injection Configuration ArmorFeed

builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IShipmentReviewRepository, ShipmentReviewRepository>();
builder.Services.AddScoped<IShipmentReviewService, ShipmentReviewService>();
builder.Services.AddScoped<IEnterpriseRepository, EnterpriseRepository>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
// AutoMapper Configuration

builder.Services.AddAutoMapper(
    typeof(ModelToResourceProfile),
    typeof(ResourceToModelProfile));


var app = builder.Build();

// Validation for ensuring Database Objects are created

using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    context.Database.EnsureCreated();
}


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();