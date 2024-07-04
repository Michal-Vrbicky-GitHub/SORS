using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SORS.Data;
using SORS.Services;
using Microsoft.Extensions.Configuration;
using SORS.Controllers;
using Microsoft.Win32;
using System.Security.Principal;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
/*
builder.Services.AddHangfire(configuration =>
    configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));*/

//builder.Services.AddHangfireServer();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Identity/Pages/Account/Register");
    options.Conventions.AllowAnonymousToPage("/InvalidToken");
    options.Conventions.AllowAnonymousToPage("/Register");
    options.Conventions.AllowAnonymousToPage("/Privacy");
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
//builder.Services.AddRazorPages();//
builder.Services.AddTransient<IUsersService, UserService>();
builder.Services.AddTransient<IMeasureService, MeasureService>();
builder.Services.AddTransient<IStationsService, StationsService>();
//builder.Services.AddTransient<IStationService, StationService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<SORS.Services.IEmailSender, SORS.Services.EmailSender>();
builder.Services.AddHostedService<ReportCleanupService>();
builder.Services.AddHostedService<ReportsCheck>();
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoins =>
{
    endpoins.MapRazorPages();
    endpoins.MapControllers();//:D
});
app.UseSwagger();
app.UseSwaggerUI();

/*
// Use Hangfire dashboard
app.UseHangfireDashboard();
app.UseHangfireServer();
// Schedule a recurring job
RecurringJob.AddOrUpdate<StationMonitor>(x => x.CheckStationReports(), Cron.Minutely);
*/

app.Run();
