using System.Collections.Generic;
using System.Linq;

namespace OrderProjectsInSlnFile
{
    public static class ProjectsSorter
    {
        private static ProjectEntryComparer comparer = new ProjectEntryComparer();

        public static IEnumerable<ProjectEntry> GetSorted(IEnumerable<ProjectEntry> projects)
        {
            return projects.OrderBy(p => p, comparer);
        }

        public static bool IsSorted(IEnumerable<ProjectEntry> projects)
        {
            if (projects.Count() < 2)
            {
                return true;
            }
            return !projects.Zip(projects.Skip(1), (a, b) => comparer.Compare(a, b) <= 0).Contains(false);
        }
    }
}