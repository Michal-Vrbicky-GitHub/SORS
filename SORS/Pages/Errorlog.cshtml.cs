using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace SORS.Pages
{
    public class ErrorlogModel : PageModel
    {
        private readonly string logFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "log.txt");

        public string LogContent { get; private set; }

        public void OnGet()
        {
            if (System.IO.File.Exists(logFilePath))
            {
                //LogContent = System.IO.File.ReadAllText(logFilePath).Replace(Environment.NewLine, "<br/>");
                string logContent = System.IO.File.ReadAllText(logFilePath);

                // Replace all occurrences of \r\n and \n with a common delimiter
                logContent = logContent.Replace("\r\n", "\n").Replace("\n\n", "\n\u2028\n");

                // Split based on the delimiter
                var logEntries = logContent.Split(new[] { "\n\u2028\n" }, StringSplitOptions.None).ToList();

                logEntries.Reverse();

                // Join the reversed list with double newlines and replace single newlines with <br/>
                LogContent = string.Join("\n\n", logEntries)
                                   .Replace("\n", "<br/>");
            }
            else
            {
                LogContent = "No errors to display.";
            }
        }
    }
}

