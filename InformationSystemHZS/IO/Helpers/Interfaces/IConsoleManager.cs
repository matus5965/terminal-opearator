namespace InformationSystemHZS.IO.Helpers.Interfaces;

public interface IConsoleManager
{
    public void Clear();
    public string? ReadLine();
    public void WriteLine(string s);
}