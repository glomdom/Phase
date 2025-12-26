using System.Buffers;
using Phase.Logging;

namespace Phase.Core;

public readonly ref struct FileBuffer : IDisposable {
    private readonly byte[]? _rentedArray;
    private readonly int _length;

    public FileBuffer(byte[]? rentedArray, int length) {
        _rentedArray = rentedArray;
        _length = length;
    }

    public ReadOnlySpan<byte> Span => _rentedArray is null
        ? ReadOnlySpan<byte>.Empty
        : _rentedArray.AsSpan(0, _length);

    public void Dispose() {
        if (_rentedArray is null) return;

        ArrayPool<byte>.Shared.Return(_rentedArray);
        Logger.Trace("disposed file buffer");
    }
}