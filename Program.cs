using System.Text;

namespace FalloutHackingOutput;

internal class Program
{
    private const string _noiseCharacters = "!@#$%^&*()_+-=[]{}/\\,.<>?|~`:;'\"";
    private List<string> _keywords = ["test"];

    private uint? _maxRow = null;
    private uint? _maxRows = null;
    private uint? _keywordRate = null;

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

    private List<string> AddWordsToColumn(List<string> rows)
    {
        List<string> newRows = [];

        foreach (string row in rows)
        {
            StringBuilder newRow = new StringBuilder(row);

            string word = _keywords[Random.Shared.Next(_keywords.Count)];
            int rowIndex = Random.Shared.Next(newRow.Length);

            if (newRow.Length > rowIndex + word.Length &&
                Random.Shared.Next((int)_keywordRate!) == 0)
            {
                for (int i = rowIndex, j = 0; i < newRow.Length && j < word.Length; i++, j++)
                    newRow[i] = word[j];
            }

            newRows.Add(newRow.ToString());
        }

        return newRows;
    }

    private void Start()
    {
        _maxRow = Input.AskInputUntilUint("Enter amount of characters in a row: ");
        _maxRows = Input.AskInputUntilUint("Enter amount of rows: ");
        _keywordRate = Input.AskInputUntilUint("Enter keyword appearing chance (1 in X): ");

        foreach (string row in AddWordsToColumn(GenerateNoiseColumn()))
            Console.WriteLine(row);
    }

    static void Main(string[] args)
    {
        new Program().Start();
    }
}
