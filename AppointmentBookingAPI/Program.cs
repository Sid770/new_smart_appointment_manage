using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Azure.Data.Tables;
using AppointmentBookingAPI.Data;
using AppointmentBookingAPI.Repositories;
using AppointmentBookingAPI.Repositories.Azure;
using AppointmentBookingAPI.Services;
using AppointmentBookingAPI.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Determine which storage to use based on environment
var useAzureStorage = !string.IsNullOrEmpty(builder.Configuration["AzureTableStorage:ConnectionString"]);

if (useAzureStorage)
{
    // Azure Table Storage Configuration (Production)
    var connectionString = builder.Configuration["AzureTableStorage:ConnectionString"]!;
    builder.Services.AddSingleton(new TableServiceClient(connectionString));
    
    // Register Azure Repositories
    builder.Services.AddScoped<ITimeSlotRepository, AzureTimeSlotRepository>();
    builder.Services.AddScoped<IAppointmentRepository, AzureAppointmentRepository>();
    
    builder.Logging.AddConsole();
    Console.WriteLine("✅ Using Azure Table Storage (Production Mode)");
}
else
{
    // SQLite Configuration (Local Development)
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=AppointmentBooking.db"));
    
    // Register SQLite Repositories
    builder.Services.AddScoped<ITimeSlotRepository, TimeSlotRepository>();
    builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
    
    builder.Logging.AddConsole();
    Console.WriteLine("✅ Using SQLite Database (Local Development Mode)");
}

// Register Services
builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// Configure CORS - Allow all origins in production, specific origin in development
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.AddPolicy("AllowAngularApp",
            policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    }
    else
    {
        options.AddPolicy("AllowAngularApp",
            policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
    }
});

// Configure Swagger/OpenAPI - Enable in all environments
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Appointment Booking API",
        Version = "v1",
        Description = "API for managing appointments and time slots - Smart Appointment Booking System",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Support Team",
            Email = "support@appointmentbooking.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database based on storage type
if (!useAzureStorage)
{
    // Only seed SQLite database for local development
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
    }
}

// Configure the HTTP request pipeline
// Enable Swagger in all environments (Development & Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appointment Booking API v1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
    c.DocumentTitle = "Appointment Booking API Documentation";
});

// Use custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

var environment = app.Environment.EnvironmentName;
var storageType = useAzureStorage ? "Azure Table Storage" : "SQLite";

app.Logger.LogInformation("🚀 Appointment Booking API is starting...");
app.Logger.LogInformation($"🌍 Environment: {environment}");
app.Logger.LogInformation($"💾 Storage: {storageType}");
app.Logger.LogInformation("📚 Swagger UI available at: /swagger");
app.Logger.LogInformation("🔗 API Base URL: /api");

app.Run();
