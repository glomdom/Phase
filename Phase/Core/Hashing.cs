using System.IO.Hashing;
using System.Text;

namespace Phase.Core;

public static class Hashing {
    public static string GetHash(string path, string flags) {
        using var fileBuffer = FileHandling.Read(path);
        var hasher = new XxHash3();
        hasher.Append(fileBuffer.Span);
        
        var maxBytes = Encoding.UTF8.GetByteCount(flags);
        Span<byte> flagBuffer = stackalloc byte[maxBytes];
        var bytesWritten = Encoding.UTF8.GetBytes(flags, flagBuffer);
        
        hasher.Append(flagBuffer[..bytesWritten]);
        
        return Convert.ToHexString(hasher.GetCurrentHash());
    }
}