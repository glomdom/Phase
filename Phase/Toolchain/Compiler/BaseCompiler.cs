using Phase.Toolchain.Configuration;

namespace Phase.Toolchain.Compiler;

public abstract class BaseCompiler {
    public abstract CompilerFlags Flags { get; set; }

    public abstract Task<bool> CompileFileAsync(string absoluteFilePath, string hash);
}