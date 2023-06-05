using System.Collections.Generic;

namespace OrderProjectsInSlnFile
{
    public class ProjectEntryComparer : IComparer<ProjectEntry>
    {
        public int Compare(ProjectEntry x, ProjectEntry y)
        {
            return string.Compare(x.GetFullPath(), y.GetFullPath(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
