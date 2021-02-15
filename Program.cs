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
    -s, --seed<num>        : REQUIRED, The initial value of the world, written as a decimal number
    -w, --world - size<num>  : Side length of the square world(Default 3)
                
Example:
    {0} -s 23 - w 4     : Creates a 4 x 4 world with the starting value of 23"
           , filename);

            Environment.Exit(returnCode);
        }

    }
}
