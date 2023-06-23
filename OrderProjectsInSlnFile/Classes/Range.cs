namespace OrderProjectsInSlnFile
{
    /// <summary>
    /// Represents a range in a sequence with start and end positions. 
    /// </summary>
    public sealed class Range : IEquatable<Range>
    {
        /// <summary>
        /// Initializes range from <c>start</c> to (excluding) <c>end</c>.
        /// </summary>
        /// <param name="start">
        /// Position of range start.
        /// </param>
        /// <param name="end">
        /// Position of range end. If <c>end</c> is equal to <c>start</c>, range will be empty.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If both arguments are negative or if <c>start</c> is less then <c>end</c>.
        /// </exception>
        public Range(int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException($"{nameof(start)} must be less than or equal to {nameof(end)}");
            }
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start position of the range.
        /// </summary>
        public int Start { get; private set; }

        /// <summary>
        /// Gets the end position of the range (index of element after the last one).
        /// </summary>
        public int End { get; private set; }

        /// <summary>
        /// Identifies if the range is empty.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if <c>end</c> is equal to <c>start</c>, else returns <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            return Start == End;
        }

        /// <summary>
        /// Checks ranges for equality.
        /// </summary>
        /// <param name="other">The range to compare with.</param>
        /// <returns>
        /// <c>true</c> if <c>start</c> and <c>end</c> values are eqaul for both ranges, respectively.
        /// </returns>
        public bool Equals(Range other)
        {
            return Start == other.Start && End == other.End;
        }

        /// <summary>
        /// Empty range object.
        /// </summary>
        public static readonly Range Empty = new Range(0, 0);
    }
}
