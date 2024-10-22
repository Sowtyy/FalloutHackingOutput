﻿using System;
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
    private bool _showHexIdentifiers = true;
    private string _identifierPrefix = "";

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
            string identifierString = identifier.ToString(_showHexIdentifiers ? "X" : null);

            string row = _rows[rowIndex];
            string newRow = $"{_identifierPrefix}{identifierString} {row}";

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
        Console.WriteLine("[Enter] - Skip setting\n");
        _maxRow = Input.AskInputUntilUintNullable($"Amount of characters in a row (now: {_maxRow}): ") ?? _maxRow;
        _maxRows = Input.AskInputUntilUintNullable($"Amount of rows (now: {_maxRows}): ") ?? _maxRows;
        _keywordRate = Input.AskInputUntilUintNullable($"Keyword appearing chance (1 in X) (now: {_keywordRate}): ") ?? _keywordRate;
        _minIdentifier = Input.AskInputUntilUintNullable($"Minimum identifier number (now: {_minIdentifier}): ") ?? _minIdentifier;
        _maxIdentifier = Input.AskInputUntilUintNullable($"Maximum identifier number (now: {_maxIdentifier}): ") ?? _maxIdentifier;
        _maxIdentifierStep = Input.AskInputUntilUintNullable($"Maximum identifier step number (now: {_maxIdentifierStep}): ") ?? _maxIdentifierStep;
        _identifierPrefix = Input.AskInput($"Identifier prefix (now: {_identifierPrefix}): ");
        string? showHexIdentifiersInput = Input.AskInputUntilEqualsNullable(["1", "2"], $"Show identifiers in hexadecimal or decimal format? (now: {(_showHexIdentifiers ? 1 : 2)}) [1/2]: ");
        if (showHexIdentifiersInput != null)
            _showHexIdentifiers = showHexIdentifiersInput == "1";
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
