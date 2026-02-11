using System.Text;

namespace HastaksharSewaAnalytics.Presentation.Logging;

public static class ErrorLog
{
    private static readonly object _lock = new object();

    // Call this from Program.cs once
    public static IWebHostEnvironment? Env { get; set; }

    public static void LogErrorToFile(Exception ex, string extra = null!)
    {
        try
        {
            var basePath = Env?.ContentRootPath
                           ?? AppContext.BaseDirectory;

            var logDir = Path.Combine(basePath, "Logs", "Errors");
            Directory.CreateDirectory(logDir);

            var filePath = Path.Combine(
                logDir,
                $"error_{DateTime.Now:yyyy-MM-dd}.log"
            );

            var sb = new StringBuilder();
            sb.AppendLine("==================================================");
            sb.AppendLine($"UTC Time : {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Local    : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            if (!string.IsNullOrWhiteSpace(extra))
                sb.AppendLine($"Context  : {extra}");

            AppendException(sb, ex);

            lock (_lock)
            {
                File.AppendAllText(filePath, sb.ToString());
            }
        }
        catch
        {
            // never throw from logger
        }
    }

    private static void AppendException(StringBuilder sb, Exception ex, int depth = 0)
    {
        if (ex == null) return;

        sb.AppendLine($"[#{depth}] {ex.GetType().FullName}");
        sb.AppendLine($"Message  : {ex.Message}");
        sb.AppendLine("Stack    :");
        sb.AppendLine(ex.StackTrace);

        if (ex.InnerException != null)
        {
            sb.AppendLine("---- INNER EXCEPTION ----");
            AppendException(sb, ex.InnerException, depth + 1);
        }
    }
}
