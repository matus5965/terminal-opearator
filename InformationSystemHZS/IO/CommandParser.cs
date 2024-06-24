using InformationSystemHZS.IO.Helpers.Interfaces;
using InformationSystemHZS.Services;

namespace InformationSystemHZS.IO;

public class CommandParser
{
    private IConsoleManager _consoleManager;
    public delegate void EventHandler(object sender, string command);
    public event EventHandler Input;

    public CommandParser(IConsoleManager consoleManager)
    {
        _consoleManager = consoleManager;
    }


    private Command? ParseTextCommand(OperatorActions op, string text)
    {
        string[] tokens = Utils.GetQuotesTokens(text);

        if (tokens.Length == 0)
        {
            return null;
        }

        string stringCommand = tokens[0].ToLower();
        string[] args = tokens.Skip(1).ToArray();

        foreach (var actionPair in op.StringActions.Zip(op.Actions))
        {
            string actionName = actionPair.First;
            var action = actionPair.Second;

            if (actionName.Equals(stringCommand))
            {
                return new Command(action, args);
            }
        }

        return null;
    }

    public Command? GetCommand(OperatorActions op, OutputWriter output)
    {
        Command? command = null;
        while (command == null)
        {
            output.UserInputIndent();
            string? textCommand = _consoleManager.ReadLine();
            if (textCommand != null)
            {
                InputGiven(textCommand);
            }

            if (textCommand == null || (textCommand = textCommand.Trim()).Length == 0)
            {
                continue;
            }

            return ParseTextCommand(op, textCommand);
        }

        return null;
    }

    protected void InputGiven(string command)
    {
        EventHandler handler = Input;
        if (handler != null)
        {
            handler(this, command);
        }
    }
}

public class Command
{
    private Action<string[]> _action;
    private string[] _args;

    internal Command(Action<string[]> action, string[] args)
    {
        _action = action;
        _args = args;
    }

    public void Execute()
    {
        _action(_args);
    }
}