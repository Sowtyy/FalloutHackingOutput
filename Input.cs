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
    public static string? AskInputUntilEqualsNullable(string[] values, string? output = null)
    {
        while (true)
        {
            string input = AskInput(output);
            if (input == string.Empty)
                return null;
            if (values.Contains(input))
                return input;
        }
    }
    public static string AskInputUntilEquals(string[] values, string? output = null)
    {
        while (true)
        {
            string? input = AskInputUntilEqualsNullable(values, output);
            if (input != null)
                return input;
        }
    }
    public static bool? AskInputUntilBoolNullable(string? output = null)
    {
        string? input = AskInputUntilEqualsNullable(_boolStrings, output);
        if (input == null)
            return null;
        return _trueStrings.Contains(input);
    }
    public static bool AskInputUntilBool(string? output = null)
    {
        string input = AskInputUntilEquals(_boolStrings, output);
        return _trueStrings.Contains(input);
    }
    public static int? AskInputUntilIntNullable(string? output = null)
    {
        while (true)
        {
            string input = AskInput(output);
            if (input == string.Empty)
                return null;
            if (int.TryParse(input, out int n))
                return n;
        }
    }
    public static int AskInputUntilInt(string? output = null)
    {
        while (true)
        {
            int? input = AskInputUntilIntNullable(output);
            if (input != null)
                return (int)input;
        }
    }
    public static uint? AskInputUntilUintNullable(string? output = null)
    {
        while (true)
        {
            string input = AskInput(output);
            if (input == string.Empty)
                return null;
            if (uint.TryParse(input, out uint n))
                return n;
        }
    }
    public static uint AskInputUntilUint(string? output = null)
    {
        while (true)
        {
            uint? input = AskInputUntilUintNullable(output);
            if (input != null)
                return (uint)input;
        }
    }
}
