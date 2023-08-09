﻿/*
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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SortingLibrary
{
    public class ProjectsSorter
    {
        public ProjectsSorter()
        {
        }

        public ProjectsSorter(CultureInfo cultureInfo)
        {
            comparer = new ProjectEntryComparer(cultureInfo);
        }

        private readonly ProjectEntryComparer comparer = new ProjectEntryComparer();

        public IEnumerable<ProjectEntry> GetSorted(IEnumerable<ProjectEntry> projects)
        {
            return projects.OrderBy(p => p, comparer);
        }

        public bool IsSorted(IEnumerable<ProjectEntry> projects)
        {
            if (projects.Count() < 2)
            {
                return true;
            }
            return !projects.Zip(projects.Skip(1), (a, b) => comparer.Compare(a, b) <= 0).Contains(false);
        }
    }
}