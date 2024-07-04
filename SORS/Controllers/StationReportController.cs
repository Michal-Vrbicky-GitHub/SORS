using Microsoft.AspNetCore.Mvc;
using SORS.Data.Models;
using Microsoft.Extensions.Logging;
using SORS.Data;
using System;
using System.Linq;
using static System.Collections.Specialized.BitVector32;
using SORS.Services;

namespace SORS.Controllers
{
    [ApiController]
    [Route("api")]
    public class StationReportController : Controller
    {
        private readonly ILogger<StationReportController> _logger;
        private readonly ApplicationDbContext _contextReports;
        private readonly ApplicationDbContext _contextStations;

        public StationReportController(ILogger<StationReportController> logger, ApplicationDbContext contextReports, ApplicationDbContext contextStations)
        {
            _logger = logger;
            _contextReports = contextReports;
            _contextStations = contextStations;
        }

        [HttpGet("get-stations")]
        public IActionResult GetListOfStations()
        {
            var stations = _contextStations.Stations.ToList();
            return Ok(stations);
        }

        [HttpPost("add-station")]
        public IActionResult AddStation(Station station)
        {
            if (_contextStations.Stations.Any(x => x.StationID.Equals(station.StationID)))//name can be same
            {
                return BadRequest("Duplicities are not allowed.");
            }

            _contextStations.Stations.Add(station);
            _contextStations.SaveChanges();

            return Ok("Station has been successfully added.");
        }

        [HttpPost("add-report")]
		public IActionResult AddReport(StationReport report)
		{
			if (_contextReports.Report.Any(x => x.Id == report.Id))
			{
				// If the report ID already exists, write the incoming JSON to the log file
				LogReport(report, "Duplicate report ID detected.");
				return BadRequest("Duplicate report ID is not allowed.");
			}

			if (!_contextStations.Stations.Any(s => s.StationID == report.StationId))
			{
				// If the StationId does not exist, log the report and return an error
				LogReport(report, $"Station ID {report.StationId} not found.");
				return BadRequest($"Station ID {report.StationId} does not exist.");
			}
			var station = _contextStations.Stations.FirstOrDefault(s => s.StationID == report.StationId);
			if (station.Token != report.Token)
			{
				// If the token does not match, log the report and return an error
				LogReport(report, $"Invalid token for Station ID {report.StationId}.");
				return BadRequest("Invalid token.");
            }

            if (report.Value < 1 || report.Value > 100)
            {
                LogReport(report, $"Invalid report value for Station ID {report.StationId}. Value must be between 1 and 100.");
                return BadRequest("Invalid report value. Value must be between 1 and 100.");
            }

            _contextReports.Report.Add(report);
			_contextReports.SaveChanges();

			return Ok("Report has been successfully added.");
		}

        private void LogReport(StationReport report, string message)
		{
			string logMessage = $"Time: {DateTime.Now}\n{message}\nJSON content:\n{report.ToJson()}\n";
			string logFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "log.txt");
			if (!System.IO.File.Exists(logFilePath))
			{
				using (StreamWriter writer = System.IO.File.CreateText(logFilePath))
				{
					writer.WriteLine(logMessage);
				}
			}
			else
			{
				using (StreamWriter writer = System.IO.File.AppendText(logFilePath))
				{
					writer.WriteLine(logMessage);
				}
			}
		}

        [HttpPost("list")]
        public IActionResult GetWaterMeasures(FilterDto filter)
        {
            var waterMeasures = Enumerable.Range(1, 5).Select(index => new WaterMeasure
            {
                //In
            }).Where(x => x.Summary.StartsWith(filter.prefix)).ToArray();

            return Ok(waterMeasures);
        }

        [HttpGet("get-report")]
        public IActionResult GetListOfReports()
        {
            var reports = _contextReports.Report.ToList();
            return Ok(reports);
        }
    }/*
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IEmailSender _emailSender;

        public TestController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet("send-email")]
        public async Task<IActionResult> SendEmail()
        {
            await _emailSender.SendEmailAsync("recipient@example.com", "Test Email", "This is a test email.");
            return Ok("Email sent successfully.");
        }
    }*/
}
