using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace UnlockAllBackgrounds;

public record ModMetadata : IModMetadata
{
    public string ModGuid { get; init; } = "com.mybutthasarash.unlockallbackgrounds";
    public string Name { get; init; } = "Unlock All Backgrounds";
    public string Author { get; init; } = "Joel";
    public List<string>? Contributors { get; init; }
    public Version Version { get; init; } = new(1, 0, 0);
    public Range SptVersion { get; init; } = new("~4.1");
    public bool HasPrepatcher { get; init; }
    public List<string>? Incompatibilities { get; init; }
    public Dictionary<string, Range>? ModDependencies { get; init; }
    public string? Url { get; init; }
    public string License { get; init; } = "MIT";
}
