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

        private readonly CultureInfo cultureInfo = CultureInfo.CurrentCulture;

        public int Compare(ProjectEntry x, ProjectEntry y)
        {
            var xPath = x.GetFullPath();
            var yPath = y.GetFullPath();
            while (xPath.Count > 0 && yPath.Count > 0)
            {
                var xEntry = xPath.Pop();
                var yEntry = yPath.Pop();
                if (xEntry.IsSolutionFolder != yEntry.IsSolutionFolder)
                { 
                    return xEntry.IsSolutionFolder ? -1 : +1; 
                }
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
