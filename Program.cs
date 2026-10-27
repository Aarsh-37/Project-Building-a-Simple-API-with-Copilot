using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Centralized exception handling; returns a safe Problem Details response.
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Simple request logging middleware.
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();
app.Run();
