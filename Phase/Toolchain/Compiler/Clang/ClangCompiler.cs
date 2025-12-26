using System.Diagnostics;
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
            FileName = "clang++",
            Arguments = $"{Flags.ToClangFlags()} {absoluteFilePath} -o {objectFilePath}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(processInfo);
        await process!.WaitForExitAsync();

        Logger.Trace($"> clang++ {Flags.ToClangFlags()} {absoluteFilePath} -o {objectFilePath}");

        if (process.ExitCode != 0) {
            Logger.Error($"failed to compile {absoluteFilePath}, check above for errors");

            return false;
        }

        Logger.Success($"compiled {fileNameNoExt}");

        await Task.Run(() => RunJanitor(fileNameNoExt, hash));

        return true;
    }

    private void RunJanitor(string fileName, string hash) {
        var pattern = $"{fileName}.*.o";
        var dir = Path.GetFullPath("./.phase/objects/");

        Logger.Trace($"running janitor inside {dir}");

        foreach (var file in Directory.GetFiles(dir, pattern)) {
            var filename = Path.GetFileName(file);

            if (file.Contains(hash)) {
                Logger.Trace($"janitor located {hash}, skipping");

                continue;
            }

            try {
                File.Delete(file);
                Logger.Trace($"janitor deleted artifact {filename}");
            } catch (IOException) {
                Logger.Warning($"janitor cannot delete artifact {filename}");
            }
        }
    }
}