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

    private uint? _maxRow = null;
    private uint? _maxRows = null;
    private uint? _keywordRate = null;

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

    private void AddWordsToRows()
    {
        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            string row = _rows[rowIndex];
            StringBuilder newRow = new StringBuilder(row);

            string word = _keywords[Random.Shared.Next(_keywords.Count)];
            int characterIndex = Random.Shared.Next(newRow.Length);

            if (newRow.Length > characterIndex + word.Length &&
                Random.Shared.Next((int)_keywordRate!) == 0)
            {
                for (int i = characterIndex, j = 0; i < newRow.Length && j < word.Length; i++, j++)
                    newRow[i] = word[j];
            }

            _rows[rowIndex] = newRow.ToString();
        }
    }

    private void GenerateRows()
    {
        GenerateNoiseRows();
        AddWordsToRows();
    }

    private void RowsToText()
    {
        _rowsText = string.Join("\n", _rows);
    }

    private void AskSettings()
    {
        _maxRow = Input.AskInputUntilUint("Enter amount of characters in a row: ");
        _maxRows = Input.AskInputUntilUint("Enter amount of rows: ");
        _keywordRate = Input.AskInputUntilUint("Enter keyword appearing chance (1 in X): ");
    }

    private void Start(bool toAskSettings = true)
    {
        if (toAskSettings)
            AskSettings();

        _rows.Clear();
        _rowsText = "";

        GenerateRows();
        RowsToText();

        Console.WriteLine("\n" + _rowsText);
    }

    static void Main(string[] args)
    {
        Program program = new Program();

        bool toAskSettings = true;
        bool toStart = true;

        while (true)
        {
            if (toStart)
                program.Start(toAskSettings);
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
        }
    }
}
