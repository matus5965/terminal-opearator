using InformationSystemHZS.IO.Helpers.Interfaces;

namespace InformationSystemHZS.IO.Helpers;

/// <summary>
/// This manager serves as a wrapper for IO operations for our internal purposes (such as testing).
/// DO NOT TOUCH THIS CLASS AND ITS IMPLEMENTATION.
/// </summary>
public class ConsoleManager : IConsoleManager
{
    public void Clear()
    {
        Console.Clear();
    }

    public string? ReadLine()
    {
        return Console.ReadLine();
    }

    public void WriteLine(string s)
    {
        Console.WriteLine(s);
    }
}