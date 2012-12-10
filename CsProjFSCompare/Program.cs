using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace CsProjFSCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            var basePath = Path.GetDirectoryName(path);

            var files = Directory.EnumerateFiles(basePath, "*", SearchOption.AllDirectories)
                .Where(x=>!x.StartsWith(Path.Combine(basePath, "bin")))
                .Where(x => !x.StartsWith(Path.Combine(basePath, "obj")))
                .Where(x=>!x.Equals(path))
                .ToList();

            var docNav = new XPathDocument(path);
            var nav = docNav.CreateNavigator();
            var namespaceManager = new XmlNamespaceManager(nav.NameTable);
            namespaceManager.AddNamespace("pr", "http://schemas.microsoft.com/developer/msbuild/2003");

            nav.MoveToRoot();

            var selections = nav.Select("pr:Project/pr:ItemGroup/pr:*/@Include", namespaceManager);
            
            foreach (XPathNavigator item in selections)
            {
                var itemPath = Path.Combine(basePath, item.Value);
                if (files.Contains(itemPath))
                {
                    files.Remove(itemPath);
                }
            }

            Console.WriteLine("There are {0} files in the directory that are not in the project", files.Count);
            
            foreach (var file in files)
            {
                var filePath = file;

                Console.WriteLine(file.Remove(0, basePath.Length));
            }
        }
    }
}
