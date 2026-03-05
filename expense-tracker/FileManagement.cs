using System.Text;
using System.Text.Json;

public class FileManagement
{
    private readonly string _currentDirectory = Directory.GetCurrentDirectory();
    // private readonly string _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory();
    private readonly string _folderName = "ExpenseTrackerData";

    private readonly string _folderPath;

    public FileManagement()
    {
        _folderPath = Path.Combine(_currentDirectory, _folderName);
    }

    public async Task<T> CreateFile<T>(T model, string _fileName) where T : IEntity
    {
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        string _filePath = Path.Combine(_folderPath, _fileName);

        try
        {
            List<T> models = await ReadFromFileAsync<T>(_filePath);

            if (models == null)
            {
                models = new List<T>();
            }

            var index = models.FindIndex(x => x.ID == model.ID);

            if (index != -1)
            {
                models[index] = model;
            }
            else
            {
                models.Add(model);
            }

            string modelJson = JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, modelJson);

            return model;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write to file '{_filePath}': {ex.Message}");
        }
    }

    public async Task<bool> CreateFile<T>(List<T> models, string _fileName) where T : IEntity
    {
        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        string _filePath = Path.Combine(_folderPath, _fileName);

        try
        {
            if (models == null)
            {
                throw new Exception($"No data found");
            }

            string modelJson = JsonSerializer.Serialize(models, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, modelJson);

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to write to file '{_filePath}': {ex.Message}");
        }
    }

    public async Task<List<T>> ReadFromFileAsync<T>(string fileName)
    {
        string _filePath = Path.Combine(_folderPath, fileName);

        if (!File.Exists(_filePath))
        {
            return new List<T>();
        }

        string fileContent = await File.ReadAllTextAsync(_filePath);

        if (fileContent.Trim() == "{}" || fileContent.Trim() == "[]")
        {
            return new List<T>();
        }

        if (string.IsNullOrEmpty(fileContent) || string.IsNullOrWhiteSpace(fileContent) || fileContent == "[]")
        {
            throw new InvalidDataException($"The file '{fileName}' is empty or contains invalid data.");
        }

        List<T>? data = JsonSerializer.Deserialize<List<T>>(fileContent);

        if (data == null)
        {
            throw new InvalidDataException($"Failed to deserialize the content of '{fileName}'. The data may be malformed or not in the expected format.");
        }

        return data;
    }

    public async Task<List<T>> ReadFilesAsync<T>(string fileName)
    {
        var fileContents = Directory.EnumerateFiles(_folderPath, fileName, SearchOption.TopDirectoryOnly)
            .Select(file => File.ReadAllTextAsync(file))
            .ToList();

        List<T> models = new List<T>();

        foreach (var content in fileContents)
        {
            // Deserialize each file's content back into the model type T
            List<T>? fileModels = JsonSerializer.Deserialize<List<T>>(await content);
            if (fileModels != null)
            {
                models.AddRange(fileModels);
            }
        }

        return models;
    }

    public async Task WriteToCSV<T>(List<T> models, string fileName)
    {
        string _filePath = Path.Combine(_folderPath, fileName);
        var header = "";
        var builder = new StringBuilder();

        var info = typeof(T).GetProperties();

        if (!File.Exists(_filePath))
        {
            using (var file = File.Create(_filePath))
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    header += prop.Name + ",";
                }

                header = header.Substring(0, header.Length - 2);
                builder.AppendLine(header);
                
                foreach (var item in models)
                {
                    var line = "";

                    foreach (var prop in info)
                    {
                        line += prop.GetValue(item, null) + ",";
                    }

                    line = line.Substring(0, line.Length - 2);
                    builder.AppendLine(line);
                }
            }

            using (var writer = new StreamWriter(_filePath, true))
            {
                await writer.WriteAsync(builder.ToString());
            }
        }
    }
}