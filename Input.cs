using System;
using System.Linq;

internal class Input
{
    private static readonly string[] _boolStrings = ["y", "n", "yes", "no", "true", "false", "1", "0"];
    private static readonly string[] _trueStrings = ["y", "yes", "true", "1"];
    private static readonly string[] _falseStrings = ["n", "no", "false", "0"];
    public static string AskInput(string? output = null)
    {
        if (output != null)
            Console.Write(output);
        return Console.ReadLine()!;
    }
    public static string AskInputUntilEquals(string[] values, string? output = null)
    {
        while (true)
        {
            string input = AskInput(output);
            if (values.Contains(input))
                return input;
        }
    }
    public static bool AskInputUntilBool(string? output = null)
    {
        string input = AskInputUntilEquals(_boolStrings, output);
        return _trueStrings.Contains(input);
    }
    public static int AskInputUntilInt(string? output = null)
    {
        while (true)
        {
            if (int.TryParse(AskInput(output), out int n))
                return n;
        }
    }
    public static uint AskInputUntilUint(string? output = null)
    {
        while (true)
        {
            if (uint.TryParse(AskInput(output), out uint n))
                return n;
        }
    }
}
