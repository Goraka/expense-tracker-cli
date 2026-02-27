using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using static System.Console;

public static class ConfigSettings
{
    private static readonly ConsoleColor foregroundColor = ForegroundColor;
    // private static readonly string _currentDirectory = Directory.GetCurrentDirectory();

    public static void SetValue<T>(T data, Config config, string path)
    {
        var configItem = config.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(T));

        if (configItem != null)
        {
            configItem.SetValue(config, data);
        }
        else
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"No matching configuration item found for type {typeof(T).Name}");
            ForegroundColor = foregroundColor;
            return;
        }

        var appSettingsJson = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        var appSettingsPath = Path.Combine(path, "appsettings.json");
        File.WriteAllText(appSettingsPath, appSettingsJson);
    }

    public static Config GetConfigurations(string path)
    {
        // Config config = new ConfigurationBuilder()
        //                 .SetBasePath(Directory.GetCurrentDirectory())
        //                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //                 .Build()
        //                 .Get<Config>() ?? new Config();

        var appSettingsPath = Path.Combine(path, "appsettings.json");
        var res = File.ReadAllText(appSettingsPath);

        var appSettingsJson = JsonSerializer.Deserialize<Config>(res);

        return appSettingsJson;
    }
}