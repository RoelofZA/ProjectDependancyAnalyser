using Dependancy.ProjectAnalyser.Engine.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dependancy.ProjectAnalyser.Engine
{
    public class Renderer
    {
        #region Class Variables
        private ArrayList _projectArrayList = new ArrayList(); 
        #endregion

        public void RenderMermaidDependencyGraph(List<CsProject> csProject, string OutputFilePath)
        {
            // Load Resource
            string htmlPage = GetEmbeddedResource("Dependancy.ProjectAnalyser.Engine", "Flow.html");
            StringBuilder stringBuilder = new StringBuilder();

            // Build Dependency Project Array
            BuildProjectHash(csProject);

            // Build Dependency
            foreach (var project in csProject)
            {
                string parentName = _projectArrayList.IndexOf(project.Name) + $"[{project.Name}]";
                foreach (string dependencyProjectName in project.Dependencies)
                {
                    string childName = $"|{dependencyProjectName}|{_projectArrayList.IndexOf(dependencyProjectName)}[{dependencyProjectName}]";
                    stringBuilder.Append($"{Environment.NewLine}\t{parentName} --> {childName}");
                }
            }

            htmlPage = htmlPage.Replace("%GRAPH%", stringBuilder.ToString());

            System.IO.File.WriteAllText(OutputFilePath, htmlPage);
        }
        public void RenderMermaidServiceReferenceGraph(List<CsProject> csProject, string OutputFilePath)
        {
            // Load Resource
            string htmlPage = GetEmbeddedResource("Dependancy.ProjectAnalyser.Engine", "Flow.html");
            StringBuilder stringBuilder = new StringBuilder();

            // Build Dependency Project Array
            BuildProjectHash(csProject);

            // Build Dependency
            foreach (var project in csProject)
            {
                string parentName = _projectArrayList.IndexOf(project.SolutionName) + $"[{project.SolutionName}]";
                foreach (string serviceReference in project.ServiceReferences)
                {
                    string childName = $"|{serviceReference}|{_projectArrayList.IndexOf(serviceReference)}[{serviceReference}]";
                    stringBuilder.Append($"{Environment.NewLine}\t{parentName} --> {childName}");
                }
            }

            htmlPage = htmlPage.Replace("%GRAPH%", stringBuilder.ToString());

            System.IO.File.WriteAllText(OutputFilePath, htmlPage);
        }

        public string GetEmbeddedResource(string ns, string res)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", ns, res))))
            {
                return reader.ReadToEnd();
            }
        }

        private void BuildProjectHash(List<CsProject> csProject)
        {
            foreach (var project in csProject)
            {
                if (!_projectArrayList.Contains(project.Name))
                    _projectArrayList.Add(project.Name);

                foreach (string projectName in project.Dependencies)
                {
                    if (!_projectArrayList.Contains(projectName))
                        _projectArrayList.Add(projectName);
                }

                foreach (string projectName in project.ServiceReferences)
                {
                    if (!_projectArrayList.Contains(projectName))
                        _projectArrayList.Add(projectName);
                }
            }
        }
    }
}
