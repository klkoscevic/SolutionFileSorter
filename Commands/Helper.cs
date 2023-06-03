using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderProjectsInSlnFile
{
    public class Helper
    {
        public bool IsSlnFileSorted(string solutionFilePath)
        {
            if (string.IsNullOrEmpty(solutionFilePath) || !File.Exists(solutionFilePath))
            {
                System.Windows.Forms.MessageBox.Show($"Solution file '{solutionFilePath}' does not exist.", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Information);
                throw new Exception();
            }

            SolutionFile solutionFileSorted = null;
            SolutionFile solutionFileNotChanged = null;

            using (var reader = new StreamReader(solutionFilePath))
            {
                solutionFileSorted = new SolutionFile(reader);
            }
            using (var reader = new StreamReader(solutionFilePath))
            {
                solutionFileNotChanged = new SolutionFile(reader);
            }

            solutionFileSorted.Sort();

            try
            {
                Assert.IsTrue(solutionFileSorted.LinesInFile.SequenceEqual(solutionFileNotChanged.LinesInFile));
                return true;
            }
            catch
            {
                return false;
            }
        }
    } 
}
