using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmLift.ModelLayout
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
            public string Name {get; private set;}
			public string ValueXPath { get; private set; }

            public Column(string name, string xpath)
            {
                Name = name;
				ValueXPath = xpath;
            }
        }
    }
}
