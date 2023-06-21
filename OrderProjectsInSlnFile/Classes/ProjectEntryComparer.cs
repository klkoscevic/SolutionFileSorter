using System.Collections.Generic;
using System.Globalization;

namespace OrderProjectsInSlnFile
{
    public class ProjectEntryComparer : IComparer<ProjectEntry>
    {
        public ProjectEntryComparer()
        {
        }

        public ProjectEntryComparer(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        private readonly CultureInfo cultureInfo = CultureInfo.CurrentUICulture;

        public int Compare(ProjectEntry x, ProjectEntry y)
        {
            return string.Compare(x.GetFullPath(), y.GetFullPath(), cultureInfo, CompareOptions.IgnoreCase);
        }
    }
}
