using System.Text.Json;
using static System.Console;

public static class ConfigSettings
{
    private static readonly ConsoleColor foregroundColor = ForegroundColor;

    public static void SetValue<T>(T data, Config config)
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
        File.WriteAllText("appsettings.json", appSettingsJson);
    }
}