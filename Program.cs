using System;
using System.IO;
using System.Numerics;
using System.Threading;

namespace GameOfLife
{
    class Program
    {
        static BigInteger seed;
        static int worldSize = 3;

        static void Main(string[] args)
        {
            ParseArgs();

            World world = new World(seed, worldSize);

            while (!world.Stable)
            {
                Console.Clear();
                Console.WriteLine(world);

                world.Advance();

                Thread.Sleep(500);
            }

            Console.Write("World is stable.");
        }

        static void ParseArgs()
        {
            string[] args = Environment.GetCommandLineArgs();

            bool wrongUsage = false;

            var enumerator = args.GetEnumerator();

            enumerator.MoveNext(); //Skip the exe name;

            while(enumerator.MoveNext())
            {
                string arg = (string)enumerator.Current;
                switch (arg)
                {
                    case "--seed":
                    case "-s":
                        if (!enumerator.MoveNext() || !BigInteger.TryParse((string)enumerator.Current, out seed)) 
                            wrongUsage = true;
                        break;
                    case "--world-size":
                    case "-w":
                        if (!enumerator.MoveNext() || !Int32.TryParse((string)enumerator.Current, out worldSize))
                            wrongUsage = true;
                        break;
                    case "--file":
                    case "-f":
                        if (!enumerator.MoveNext() || !ParseWorldFile((string)enumerator.Current, out seed, out worldSize))
                            wrongUsage = true;
                        break;
                    case "--help":
                    case "-h":
                        PrintHelpAndExit(0);
                        break;
                    default:
                        wrongUsage = true;
                        break;
                }
            }

            if(seed == 0 || wrongUsage) PrintHelpAndExit(1);

        }

        static void PrintHelpAndExit(int returnCode)
        {

            string filename = Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            Console.WriteLine(
@"Conway's Game of life simulation
Usage:
    {0} [OPTIONS] ..
                
Command Line Arguments:
    -f, --file <file>         : Parse seed and world size from file, containing rows of 0 and 1
    -s, --seed <num>          : The initial value of the world, written as a decimal number
    -w, --world-size <num>    : Side length of the square world(Default 3)

Example:
    {0} -f world.txt    : Parse seed and world size from 'world.txt'
    {0} -s 23 - w 4     : Creates a 4 x 4 world with the starting value of 23"
           , filename);

            Environment.Exit(returnCode);
        }

        static bool ParseWorldFile(string filename, out BigInteger seed, out int worldSize)
        {
            seed = BigInteger.Zero;
            worldSize = 0;

            try
            {
                string data = File.ReadAllText(filename);

                data = data.Replace("\r", "");
                data = data.Replace("\n", "");

                char[] rev = data.ToCharArray();
                Array.Reverse(rev);
                data = new string(rev);

                int digits = 0;

                foreach (char digit in data)
                {
                    seed <<= 1;
                    seed += digit == '1' ? 1 : 0;
                    digits++;
                }

                worldSize = (int)Math.Sqrt(digits);

                return true;
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException) Console.WriteLine("Error: File not found.");
                return false;
            }
        }


    }
}

