/*
MIT License

Copyright(c) 2023 Klara Koščević

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using KKoščević.SolutionFileSorter.Shared;
using System.Globalization;

namespace KKoščević.SolutionFileSorter.ConsoleApplication
{
    internal class Program
    {
        private enum Result
        {
            OK = 0,
            NoArguments = 1,
            FileNotFound = 2,
            InvalidCulture = 3,
            FileCorrupted = 4
        }

        static int Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("No input was given.");
                Console.WriteLine("Use '/?' for help.");
                return (int)Result.NoArguments;
            }

            if (args[0] == "/?")
            {
                DisplayHelp();
                return (int)Result.NoArguments;

            }

            string solutionFilePath = args[0];

            CultureInfo cultureInfo = null;

            if (!File.Exists(solutionFilePath))
            {
                Console.WriteLine($@"File '{solutionFilePath}' does not exist. Sorting won't happen.");
                return (int)Result.FileNotFound;

            }

            if (args.Length == 2)
            {
                try
                {
                    cultureInfo = CultureInfo.GetCultureInfo(args[1]);
                }
                catch (CultureNotFoundException)
                {
                    Console.WriteLine($@"'{args[1]}' is invalid culture. Sorting won't happen.");
                    return (int)Result.InvalidCulture;
                }
            }

            SlnProjectsSorter sorter;
            try
            {
                using (var reader = new StreamReader(solutionFilePath))
                {
                    sorter = cultureInfo != null ? new SlnProjectsSorter(reader, cultureInfo) : new SlnProjectsSorter(reader);
                }

                if (!sorter.AlreadySorted)
                {
                    using (var writer = new StreamWriter(solutionFilePath))
                    {
                        var backupFilePath = solutionFilePath + ".bak";
                        File.Copy(solutionFilePath, backupFilePath, true);

                        sorter.WriteSorted(writer);
                    }
                    Console.WriteLine($@"Projects in the .sln file {solutionFilePath} are now sorted alphabetically.");
                    return (int)Result.OK;

                }
                else
                {
                    Console.WriteLine($@"Projects in the .sln file {solutionFilePath} are already sorted alphabetically.");
                    return (int)Result.OK;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (int)Result.FileCorrupted;
            }
        }

        static void DisplayHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("SolutionFileSorter.exe <path-to-sln-file> <culture-info> [<culture-info>]");
            Console.WriteLine(" - Sorts the projects within a .sln file.");
            Console.WriteLine(" - <culture-info> is optional. Default is invariant culture.");
        }
    }
}
