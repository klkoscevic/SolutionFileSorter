using System.Collections.Generic;

namespace OrderProjectsInSlnFile
{
    public class ProjectEntryComparer : IComparer<ProjectEntry>
    {
        public int Compare(ProjectEntry x, ProjectEntry y)
        {
            int result = string.Compare(x.GetParentPath(), y.GetParentPath(), true);
            return result != 0 ? result : string.Compare(x.Name, y.Name);
        }
    }
}