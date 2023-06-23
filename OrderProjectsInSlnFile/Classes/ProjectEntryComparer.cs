using System.Collections.Generic;
using System.Globalization;

namespace OrderProjectsInSlnFile
{
    /// <summary>
    /// Compares two <c>ProjectEntry</c> items. If both items are solution folders or both items are projects, comparison is made by 
    /// their names. If one of items is solution folder and the other is project, comparison places folder before project.
    /// </summary>
    public class ProjectEntryComparer : IComparer<ProjectEntry>
    {
        /// <summary>
        /// Initializes comparer using <c>CultureInfo.CurrentCulture</c>.
        /// </summary>
        public ProjectEntryComparer()
        {
        }

        /// <summary>
        /// Initializes comparer using <c>CultureInfo</c> provided.
        /// </summary>
        /// <param name="cultureInfo">
        /// <c>CultureInfo</c> object to use in comparison.
        /// </param>
        public ProjectEntryComparer(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        private readonly CultureInfo cultureInfo = CultureInfo.CurrentCulture;

        /// <summary>
        /// Compares two <c>ProjectEntry</c> items.
        /// </summary>
        /// <param name="x">First item.</param>
        /// <param name="y">Second item.</param>
        /// <returns>
        /// Returns positive integer if the first item must follow the second, negative integer if 
        /// the first item must preceed the second and 0 if order is undefined.
        /// </returns>
        public int Compare(ProjectEntry x, ProjectEntry y)
        {
            var xPath = x.GetFullPath();
            var yPath = y.GetFullPath();
            while (xPath.Count > 0 && yPath.Count > 0)
            {
                var xEntry = xPath.Pop();
                var yEntry = yPath.Pop();
                // If one of entries is solution folder (and another is project) then solution
                // folder must always come before project, regardless of alphabetical order.
                if (xEntry.IsSolutionFolder != yEntry.IsSolutionFolder)
                { 
                    return xEntry.IsSolutionFolder ? -1 : +1; 
                }
                // For entries of the same type, comparison is done alphabetically.
                var compare = string.Compare(xEntry.Name, yEntry.Name, cultureInfo, CompareOptions.IgnoreCase);
                if (compare != 0)
                {
                    return compare;
                }
            }
            return xPath.Count - yPath.Count;
        }
    }
}
