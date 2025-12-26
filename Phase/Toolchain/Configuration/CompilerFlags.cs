namespace Phase.Toolchain.Configuration;

public record CompilerFlags {
    public OptimizationLevel? Optimization { get; set; }
    public CppStandard? Standard { get; set; }

    public static CompilerFlags Default() => new() {
        Optimization = OptimizationLevel.Standard,
        Standard = CppStandard.Latest,
    };

    public string ToClangFlags() {
        var flags = new List<string>();

        if (Optimization is { } level) {
            flags.Add(Utilities.OptimizationLevelToFriendly(level));
        }

        if (Standard is { } standard) {
            flags.Add(Utilities.StandardToFriendly(standard));
        }

        return string.Join(' ', flags);
    }
};