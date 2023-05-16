using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OrderProjectsInSlnFile.Classes
{
    class ProjectLine
    {
        public string Line { get; set; }
        public string Name { get; set; }
    }
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
    }
}
