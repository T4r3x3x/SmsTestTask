using Sms.Shared.Configuration;
using Sms.WpfApp.Features.EnvironmentVariables;

namespace Sms.WpfApp.Tests;

public sealed class AppSettingsTests
{
    [Fact]
    public void Load_ReadsEnvironmentVariableConfiguration()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(
                path,
                """
                {
                  "EnvironmentVariables": ["SMS_URL"],
                  "Defaults": { "SMS_URL": "https://localhost" },
                  "Comments": { "SMS_URL": "Endpoint" },
                  "SensitiveVariables": ["SMS_PASSWORD"]
                }
                """);

            var settings = JsonSettingsLoader.Load<AppSettings>(path);

            Assert.Equal("SMS_URL", Assert.Single(settings.EnvironmentVariables));
            Assert.Equal("https://localhost", settings.Defaults["SMS_URL"]);
            Assert.Equal("Endpoint", settings.Comments["SMS_URL"]);
            Assert.Contains("SMS_PASSWORD", settings.SensitiveVariables);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
