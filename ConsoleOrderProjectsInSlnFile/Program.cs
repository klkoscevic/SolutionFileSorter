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
