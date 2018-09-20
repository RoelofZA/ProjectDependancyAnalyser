using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependancy.ProjectAnalyser.Engine.Entities
{
    public class CsProject
    {
        public string SolutionName { get; set; } = "";
        public string Name { get; set; } = "";
        public List<string> Dependencies { get; set; } = new List<string>();
        public List<string> ServiceReferences { get; set; } = new List<string>();
    }
}
