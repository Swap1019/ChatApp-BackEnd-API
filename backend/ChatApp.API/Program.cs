using ChatApp.Application.Abstractions.Persistence;
using ChatApp.Application.Abstractions.Time;
using ChatApp.Application.Authorization.DependencyInjection;
using ChatApp.Application.Services;
using ChatApp.API.Middleware;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Services;
using ChatApp.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("ChatApp.API")));

builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddScoped<IUserStateService, UserStateService>();

// Register authorization policies
builder.Services.AddConversationAuthorization();
builder.Services.AddMessageAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<UserStateMiddleware>();
app.UseAuthorization();

app.Run();
