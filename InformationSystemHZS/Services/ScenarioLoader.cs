using InformationSystemHZS.Models.HelperModels;
using Newtonsoft.Json;

namespace InformationSystemHZS.Services;

/// <summary>
/// This class parses data from Brnoslava.json and deserializes it into ScenarioObject if succesfull.
/// DO NOT TOUCH THIS CLASS IF NOT NECESSARY.
/// </summary>
public static class ScenarioLoader
{
    /// <summary>
    /// Parses ScenarioObjectDto data from a data store indicated by given fileName.
    /// Returns ScenarioObjectDto or null. Can throw exceptions on failure (see ReadAllText() method exceptions).
    /// </summary>
    public static ScenarioObjectDto? GetInitialScenarioData(string fileName)
    {
        // Define the file path relative to the project directory
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Datasets", fileName);

        // Read the JSON file content
        var jsonContent = File.ReadAllText(filePath);

        // Parse the JSON into a RootObject
        return JsonConvert.DeserializeObject<ScenarioObjectDto>(jsonContent);
    }
}