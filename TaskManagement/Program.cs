using Serilog;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Abstractions;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); // replace built-in logging

builder.Services.AddControllersWithViews();

// DI
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Add services to the container.
builder.Services.AddRazorPages();

// Global anti-forgery for all unsafe HTTP methods
builder.Services.AddControllersWithViews(opts =>
{
    opts.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

var app = builder.Build();

// Friendly status-code pages (404, 403, 500...) -> /Error/Status/{code}
app.UseStatusCodePagesWithReExecute("/Error/Status/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

try
{
    Log.Information("Starting up TaskManagement app");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}