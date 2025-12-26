using System.Buffers;
using Phase.Logging;

namespace Phase.Core;

public static class FileHandling {
    public static FileBuffer Read(string path) {
        using var handle = File.OpenHandle(path);

        var length = RandomAccess.GetLength(handle);
        switch (length) {
            case 0: return new FileBuffer(null, 0);
            case > int.MaxValue: throw new IOException("File too large");
        }

        var buffer = ArrayPool<byte>.Shared.Rent((int)length);
        Logger.Trace("rented shared buffer");

        try {
            var span = buffer.AsSpan(0, (int)length);

            RandomAccess.Read(handle, span, 0);
            Logger.Trace("read into shared buffer successfully");
        } catch {
            ArrayPool<byte>.Shared.Return(buffer);
            Logger.Error("failed to read into shared buffer");
        }

        return new FileBuffer(buffer, (int)length);
    }
}