namespace InformationSystemHZS.Services;

public static class Logger
{
    public static void OnInputGiven(object sender, string command)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter("log.txt", true))
            {
                writer.WriteLine(command);
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions, such as file access issues (this should not ever happen)
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}