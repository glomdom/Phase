using System.Collections.Concurrent;
using Phase.Logging;
using Phase.Toolchain.Compiler.Clang;
using Phase.Toolchain.Configuration;

namespace Phase.Core;

public sealed class BuildEngine {
    private readonly ClangCompiler _clang;

    public BuildEngine(CompilerFlags flags) {
        _clang = new ClangCompiler(flags);
    }

    public async Task BuildProjectAsync(Project project) {
        Logger.Debug($"starting build for project {project.Name}");

        var units = project.Sources.Select(fp => new SourceUnit {
            FilePath = fp,
            Flags = project.Flags,
            ObjectName = Path.GetFileNameWithoutExtension(fp),
        });

        var objects = new ConcurrentBag<string>();

        foreach (var unit in units) {
            await CompileUnitAsync(unit, objects);
        }

        await LinkProjectAsync(project, objects.ToList());

        Logger.Success($"finished building {project.Name}");
    }

    public async Task LinkProjectAsync(Project project, List<string> objects) {
        Logger.Debug($"linking project {project.Name}");

        var binDir = Path.Combine(project.RootDirectory, "bin/");
        Directory.CreateDirectory(binDir);
        Logger.Trace($"created {binDir}");

        var exePath = Path.Combine(binDir, $"{project.Name}.exe");
        Logger.Trace($"{exePath}");

        var objList = string.Join(' ', objects.Select(x => $"\"{Path.GetFullPath(x)}\""));
        var flags = CompilerFlags.Default();
        var args = $"/Fe\"{exePath}\" {objList}";

        await _clang.LinkObjectsAsync(args);
    }

    private async Task CompileUnitAsync(SourceUnit unit, ConcurrentBag<string> objects) {
        var hash = Hashing.GetHash(unit.FilePath, unit.Flags.ToClangFlags());
        var output = $".phase/objects/{unit.ObjectName}.{hash}.o";

        objects.Add(output);

        if (File.Exists(output)) {
            Logger.Success($"cache hit {unit.ObjectName}");

            return;
        }

        Logger.Debug($"building {unit.ObjectName}");
        await _clang.CompileFileAsync(unit.FilePath, hash);

        _ = Task.Run(() => Janitor.Cleanup(unit.ObjectName, hash));
    }
}