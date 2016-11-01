using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmLift.ModelLayout;
using System.IO;
using System.Windows.Forms;

namespace XmLift.GUI
{
    public class LayoutManager
    {
        private List<Layout> availableLayouts;

        public void LoadLayouts()
        {
			List<Layout> layouts = new List<Layout>();

			foreach (string ldir in getLayoutDirs()) {
				if (Directory.Exists(ldir)) {
					layouts.AddRange (loadLayoutsFromDir(ldir));
				}
			}

            availableLayouts = layouts;
        }

		// Return the suitable layout dirs.
		// Currently this set is made of:
		// - a global layout directory (application dir / layouts)
		// - a personal layout directory, if one exists (user home / .XmLift / layouts)
        public string[] getLayoutDirs()
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
				// If a home dir cannot be discovered, return only the global location
				return new string[]{Path.Combine(Application.StartupPath, "layouts")};
            }
            else
            {
				return new string[]{Path.Combine(homeDir, ".XmLift", "layouts"), 
					Path.Combine(Application.StartupPath, "layouts")};
            }
        }

        private List<Layout> loadLayoutsFromDir(string dir)
        {
            LayoutParser parser = new LayoutParser();
            List<Layout> result = new List<Layout>();

            foreach (string layoutFile in Directory.EnumerateFiles(dir, "*.xml"))
            {
                Layout l = parser.parseXmlFile(layoutFile);
                result.Add(l);
            }

            return result;
        }

        internal List<Layout> listCompatibleLayouts(string txtXmlPath)
        {
            // TODO - Filter based on the EvaluationXPath, returning only
            // the layouts that match this XML

            return availableLayouts;
        }
    }
}
