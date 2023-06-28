﻿using System;

namespace OrderProjectsInSlnFile
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
            return Start == other.Start && End == other.End;
        }

        public static readonly Range Empty = new Range(0, 0);
    }
}