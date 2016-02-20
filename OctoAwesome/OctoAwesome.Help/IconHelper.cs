using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputDir = args[0];

            var htmlFiles = Directory.GetFiles(Path.Combine(outputDir, "html"));

            foreach (var file in htmlFiles)
            {
                var content = File.ReadAllText(file);
                content = content.Replace("icons/", "images/");
                File.WriteAllText(file, content);
            }

            var cssFiles = Directory.GetFiles(Path.Combine(outputDir, "styles"));

            foreach (var file in cssFiles)
            {
                var content = File.ReadAllText(file);
                content = content.Replace("icons/", "images/");
                File.WriteAllText(file, content);
            }

            var imgFiles = Directory.GetFiles(Path.Combine(outputDir, "icons"));

            foreach (var file in imgFiles)
            {
                if (file.EndsWith(".ico"))
                    continue;

                File.Move(file, Path.Combine(outputDir, "images", Path.GetFileName(file)));
            }

            Directory.Delete(Path.Combine(outputDir, "icons"), true);
        }
    }
}
