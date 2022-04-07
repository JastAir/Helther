using Helther.Server.Services;
using Helther.Shared.Entity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AppContext = Helther.Shared.Db.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppContext>(ServiceLifetime.Transient);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IPingService, PingService>();
builder.Services.AddHostedService<PingHostedService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
