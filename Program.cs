using System.Globalization;
using System.Text;

namespace NumberFileGenerator;

/// <summary>Prompts for a size and contains failures at the console application boundary.</summary>
static class Program
{
    /// <summary>Writes one number file beside the executable, without persistent configuration.</summary>
    static int Main()
    {
        Console.OutputEncoding = new UTF8Encoding(false);
        using CancellationTokenSource cancellation = new();
        bool writing = false;
        ConsoleCancelEventHandler cancel = (_, e) =>
        {
            if (writing) { e.Cancel = true; cancellation.Cancel(); }
        };
        Console.CancelKeyPress += cancel;
        try
        {
            Console.WriteLine("NumberFileGenerator 1.0.0");
            Console.WriteLine(Strings.Description);
            OutputPlan plan;
            while (true)
            {
                Console.Write(Strings.Prompt);
                string? input = Console.ReadLine();
                if (input is null) return 1;
                if (OutputPlan.TryParse(input, out plan)) break;
                Console.Error.WriteLine(Strings.InvalidSize);
            }
            string directory = AppContext.BaseDirectory;
            string root = Path.GetPathRoot(directory)!;
            if (Directory.Exists(root) && new DriveInfo(root).AvailableFreeSpace < plan.Bytes)
                throw new IOException(Strings.NoSpace);
            writing = true;
            NumberFileWriter.Write(directory, plan, cancellation.Token);
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine(Strings.Cancelled);
            return 2;
        }
        catch (Exception error)
        {
            Console.Error.WriteLine(Strings.Failed);
            Console.Error.WriteLine(error);
            return 1;
        }
        finally
        {
            writing = false;
            Console.CancelKeyPress -= cancel;
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine(Strings.Close);
                Console.ReadLine();
            }
        }
    }
}

/// <summary>Describes an exact byte count, complete final number, and trailing space padding.</summary>
readonly record struct OutputPlan(long Bytes, long LastNumber, int Padding, string SizeLabel)
{
    public const long BytesPerGb = 1L << 30;

    /// <summary>Accepts positive decimal GB values using a comma or dot, with no thousands separators.</summary>
    /// <param name="input">Requested GB amount, for example 1,5.</param>
    /// <param name="plan">Exact output size and sequence bounds on success.</param>
    /// <returns>Whether the input fits a positive whole-byte file size.</returns>
    public static bool TryParse(string input, out OutputPlan plan)
    {
        plan = default;
        string value = input.Trim().Replace(',', '.');
        if (!decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal gb)
            || gb <= 0 || gb > (decimal)long.MaxValue / BytesPerGb) return false;
        long bytes = (long)decimal.Truncate(gb * BytesPerGb);
        if (bytes < 2) return false;
        long remaining = bytes, last = 0, first = 1;
        for (int width = 1; remaining > 0; ++width)
        {
            long group = 9 * first;
            long take = Math.Min(group, remaining / (width + 1));
            last += take;
            remaining -= take * (width + 1);
            if (take < group) break;
            first *= 10;
        }
        plan = new(bytes, last, (int)remaining, gb.ToString("0.############################", CultureInfo.InvariantCulture));
        return true;
    }
}
