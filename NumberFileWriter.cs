using System.Diagnostics;

namespace NumberFileGenerator;

/// <summary>Streams increasing ASCII numbers with bounded memory and no sidecar files.</summary>
static class NumberFileWriter
{
    const int BufferBytes = 8 * 1024 * 1024;

    /// <summary>Creates a unique file beside the program and durably flushes the requested byte count.</summary>
    /// <param name="directory">Existing executable directory; no directory is created.</param>
    /// <param name="plan">Requested size, last complete number and final whitespace count.</param>
    /// <param name="cancellation">Stops at a buffer boundary, preserving an explicitly reported partial file.</param>
    public static void Write(string directory, OutputPlan plan, CancellationToken cancellation)
    {
        using FileStream file = CreateFile(directory, plan);
        Console.WriteLine(Strings.Destination, file.Name);
        byte[] buffer = GC.AllocateUninitializedArray<byte>(BufferBytes);
        byte[] digits = new byte[20];
        int first = digits.Length - 1;
        digits[first] = (byte)'1';
        int used = 0;
        long committed = 0, progressAt = 0;
        Stopwatch elapsed = Stopwatch.StartNew();
        try
        {
            for (long number = 1; number <= plan.LastNumber; ++number)
            {
                int count = digits.Length - first;
                digits.AsSpan(first, count).CopyTo(buffer.AsSpan(used));
                used += count;
                buffer[used++] = (byte)' ';
                if (used > buffer.Length - 32)
                {
                    FlushBuffer(file, buffer, ref used, ref committed, cancellation);
                    if (elapsed.ElapsedMilliseconds >= progressAt)
                    {
                        Console.WriteLine(Strings.Progress, committed / (double)plan.Bytes * 100,
                            committed / (double)OutputPlan.BytesPerGb);
                        progressAt = elapsed.ElapsedMilliseconds + 1000;
                    }
                }
                int position = digits.Length - 1;
                while (position >= first && digits[position] == (byte)'9') digits[position--] = (byte)'0';
                if (position < first) digits[--first] = (byte)'1';
                else ++digits[position];
            }
            buffer.AsSpan(used, plan.Padding).Fill((byte)' ');
            used += plan.Padding;
            FlushBuffer(file, buffer, ref used, ref committed, cancellation);
            file.Flush(true);
            if (committed != plan.Bytes || file.Length != plan.Bytes) throw new IOException(Strings.WrongLength);
            Console.WriteLine(Strings.Completed, plan.Bytes, plan.LastNumber, elapsed.Elapsed.TotalSeconds);
        }
        catch
        {
            // A failed or cancelled write remains the only output; never disguise it as a complete file.
            Console.Error.WriteLine(Strings.Partial, file.Name, file.Length);
            throw;
        }
    }

    /// <summary>Chooses an unused filename atomically, without overwriting existing files.</summary>
    static FileStream CreateFile(string directory, OutputPlan plan)
    {
        for (int suffix = 1; ; ++suffix)
        {
            string ending = suffix == 1 ? "" : $"-{suffix}";
            string path = Path.Combine(directory, $"Numbers-{plan.SizeLabel}GB{ending}.txt");
            try
            {
                return new FileStream(path, new FileStreamOptions
                {
                    Mode = FileMode.CreateNew,
                    Access = FileAccess.Write,
                    Share = FileShare.Read,
                    BufferSize = 1,
                    PreallocationSize = plan.Bytes,
                    Options = FileOptions.SequentialScan
                });
            }
            catch (IOException error) when ((error.HResult & 0xFFFF) is 80 or 183)
            {
                // A concurrent generator can claim this name between requests; choose the next suffix.
            }
        }
    }

    /// <summary>Writes complete number tokens from the bounded buffer and checks cancellation.</summary>
    static void FlushBuffer(FileStream file, byte[] buffer, ref int used, ref long committed, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();
        if (used == 0) return;
        file.Write(buffer, 0, used);
        committed += used;
        used = 0;
    }
}
