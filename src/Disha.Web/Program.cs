using System;
using Disha.Web.Data;
using Disha.Web.Services;
using Disha.Web.Services.AiMentor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MySQL Database via EF Core Pomelo Provider
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=127.0.0.1;Port=3306;Database=disha_db;Uid=root;Pwd=;";

builder.Services.AddDbContext<DishaDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "Disha.AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Relaxed for local HTTP development
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
    });

// Register HTTP Client for AI Providers
builder.Services.AddHttpClient<GroqMentorProvider>();
builder.Services.AddHttpClient<GeminiMentorProvider>();

// Register Application Services
builder.Services.AddScoped<IRecommendationService, RecommendationService>();
builder.Services.AddScoped<IAidMatchService, AidMatchService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IAiMentorService, AiMentorService>();

var app = builder.Build();

// Seed Database automatically on startup if empty
await DbSeeder.SeedDatabaseAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
