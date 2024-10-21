using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FalloutHackingOutput;

internal class Program
{
    private const string _noiseCharacters = "!@#$%^&*()_+-=[]{}/\\,.<>?|~`:;'\"";
    private readonly List<string> _keywords = ["test"];

    private uint _maxRow = 0;
    private uint _maxRows = 0;
    private uint _keywordRate = 0;
    private uint _minIdentifier = 0;
    private uint _maxIdentifier = 0;
    private uint _maxIdentifierStep = 0;

    private readonly List<string> _rows = [];
    private string _rowsText = "";

    private void SaveToFile()
    {
        string path = Process.GetCurrentProcess().ProcessName + "-output.txt";
        File.WriteAllText(path, _rowsText);
        Console.WriteLine($"\nSaved to '{path}'.");
    }

    private void GenerateNoiseRows()
    {
        for (int i = 0; i < _maxRows; i++)
        {
            string row = "";

            for (int j = 0; j < _maxRow; j++)
                row += _noiseCharacters[Random.Shared.Next(_noiseCharacters.Length)];

            _rows.Add(row);
        }
    }

    private void InsertWordsIntoRows()
    {
        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            string row = _rows[rowIndex];
            StringBuilder newRow = new StringBuilder(row);

            string word = _keywords[Random.Shared.Next(_keywords.Count)];
            int tempCharacterIndex = newRow.Length - word.Length;
            int characterIndex = tempCharacterIndex < 0 ? 0 : Random.Shared.Next(tempCharacterIndex + 1);

            if (newRow.Length >= word.Length + characterIndex &&
                Random.Shared.Next((int)_keywordRate!) == 0)
            {
                for (int i = characterIndex, j = 0; i < newRow.Length && j < word.Length; i++, j++)
                    newRow[i] = word[j];
            }

            _rows[rowIndex] = newRow.ToString();
        }
    }

    private void InsertIdentifiersIntoRows()
    {
        long maxIdentifierStepTotal = _maxIdentifierStep * _rows.Count;
        uint maxIdentifier = (uint)(_maxIdentifier - maxIdentifierStepTotal);
        int identifier = Random.Shared.Next((int)_minIdentifier, (int)maxIdentifier);

        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            identifier += Random.Shared.Next(1, (int)_maxIdentifierStep + 1);

            string row = _rows[rowIndex];
            string newRow = $"{identifier} {row}";

            _rows[rowIndex] = newRow;
        }
    }

    private void GenerateRows()
    {
        GenerateNoiseRows();
        InsertWordsIntoRows();
        InsertIdentifiersIntoRows();
    }

    private void RowsToText()
    {
        _rowsText = string.Join("\n", _rows);
    }

    private void ClearRows()
    {
        _rows.Clear();
        _rowsText = "";
    }

    private void AskSettings()
    {
        _maxRow = Input.AskInputUntilUint("Enter amount of characters in a row: ");
        _maxRows = Input.AskInputUntilUint("Enter amount of rows: ");
        _keywordRate = Input.AskInputUntilUint("Enter keyword appearing chance (1 in X): ");
        _minIdentifier = Input.AskInputUntilUint("Enter minimum identifier: ");
        _maxIdentifier = Input.AskInputUntilUint("Enter maximum identifier: ");
        _maxIdentifierStep = Input.AskInputUntilUint("Enter maximum identifier step: ");
    }

    private void Start(bool toAskSettings = true)
    {
        if (toAskSettings)
            AskSettings();

        GenerateRows();
        RowsToText();

        Console.WriteLine("\n" + _rowsText);
    }

    static void Main()
    {
        Program program = new Program();

        bool toAskSettings = true;
        bool toStart = true;

        while (true)
        {
            if (toStart)
            {
                program.ClearRows();
                program.Start(toAskSettings);
            }
            else
                toStart = true;

            string input = Input.AskInput("\n" + """
                                          [Enter] - Try again
                                          [C]     - Try again with new settings
                                          [S]     - Save to file
                                          [Q]     - Quit
                                          """ + " ").ToLower();
            if (input == "q")
                break;
            if (input == "s")
            {
                toStart = false;
                program.SaveToFile();
                continue;
            }
            toAskSettings = input == "c";

            if (toAskSettings)
                Console.WriteLine();
        }
    }
}
