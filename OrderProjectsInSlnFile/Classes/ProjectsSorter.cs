using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OrderProjectsInSlnFile
{
    /// <summary>
    /// Sorts project using <c>ProjectEntriesComparer</c> class.
    /// </summary>
    public class ProjectsSorter
    {
        /// <summary>
        /// Initializes instance that uses <c>CultureInfo.CurrentCulture</c> for sorting.
        /// </summary>
        public ProjectsSorter()
        {
        }

        /// <summary>
        /// Initializes instance that uses <c>CultureInfo</c> provided for sorting
        /// </summary>
        /// <param name="cultureInfo">
        /// <c>CultureInfo</c> object to use in comparison.
        /// </param>
        public ProjectsSorter(CultureInfo cultureInfo)
        {
            comparer = new ProjectEntryComparer(cultureInfo);
        }

        private readonly ProjectEntryComparer comparer = new ProjectEntryComparer();

        /// <summary>
        /// Sorts collection of <c><ProjectEntry></c> objects provided.
        /// </summary>
        /// <param name="projects">
        /// Collection to sort.
        /// </param>
        /// <returns>
        /// Sorted collection of <c><ProjectEntry></c> objects.
        /// </returns>
        public IEnumerable<ProjectEntry> GetSorted(IEnumerable<ProjectEntry> projects)
        {
            return projects.OrderBy(p => p, comparer);
        }

        /// <summary>
        /// Checks if collection of <c><ProjectEntry></c> objects provided is sorted.
        /// </summary>
        /// <param name="projects">
        /// Collection to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if collection is sorted, else returns <c>false</c>.
        /// </returns>
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
