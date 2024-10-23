using System.Text.Json.Nodes;

namespace FalloutHackingOutput;

internal class Settings
{
    public uint MaxRow = 0;
    public uint MaxRows = 0;
    public uint KeywordRate = 0;
    public uint MinIdentifier = 0;
    public uint MaxIdentifier = 0;
    public uint MaxIdentifierStep = 0;
    public bool ShowHexIdentifiers = true;
    public string IdentifierPrefix = "";

    public JsonObject ToJson()
    {
        return new JsonObject()
        {
            ["max_row"] = MaxRow,
            ["max_rows"] = MaxRows,
            ["keyword_rate"] = KeywordRate,
            ["min_identifier"] = MinIdentifier,
            ["max_identifier"] = MaxIdentifier,
            ["max_identifier_step"] = MaxIdentifierStep,
            ["show_hex_identifiers"] = ShowHexIdentifiers,
            ["identifier_prefix"] = IdentifierPrefix
        };
    }
    public void FromJson(JsonObject json)
    {
        MaxRow = json["max_row"]!.GetValue<uint>();
        MaxRows = json["max_rows"]!.GetValue<uint>();
        KeywordRate = json["keyword_rate"]!.GetValue<uint>();
        MinIdentifier = json["min_identifier"]!.GetValue<uint>();
        MaxIdentifier = json["max_identifier"]!.GetValue<uint>();
        MaxIdentifierStep = json["max_identifier_step"]!.GetValue<uint>();
        ShowHexIdentifiers = json["show_hex_identifiers"]!.GetValue<bool>();
        IdentifierPrefix = json["identifier_prefix"]!.GetValue<string>();
    }
    public void FromJson(string json)
    {
        FromJson(JsonNode.Parse(json)!.AsObject());
    }
}
