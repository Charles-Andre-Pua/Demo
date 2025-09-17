using Demo.Configurations;
using Demo.Context;
using Demo.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDBContext>(options =>
    options
        .UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:Default").Value,
            sql => sql.EnableRetryOnFailure())
        .EnableSensitiveDataLogging(),
    ServiceLifetime.Transient
);
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));
builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHangfireServer();

builder.Services.AddScoped<EmailService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "MyCookieAuth";
    options.DefaultSignInScheme = "MyCookieAuth";
    options.DefaultAuthenticateScheme = "MyCookieAuth";
})
.AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // must come before UseAuthorization
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

app.Run();
