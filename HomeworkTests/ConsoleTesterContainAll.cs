namespace HomeworkTests;

public class ConsoleTesterContainAll : ConsoleTester
{
    public ConsoleTesterContainAll(string[]? inputs, string[]? outputs) : base(inputs, outputs)
    {
    }

    public override void WriteLine(string s)
    {
        var expectedOutput = Outputs[OutputPointer];
        var expectedStrings = expectedOutput.Split("#");
        Console.WriteLine(s);
        foreach (var word in expectedStrings)
        {
            Console.WriteLine(word);
            if (!s.Contains(word))
            {
                throw new Exception("TEST FAILED");
            }
        }

        OutputPointer++;
    }
}