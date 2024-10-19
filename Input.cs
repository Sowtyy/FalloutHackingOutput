internal class Input
{
    public static string AskInput(string? output = null)
    {
        if (output != null)
            Console.Write(output);
        return Console.ReadLine()!;
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
