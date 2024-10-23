using System;

namespace FalloutHackingOutput;

public static class VersionInfo
{
    public const int Major = 1;
    public const int Minor = 0;
    public const int Patch = 0;

    public static string Name { get; } = $"{Major}.{Minor}.{Patch}";
}

public static class AppInfo
{
    public const string Name = "Fallout Hacking Output";
    public const string Author = "Sowtyy";
    public static readonly string VersionName = VersionInfo.Name;
    public static readonly DateTime VersionDate = new DateTime(2024, 10, 23);

    public static string NameAndVersion { get; } = $"{Name} {VersionName}";
}
