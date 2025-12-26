using System.Diagnostics;
using Phase.Core;
using Phase.Logging;
using Phase.Toolchain.Configuration;

namespace Phase.Toolchain.Compiler.Clang;

public sealed class ClangCompiler : BaseCompiler {
    public override CompilerFlags Flags { get; set; }

    public ClangCompiler(CompilerFlags flags) {
        Flags = flags;
    }

    public override async Task<bool> CompileFileAsync(string absoluteFilePath, string hash) {
        Logger.Trace($"starting compile of {absoluteFilePath}");

        var fileNameNoExt = Path.GetFileNameWithoutExtension(absoluteFilePath);
        var fileName = Path.GetFileName(absoluteFilePath);
        var dir = Path.GetFullPath("./.phase/objects");

        var objectFilePath = $"{dir}/{fileNameNoExt}.{hash}.o";

        if (File.Exists(objectFilePath)) {
            Logger.Debug($"cache hit {hash} for {fileName}");

            return true;
        }

        var processInfo = new ProcessStartInfo {
            FileName = "clang-cl",
            Arguments = $"/c {Flags.ToClangFlags()} {absoluteFilePath} /Fo\"{objectFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(processInfo);
        await process!.WaitForExitAsync();

        Logger.Trace($"> clang-cl {processInfo.Arguments}");

        if (process.ExitCode != 0) {
            Logger.Error($"failed to compile {absoluteFilePath}, check above for errors");

            return false;
        }

        Logger.Success($"compiled {fileNameNoExt}");

        await Task.Run(() => Janitor.Cleanup(fileNameNoExt, hash));

        return true;
    }

    public async Task LinkObjectsAsync(string args) {
        var processInfo = new ProcessStartInfo {
            FileName = "clang-cl",
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        Logger.Trace($"> clang-cl {args}");

        using var process = Process.Start(processInfo);
        await process!.WaitForExitAsync();
    }
}