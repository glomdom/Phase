using Phase.Logging;

namespace Phase.Core;

public static class Janitor {
    public static void Cleanup(string fileName, string hash) {
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