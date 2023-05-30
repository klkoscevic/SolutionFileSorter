using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OrderProjectsInSlnFile.Classes
{
    class ProjectLines
    {
        private readonly List<ProjectLine> projectLines = new List<ProjectLine>();
        private readonly Queue<int> lineNumbers = new Queue<int>();

        public void Enqueue(int lineNumber, ProjectLine projectLine)
        {
            lineNumbers.Enqueue(lineNumber);
            projectLines.Add(projectLine);
        }

        public void Sort()
        {
            projectLines.Sort((line1, line2) => string.Compare(line1.Name, line2.Name, true));
        }

        public Tuple<int, string> Dequeue()
        {
            if (projectLines.Count == 0)
            {
                return null;
            }

            string line = projectLines[0].Line;
            projectLines.RemoveAt(0);
            return Tuple.Create(lineNumbers.Dequeue(), line);
        }

        public void AddProjectName(ProjectLines projectLinesName)
        {
            List<ProjectLine> projectLineNames = projectLinesName.GetProjectLines();

            foreach (ProjectLine projectLine in projectLines)
            {
                foreach (ProjectLine projectLineName in projectLineNames)
                {
                    if(projectLine.GUID == projectLineName.GUID)
                    {
                        projectLine.Name = projectLineName.Name;
                        break;
                    }
                }
            }
        }

        private List<ProjectLine> GetProjectLines()
        {
            return projectLines;
        }
    }
}
