using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SORS.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ReportCleanupService : BackgroundService
{
	private readonly IServiceProvider _services;

	public ReportCleanupService(IServiceProvider services)
	{
		_services = services;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await DoWork(stoppingToken);
	}

	private async Task DoWork(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using (var scope = _services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //Determine cutoff date (7 days ago)
                //var cutoffDate = DateTime.Now.AddDays(-7);

                //Determine cutoff date (42s ago)
                //var cutoffDate = DateTime.Now.AddSeconds(-42);
                //var cutoffDate = DateTime.Now.AddMinutes(-1);
                //var cutoffDate = DateTime.Now.AddMinutes(-88);
                var cutoffDate = DateTime.Now.AddDays(-7);

                // Retrieve reports older than 7 days
                var reportsToDelete = dbContext.Report.Where(r => r.TimeStamp < cutoffDate).ToList();

				// Delete the reports
				dbContext.Report.RemoveRange(reportsToDelete);
				await dbContext.SaveChangesAsync(stoppingToken);
			}//
			//
		   //  Delay before next iteration
		  //await Task.Delay(TimeSpan.FromDays(1),    stoppingToken); // Check every day
		 // await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // Check every 2 minutes
			await Task.Delay(TimeSpan.FromSeconds(20),stoppingToken); // Check every 20s
		}
	}
}
