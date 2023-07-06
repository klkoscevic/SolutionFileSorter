using SortingLibrary;
using System;
using System.Globalization;
using System.IO;

namespace ConsoleOrderProjectsInSlnFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No input was given.");
                Console.Read();
                return;
            }

            string solutionFilePath = args[0];
            CultureInfo cultureInfo = null;

            if (args.Length == 2)
            {
                cultureInfo = CultureInfo.GetCultureInfo(args[1]);
            }

            if (!File.Exists(solutionFilePath))
            {
                Console.WriteLine($@"File '{solutionFilePath}' does not exist.");
                Console.Read();
                return;
            }

            SlnProjectsSorter sorter;

            using (var reader = new StreamReader(solutionFilePath))
            {
                if (cultureInfo != null)
                {
                    sorter = new SlnProjectsSorter(reader, cultureInfo);
                }
                else
                {
                    sorter = new SlnProjectsSorter(reader);
                }
            }

            if (!sorter.AlreadySorted)
            {
                using (var writer = new StreamWriter(solutionFilePath))
                {
                    sorter.WriteSorted(writer);
                }
                Console.WriteLine($@"Projects in the .sln file {solutionFilePath} are now sorted alphabetically.");
            }
            else
            {
                Console.WriteLine($@"Projects in the .sln file {solutionFilePath} are already sorted alphabetically.");
            }

            Console.Read();
        }
    }
}
