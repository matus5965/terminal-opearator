using HomeworkTests.Exceptions;
using InformationSystemHZS.IO.Helpers;
using InformationSystemHZS.IO.Helpers.Interfaces;

namespace HomeworkTests;

public class ConsoleTester : IConsoleManager
{
    protected int InputPointer = 0;

    protected int OutputPointer = 0;

    protected string[] Inputs;

    protected string[] Outputs;

    public ConsoleTester(string[]? inputs, string[]? outputs)
    {
        this.Inputs = inputs ?? new string[]{};
        this.Outputs = outputs ?? new string[] {};
    }

    public void Clear()
    {
        Console.Clear();
    }

    public string? ReadLine()
    {
        if (InputPointer >= Inputs.Length)
        {
            throw new InputEndException();
        }

        var inputString = Inputs[InputPointer];
        InputPointer++;
        return inputString;
    }

    public virtual void WriteLine(string s)
    {
        var expectedString = Outputs[OutputPointer];
        if (!s.Contains(expectedString))
        {
            throw new Exception("TEST FAILED");
        }
    }

    public bool HasValidEndingState()
    {
        return OutputPointer == Outputs.Length && InputPointer == Inputs.Length;
    }
}