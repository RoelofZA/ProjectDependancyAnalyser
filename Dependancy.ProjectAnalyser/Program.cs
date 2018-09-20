using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dependancy.ProjectAnalyser.Engine;

namespace Dependancy.ProjectAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyser analyser = new Analyser();
            //var projects = analyser.AnalyseFolder(@"C:\Users\roelofb\source\repos\Dependancy.ProjectAnalyser");
            //var projects = analyser.AnalyseFolder(@"C:\DevOps\CODE\SA\APIs\Captiv8.API");
            //var projects = analyser.AnalyseFolder(@"C:\DevOps\CODE\SA\WCFServices");
            var projects = analyser.AnalyseFolder(@"C:\DevOps\CODE\SA\WCFServices\CreditDecisionService");

            Renderer renderer = new Renderer();
            renderer.RenderMermaidDependencyGraph(projects, @"C:\Users\roelofb\source\repos\Dependancy.ProjectAnalyser\test2.html");
            renderer.RenderMermaidServiceReferenceGraph(projects, @"C:\Users\roelofb\source\repos\Dependancy.ProjectAnalyser\service2.html");

            //Console.ReadLine();
        }
    }
}
