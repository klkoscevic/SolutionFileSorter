using OrderProjectsInSlnFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Encoding encoding = null;
            SolutionFile slnFile = null;
            using (var reader = new StreamReader(solutionFilePath))
            {
                slnFile = new SolutionFile(reader);
                encoding = reader.CurrentEncoding;
            }

            slnFile.Sort();

            using (var writer = new StreamWriter(solutionFilePath, false, encoding))
            {
                foreach (var line in slnFile.LinesInFile)
                {
                    writer.WriteLine(line);
                }
                writer.Flush();
            }

            Console.WriteLine("Sorting complete.");
        }
    }
}
