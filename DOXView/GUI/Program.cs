using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DOXView.Model;
using DOXView.ModelLayout;

/*
 * DOXView
 * 
 * Domain-oriented XML Viewer
 * 
 * */
namespace DOXView.GUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                execRegular();
            }
            else if (args.Length == 2)
            {
                execWithPreloadedXmlAndLayout(args[0], args[1]);
            }
            else
            {
                MessageBox.Show("Invalid arguments");
            }
        }

        private static void execRegular()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void execWithPreloadedXmlAndLayout(string xmlPath, string layoutPath)
        {
            // Currently used for dev tests. TODO handle errors in a user-friendly way
            XmlModel model;

            try
            {
                LayoutParser layoutParser = new LayoutParser();
                Layout layout = layoutParser.parseXmlFile(layoutPath);
                ModelParser parser = new ModelParser(layout);
                model = parser.parseXmlFile(xmlPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + ex.StackTrace);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(model));
        }
    }
}
