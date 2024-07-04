namespace CalculatePi;

internal class Program
{
    static void Main(string[] args)
    {
        decimal nilakanthaPi;
        int iterations = 100_000_000;
        TimeSpan ts;
        EstimatePiWithNilakantha(iterations, out nilakanthaPi, out ts);

        Console.WriteLine("pi 28 digits = {0}", "3.1415926535897932384626433832");
        Console.WriteLine("pi estimate =  {0}", nilakanthaPi);
        Console.WriteLine("Accuracy resulting from {0} steps", iterations.ToString("N0"));
        Console.WriteLine("Calc Time {0}", ts);

        Console.WriteLine("Press any key to Exit");
        var _ = Console.ReadKey().Key;

    }

    private static void EstimatePiWithNilakantha(int iterations, out decimal nilakanthaPi, out TimeSpan ts)
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        // Nilakantha formula - code written completely by Todd Mandell 2014
        // π = 3 + 4/(2*3*4) - 4/(4*5*6) + 4/(6*7*8) - 4/(8*9*10) + 4/(10*11*12) - (4/(12*13*14) etc

        nilakanthaPi = 0;
        decimal a = 2;
        decimal b = 3;
        decimal c = 4;

        for (int f = 1; f <= iterations; f++)
        {

            nilakanthaPi += 4 / (a * b * c);

            a += 2;
            b += 2;
            c += 2;

            nilakanthaPi -= 4 / (a * b * c);

            a += 2;
            b += 2;
            c += 2;
        }

        nilakanthaPi += 3;

        stopwatch.Stop();
        ts = stopwatch.Elapsed;
    }
}
