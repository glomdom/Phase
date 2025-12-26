using Phase.Core;
using Phase.Logging;
using Phase.Toolchain.Compiler.Clang;
using Phase.Toolchain.Configuration;

namespace Phase;

internal static class Program {
    internal static async Task Main(string[] args) {
        Utilities.EnsureDirectoriesExist();

        var compiler = new ClangCompiler(CompilerFlags.Default());

        var proj = Project.FromDirectory(".", compiler);
        Logger.Trace($"has sources: {string.Join(", ", proj.Sources)}");
        Logger.Trace($"will compile with {proj.Flags.ToClangFlags()}");

        await proj.CompileAsync();

        await Logger.ShutdownAsync();
    }
}