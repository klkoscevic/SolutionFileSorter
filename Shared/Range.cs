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

using System;

namespace KKoščević.SolutionFileSorter.Shared
{
    public class Range : IEquatable<Range>
    {
        public Range(int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException($"{nameof(start)} must be less than or equal to {nameof(end)}");
            }
            Start = start;
            End = end;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        public bool IsEmpty()
        {
            return Start == End;
        }

        public bool Equals(Range other)
        {
            return other != null && Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Range);
        }

        public override int GetHashCode()
        {
            return (Start, End).GetHashCode();
        }

        public static readonly Range Empty = new Range(0, 0);
    }
}