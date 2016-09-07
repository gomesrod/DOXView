using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.ModelLayout
{
    public class LayoutDataTable
    {
        public string Title { get; private set; }
        public string RecordXPath { get; private set; }
        public List<Column> Columns { get; private set; }

        public LayoutDataTable(string title, string xpath, List<Column> cols)
        {
            Title = title;
            RecordXPath = xpath;
            Columns = cols;
        }

        public class Column
        {
            public string Description {get; private set;}
            public string XPath { get; private set; }

            public Column(string desc, string path)
            {
                Description = desc;
                XPath = path;
            }
        }
    }
}
