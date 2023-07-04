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