using Phase.Logging;
using Phase.Toolchain.Compiler;
using Phase.Toolchain.Configuration;

namespace Phase.Core;

public class Project {
    public required string Name { get; set; }
    public required string RootDirectory { get; set; }
    public required BaseCompiler Compiler { get; set; }

    public List<string> Sources { get; set; } = [];

    // TODO: split this per files?
    public CompilerFlags Flags { get; set; } = CompilerFlags.Default();

    public static Project FromDirectory(string path, BaseCompiler compiler) {
        if (!Directory.Exists(path)) {
            Logger.Error("provided path to Project#FromDirectory does not exist");
        }

        var absRoot = Path.GetFullPath(path) + "/";
        var srcDir = Path.Combine(absRoot, "src/");
        if (!Directory.Exists(srcDir)) {
            Logger.Error($"src directory inside {absRoot} does not exist");

            throw new Exception("Invalid project structure");
        }

        var dirName = new DirectoryInfo(absRoot).Name;

        Logger.Debug($"created project from {absRoot}");

        return new Project {
            Name = dirName,
            RootDirectory = absRoot,
            Sources = Directory.GetFiles(srcDir, "*.cpp", SearchOption.AllDirectories).ToList(),
            Compiler = compiler,
        };
    }
}