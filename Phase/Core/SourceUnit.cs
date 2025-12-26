using Phase.Toolchain.Configuration;

namespace Phase.Core;

public record SourceUnit {
    public required string FilePath { get; set; }
    public required string ObjectName { get; set; }
    public required CompilerFlags Flags { get; set; }
}