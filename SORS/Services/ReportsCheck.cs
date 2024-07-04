using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SORS.Data;
using SORS.Data.Models;
using SORS.Services;

namespace SORS.Services
{
    public class ReportsCheck : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportsCheck> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check interval

        public ReportsCheck(IServiceProvider serviceProvider, ILogger<ReportsCheck> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckStationsAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Task was cancelled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking stations.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckStationsAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                var stations = await dbContext.Stations
                    .Include(s => s.AlertEmails)
                    .ToListAsync(stoppingToken);

                foreach (var station in stations)
                {
                    var alertDelayTime = DateTime.Now.AddMinutes(-station.AlertDelay);

                    var recentReports = await dbContext.Report
                        .Where(r => r.StationId == station.StationID && r.TimeStamp >= alertDelayTime)
                        .ToListAsync(stoppingToken);

                    if (!recentReports.Any())
                    {
                        if (!station.SentNoReporFromStationForAlertDelayTimeAlert)
                        {
                            var subject = $"Alert for Station {station.StationID}";
                            var body = $"No report received from {station.Name}, station {station.StationID} in the last {station.AlertDelay} minutes.";

                            foreach (var alertEmail in station.AlertEmails)
                            {
                                await emailSender.SendEmailAsync(alertEmail.Email, subject, body);
                            }

                            _logger.LogWarning(body);
                            station.SentNoReporFromStationForAlertDelayTimeAlert = true;
                            await dbContext.SaveChangesAsync(stoppingToken);
                        }
                    }
                    else
                    {
                        var allReportsOutOfRange = recentReports.All(r => r.Value < station.LvlMin || r.Value > station.LvlMax);

                        if (allReportsOutOfRange && !station.SentNoReporFromStationForAlertDelayTimeAlert)
                        {
                            string lessOrMore = "over";
                            if (recentReports.First().Value < station.LvlMin)
                                lessOrMore = "below";

                            var subject = $"Alert for Station {station.Name}";
                            var body = $"Station {station.StationID} reported values {lessOrMore} range during the last {station.AlertDelay} minutes.\n" +
                                $"Station is on {station.Name}, range is {station.LvlMin} to {station.LvlMax}";

                            foreach (var alertEmail in station.AlertEmails)
                            {
                                await emailSender.SendEmailAsync(alertEmail.Email, subject, body);
                            }

                            _logger.LogWarning(body);
                            station.SentNoReporFromStationForAlertDelayTimeAlert = true;//change to false, if want to receive email every check cycle
                        }
                        else
                        {
                            station.SentNoReporFromStationForAlertDelayTimeAlert = false;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
            }
        }

    }
}
