namespace Phase.Toolchain.Configuration;

public enum OptimizationLevel {
    None, // -O0
    Less, // -O1 / -O
    Standard, // -O2
    Aggressive, // -O3
    DebugFriendly, // -Og
    Size, // -Os
    AggressiveSize, // -Oz
    Fastest, // -O3 -ffast-math
}
