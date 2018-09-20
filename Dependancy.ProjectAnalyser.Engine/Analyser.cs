using Dependancy.ProjectAnalyser.Engine.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Construction;
using System.Collections;

namespace Dependancy.ProjectAnalyser.Engine
{
    public class Analyser
    {
        #region Class Variables
        private Hashtable _solutionHashTable = new Hashtable();
        #endregion

        public List<CsProject> AnalyseFolder(string folderName)
        {
            List<CsProject> csProjects = new List<CsProject>();
            Console.WriteLine($"FolderName: {folderName}");

            BuildSolutionTree(folderName);

            string[] fileArray = Directory.GetFiles(folderName, "*.csproj", SearchOption.AllDirectories);

            Console.WriteLine($"Files found in folder:{Environment.NewLine}{String.Join(Environment.NewLine, fileArray)}");
            

            Console.WriteLine($"Projects:");
            foreach (string projectFolderName in fileArray)
            {
                if (projectFolderName.ToLower().Contains("test"))
                    continue;

                csProjects.Add(AnalyseProjectXML(projectFolderName));
            }

            return csProjects;
        }

        private void BuildSolutionTree(string folderName)
        {
            string[] fileArray = Directory.GetFiles(folderName, "*.sln", SearchOption.AllDirectories);

            foreach (string solutionPath in fileArray)
            {
                var _solutionFile = SolutionFile.Parse(solutionPath);
                string solutionName = solutionPath.Split('\\').Last().Replace(".sln", "");
                foreach(var prj in _solutionFile.ProjectsInOrder.Where(x=>x.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat))
                {
                    if (prj.ProjectName.ToLower().Contains("test"))
                        continue;
                    //Console.WriteLine(prj.ProjectName);
                    if (!_solutionHashTable.ContainsKey(prj.ProjectName))
                        _solutionHashTable.Add(prj.ProjectName, solutionName);
                }
            }
        }

        private CsProject AnalyseProjectXML(string fullPathName)
        {
            CsProject csProject = new CsProject();

            XDocument xmldoc = XDocument.Load(fullPathName);
            XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

            csProject.Name = xmldoc.Descendants(msbuild + "AssemblyName").FirstOrDefault()?.Value;
            if (_solutionHashTable.ContainsKey(csProject.Name))
                csProject.SolutionName = _solutionHashTable[csProject.Name].ToString();
            else
                csProject.SolutionName = csProject.Name;

            // Get Dependencies
            foreach (var resource in xmldoc.Descendants(msbuild + "ProjectReference"))
            {
                string includePath = resource.Descendants(msbuild+"Name").FirstOrDefault()?.Value;
                if (includePath!=null) csProject.Dependencies.Add(includePath);
            }

            // Get ServiceReferences
            foreach (var resource in xmldoc.Descendants(msbuild + "None").Where(x=>x.Attribute("Include").Value.EndsWith(".disco")).ToList())
            {
                string serviceReference = resource.Attribute("Include").Value;
                serviceReference = serviceReference.Split('\\').Last().Replace(".disco", "");
                serviceReference = Regex.Replace(serviceReference, @"[0-9]", "");

                if (!csProject.ServiceReferences.Contains(serviceReference))
                    csProject.ServiceReferences.Add(serviceReference);
            }

            // Write Project Details
            Console.WriteLine($"{Environment.NewLine}Project Name:{Environment.NewLine}\t{csProject.Name}");
            if (csProject.Dependencies.Count > 0)
                Console.WriteLine($"Project Dependencies:{Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", csProject.Dependencies)}");
            else
                Console.WriteLine($"Project Dependencies:{Environment.NewLine}\tNone");

            if (csProject.ServiceReferences.Count > 0)
                Console.WriteLine($"Project Service Reference:{Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", csProject.ServiceReferences)}");
            else
                Console.WriteLine($"Project Service Reference:{Environment.NewLine}\tNone");

            return csProject;
        }
    }
}
