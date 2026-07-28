using System.Buffers;
using System.IO.Compression;

namespace HugeMazes.IO;

public static class StoreDeflater
{
    public static void Deflate(IStore store)
    {
        var mappings = DeflateChunks(store);
        if(mappings.Length == 0) return;

        var destinationOffset = mappings[0].Offset + mappings[0].Length;

        for(var i = 1; i < mappings.Length; i++)
        {
            store.Move(mappings[i].Offset, mappings[i].Length, destinationOffset);
            destinationOffset += mappings[i].Length;
        }

        store.Length = destinationOffset;
    }

    private static Mapping[] DeflateChunks(IStore store)
    {
        var mappings = new List<Mapping>();
        using var encoder = new ZLibEncoder(8);

        var position = 0L;
        int bytesRead;
        var sourceBuffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);
        var destinationBuffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);

        try
        {
            while((bytesRead = store.Read(position, sourceBuffer)) != 0)
            {
                var status = encoder.Compress(
                    sourceBuffer.AsSpan(0, bytesRead),
                    destinationBuffer,
                    out var bytesConsumed,
                    out var bytesWritten,
                    position + bytesRead == store.Length);

                if(status != OperationStatus.Done) throw new InvalidOperationException();
                if(bytesConsumed != bytesRead) throw new InvalidOperationException();
                if(bytesWritten > sourceBuffer.Length) throw new InvalidOperationException();

                store.Write(position, destinationBuffer[..bytesWritten]);
                mappings.Add(new(position, bytesWritten));

                position += bytesRead;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sourceBuffer);
            ArrayPool<byte>.Shared.Return(destinationBuffer);
        }

        return [..mappings];
    }

    private readonly record struct Mapping(long Offset, int Length);
}
