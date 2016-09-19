using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DOXView.ModelLayout;
using System.IO;
using System.Windows.Forms;

namespace DOXView.GUI
{
    public class LayoutManager
    {
        private List<Layout> availableLayouts;

        public void LoadLayouts()
        {
            List<Layout> layouts = loadLayoutsFromDir(getLayoutDir());            

            if (layouts.Count == 0)
            {
                throw new InvalidOperationException("No layout was found");
            }

            availableLayouts = layouts;
        }

        private string getLayoutDir()
        {
            string homeDir;
            if (Environment.OSVersion.Platform.ToString().StartsWith("Win")) {
                string homeDirPart = Environment.GetEnvironmentVariable("HOMEPATH");
                // Removes the \ from the beginning, as it causes unexpected behavior in the Path.Combine method.
                if (homeDirPart.StartsWith("\\"))
                {
                    homeDirPart = homeDirPart.Substring(1);
                }
                homeDir = Path.Combine(Environment.GetEnvironmentVariable("HOMEDRIVE") + "\\", homeDirPart);
            } else {
                homeDir = Environment.GetEnvironmentVariable("HOME");
            }
            
            if (string.IsNullOrEmpty(homeDir))
            {
                // If after all a home dir is not defined,
                // the compiled application uses the path "[app dir]/layouts" 
                return Path.Combine(Application.StartupPath, "layouts");
            }
            else
            {
                return Path.Combine(homeDir, ".DOXView", "layouts");
            }
        }

        private List<Layout> loadLayoutsFromDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                throw new ArgumentException("Directory does not exist: " + dir);
            }

            LayoutParser parser = new LayoutParser();
            List<Layout> result = new List<Layout>();

            foreach (string layoutFile in Directory.EnumerateFiles(dir, "*.xml"))
            {
                Layout l = parser.parseXmlFile(layoutFile);
                result.Add(l);
            }

            return result;
        }

        internal List<Layout> listCompatibleLayouts(TextBox txtXmlPath)
        {
            // TODO - Filter based on the EvaluationXPath, returning only
            // the layouts that match this XML

            return availableLayouts;
        }
    }
}
