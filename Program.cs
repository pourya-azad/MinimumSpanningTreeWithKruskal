using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MinimumSpanningTreeWithKruskal.Models;
using MinimumSpanningTreeWithKruskal.Interfaces;
// Add the required NuGet package reference for EF Core In-Memory Database:
// Microsoft.EntityFrameworkCore.InMemory

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GraphDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<GraphDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IGraphService, MinimumSpanningTreeWithKruskal.Services.GraphService>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IGraphValidator, MinimumSpanningTreeWithKruskal.Services.GraphValidator>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IMSTAlgorithm, MinimumSpanningTreeWithKruskal.Services.KruskalMSTAlgorithm>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IGraphRepository, MinimumSpanningTreeWithKruskal.Services.GraphRepository>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IMSTRepository, MinimumSpanningTreeWithKruskal.Services.MSTRepository>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IGraphInputHandlerService, MinimumSpanningTreeWithKruskal.Services.GraphInputHandlerService>();
builder.Services.AddScoped<MinimumSpanningTreeWithKruskal.Interfaces.IGraphPersistenceService, MinimumSpanningTreeWithKruskal.Services.GraphPersistenceService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Graph}/{action=Index}/{id?}");

app.Run();
