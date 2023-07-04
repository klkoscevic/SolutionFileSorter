using SortingLibrary;
using System;
using System.IO;

namespace ConsoleOrderProjectsInSlnFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 && string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("Invalid input. Please provide a valid path to the .sln file.");
                return;
            }

            string solutionFilePath = args[0];

            SlnProjectsSorter sorter;

            using (var reader = new StreamReader(solutionFilePath))
            {
                sorter = new SlnProjectsSorter(reader);
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
