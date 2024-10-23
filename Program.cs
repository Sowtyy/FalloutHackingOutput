using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FalloutHackingOutput;

internal class Program
{
    private const string _noiseCharactersFilePath = "noise-characters.txt";
    private const string _keywordsFilePath = "keywords.txt";
    private const string _settingsFilePath = "settings.json";

    private const string _defaultNoiseCharacters = "!@#$%^&*()_+-=[]{}/\\,.<>?|~`:;'\"";
    private string[] _keywords = ["test", "testtwo"];
    private string _noiseCharacters = _defaultNoiseCharacters;

    private readonly Settings _settings = new Settings();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly List<string> _rows = [];
    private string _rowsText = "";

    private bool LoadAllSettings()
    {
        bool askSettings = false;

        if (!File.Exists(_noiseCharactersFilePath))
        {
            File.WriteAllText(_noiseCharactersFilePath, _defaultNoiseCharacters);
            _noiseCharacters = _defaultNoiseCharacters;
            Console.WriteLine($"Created '{_noiseCharactersFilePath}' file, edit it to change noise characters.");
        }
        else
        {
            _noiseCharacters = File.ReadAllText(_noiseCharactersFilePath);
        }

        if (!File.Exists(_keywordsFilePath))
        {
            File.WriteAllText(_keywordsFilePath, string.Join(Environment.NewLine, _keywords));
            Console.WriteLine($"Created '{_keywordsFilePath}' file, edit it to change keywords. Write each keyword on the new line.");
        }
        else
        {
            string keywordsText = File.ReadAllText(_keywordsFilePath);
            _keywords = keywordsText.Split(Environment.NewLine);
        }

        if (!File.Exists(_settingsFilePath))
        {
            SaveSettings();
            Console.WriteLine($"Created '{_settingsFilePath}' file, it contains other settings.");
            askSettings = true;
        }
        else
        {
            try
            {
                _settings.FromJson(File.ReadAllText(_settingsFilePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't load settings from '{_settingsFilePath}': {ex.Message}.\nResetting settings file.");
                SaveSettings();
                askSettings = true;
            }
        }

        return askSettings;
    }

    private void SaveSettings()
    {
        File.WriteAllText(_settingsFilePath, _settings.ToJson().ToJsonString(_jsonSerializerOptions));
    }

    private void SaveOutputToFile()
    {
        string path = Process.GetCurrentProcess().ProcessName + "-output.txt";
        File.WriteAllText(path, _rowsText);
        Console.WriteLine($"\nSaved to '{path}'.");
    }

    private void GenerateNoiseRows()
    {
        for (int i = 0; i < _settings.MaxRows; i++)
        {
            string row = "";

            for (int j = 0; j < _settings.MaxRow; j++)
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

            string word = _keywords[Random.Shared.Next(_keywords.Length)];
            int tempCharacterIndex = newRow.Length - word.Length;
            int characterIndex = tempCharacterIndex < 0 ? 0 : Random.Shared.Next(tempCharacterIndex + 1);

            if (newRow.Length >= word.Length + characterIndex &&
                Random.Shared.Next((int)_settings.KeywordRate!) == 0)
            {
                for (int i = characterIndex, j = 0; i < newRow.Length && j < word.Length; i++, j++)
                    newRow[i] = word[j];
            }

            _rows[rowIndex] = newRow.ToString();
        }
    }

    private void InsertIdentifiersIntoRows()
    {
        long maxIdentifierStepTotal = _settings.MaxIdentifierStep * _rows.Count;
        uint maxIdentifier = (uint)(_settings.MaxIdentifier - maxIdentifierStepTotal);
        int identifier = Random.Shared.Next((int)_settings.MinIdentifier, (int)maxIdentifier);

        for (int rowIndex = 0; rowIndex < _rows.Count; rowIndex++)
        {
            identifier += Random.Shared.Next(1, (int)_settings.MaxIdentifierStep + 1);
            string identifierString = identifier.ToString(_settings.ShowHexIdentifiers ? "X" : null);

            string row = _rows[rowIndex];
            string newRow = $"{_settings.IdentifierPrefix}{identifierString} {row}";

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
        Console.WriteLine("\n[Enter] - Skip setting\n");
        _settings.MaxRow = Input.AskInputUntilUintNullable($"Amount of characters in a row (now: {_settings.MaxRow}): ") ?? _settings.MaxRow;
        _settings.MaxRows = Input.AskInputUntilUintNullable($"Amount of rows (now: {_settings.MaxRows}): ") ?? _settings.MaxRows;
        _settings.KeywordRate = Input.AskInputUntilUintNullable($"Keyword appearing chance (1 in X) (now: {_settings.KeywordRate}): ") ?? _settings.KeywordRate;
        Console.WriteLine("Identifiers are numbers on the left.");
        _settings.MinIdentifier = Input.AskInputUntilUintNullable($"Minimum identifier number (now: {_settings.MinIdentifier}): ") ?? _settings.MinIdentifier;
        _settings.MaxIdentifier = Input.AskInputUntilUintNullable($"Maximum identifier number (now: {_settings.MaxIdentifier}): ") ?? _settings.MaxIdentifier;
        _settings.MaxIdentifierStep = Input.AskInputUntilUintNullable($"Maximum identifier step number (now: {_settings.MaxIdentifierStep}): ") ?? _settings.MaxIdentifierStep;
        _settings.IdentifierPrefix = Input.AskInput($"Identifier prefix (now: {_settings.IdentifierPrefix}): ");
        string? showHexIdentifiersInput = Input.AskInputUntilEqualsNullable(["1", "2"], $"Show identifiers in hexadecimal or decimal format? (now: {(_settings.ShowHexIdentifiers ? 1 : 2)}) [1/2]: ");
        if (showHexIdentifiersInput != null)
            _settings.ShowHexIdentifiers = showHexIdentifiersInput == "1";
    }

    private void Start(bool toAskSettings)
    {
        if (LoadAllSettings() || toAskSettings)
        {
            AskSettings();
            SaveSettings();
        }

        try
        {
            GenerateRows();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while generating rows: {ex.Message}.");
        }

        RowsToText();

        Console.WriteLine("\n" + _rowsText);
    }

    static void Main()
    {
        Program program = new Program();

        bool toAskSettings = false;
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
                program.SaveOutputToFile();
                continue;
            }
            toAskSettings = input == "c";
        }
    }
}
