using Phase.Toolchain.Configuration;

namespace Phase;

public static class Utilities {
    public static void EnsureDirectoriesExist() {
        Directory.CreateDirectory(".phase/objects/");
        Directory.CreateDirectory(".phase/trash/");
    }

    public static string OptimizationLevelToFriendly(OptimizationLevel level) {
        return level switch {
            OptimizationLevel.None => "-O0",
            OptimizationLevel.Less => "-O1",
            OptimizationLevel.Standard => "-O2",
            OptimizationLevel.Aggressive => "-O3",
            OptimizationLevel.DebugFriendly => "-Og",
            OptimizationLevel.Size => "-Os",
            OptimizationLevel.AggressiveSize => "-Oz",
            OptimizationLevel.Fastest => "-O3 -ffast-math",

            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        };
    }

    public static string StandardToFriendly(CppStandard standard) {
        return standard switch {
            CppStandard.Latest => "-std=c++26",

            _ => throw new ArgumentOutOfRangeException(nameof(standard), standard, null)
        };
    }
}