namespace FalloutHackingOutput;

internal class Program
{
    private const string _noiseCharacters = "!@#$%^&*()_+-=[]{}/\\,.<>?|~`:;'\"";

    private int? _maxRow = null;
    private int? _maxRows = null;

    private List<string> GenerateNoiseColumn()
    {
        List<string> rows = [];

        for (int i = 0; i < _maxRows; i++)
        {
            string row = "";

            for (int j = 0; j < _maxRow; j++)
                row += _noiseCharacters[Random.Shared.Next(_noiseCharacters.Length)];

            rows.Add(row);
        }

        return rows;
    }

    private void Start()
    {
        _maxRow = Input.AskInputUntilInt("Enter amount of characters in a row: ");
        _maxRows = Input.AskInputUntilInt("Enter amount of rows: ");

        foreach (string row in GenerateNoiseColumn())
            Console.WriteLine(row);
    }

    static void Main(string[] args)
    {
        Program program = new Program();
        program.Start();
    }
}
