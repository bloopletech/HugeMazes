using System.Buffers;
using System.IO.Compression;
using HugeMazes.IO;

namespace HugeMazes.Images;

public class StoreDeflater(IStore store)
{
    public void Deflate()
    {
        var mappings = DeflateChunks();
        if(mappings.Length == 0) return;

        var destinationOffset = mappings[0].Offset + mappings[0].Length;

        for(var i = 1; i < mappings.Length; i++)
        {
            store.Move(mappings[i].Offset, mappings[i].Length, destinationOffset);
            destinationOffset += mappings[i].Length;
        }

        store.Length = destinationOffset;
    }

    private Mapping[] DeflateChunks()
    {
        var mappings = new List<Mapping>();
        using var encoder = new ZLibEncoder();

        var position = 0L;
        int bytesRead;
        var sourceBuffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);
        var destinationBuffer = ArrayPool<byte>.Shared.Rent(IStore.BufferSize);

        try
        {
            while((bytesRead = store.Read(position, sourceBuffer)) != 0)
            {
                var start = 0;
                var end = 0;

                while(start < bytesRead)
                {
                    var status = encoder.Compress(
                        sourceBuffer.AsSpan(start, bytesRead - start),
                        destinationBuffer.AsSpan(end),
                        out var bytesConsumed,
                        out var bytesWritten,
                        false);

                    if(status != OperationStatus.Done) throw new InvalidOperationException();

                    start += bytesConsumed;
                    end += bytesWritten;
                }

                if(end > sourceBuffer.Length) throw new InvalidOperationException();

                store.Write(position, destinationBuffer[..end]);
                mappings.Add(new(position, end));

                position += bytesRead;
            }

            var flushStatus = encoder.Flush(destinationBuffer, out var lastBytesWritten);
            if(flushStatus != OperationStatus.Done) throw new InvalidOperationException();

            store.Write(position, destinationBuffer[..lastBytesWritten]);
            mappings.Add(new(position, lastBytesWritten));
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
