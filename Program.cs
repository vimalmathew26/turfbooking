using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using turfbooking.Data;
using turfbooking.Helper;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

var emailSettings = builder.Configuration.GetSection("EmailSettings").Get<EmailSettings>();

var smtpClient = new SmtpClient(emailSettings.Host)
{
    Port = emailSettings.Port,
    Credentials = new System.Net.NetworkCredential(emailSettings.Username, emailSettings.Password),
    EnableSsl = true
};

builder.Services
    .AddFluentEmail(emailSettings.Username, "TurfBooking Admin")
    .AddRazorRenderer()
    .AddSmtpSender(smtpClient);

builder.Services.AddTransient<SendMail>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DefaultSlots>();



builder.Services.AddAuthentication("UserAuth")
    .AddCookie("UserAuth", options =>
    {
        options.LoginPath = "/Accounts/Login";
        options.AccessDeniedPath = "/Accounts/Login";
    });

builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddSession();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/", context =>
{
    context.Response.Redirect("/Accounts/Login");
    return Task.CompletedTask;
});


app.MapDefaultControllerRoute();
app.UseSession();


app.Run();