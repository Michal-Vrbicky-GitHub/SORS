using SORS.Data;
using SORS.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SORS.Services
{
    public class StationMonitor
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<StationMonitor> _logger;

        public StationMonitor(ApplicationDbContext context, IEmailSender emailSender, ILogger<StationMonitor> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task CheckStationReports()
        {
            var reports = await _context.Report.ToListAsync();
            foreach (var report in reports)
            {
                var station = await _context.Stations.FindAsync(report.StationId);
                if (station != null)
                {
                    if (report.Value < station.LvlMin || report.Value > station.LvlMax)
                    {
                        var subject = $"Alert: Station {station.Name} Value Out of Range";
                        var body = $"The value {report.Value} for station {station.Name} is out of range ({station.LvlMin} - {station.LvlMax}).";
                        var emails = station.AlertEmails?.Select(ae => ae.Email).ToList() ?? new List<string>();

                        foreach (var email in emails)
                        {
                            await _emailSender.SendEmailAsync(email, subject, body);
                        }
                    }
                }
            }
        }
    }
}