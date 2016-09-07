using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.Model
{
    public class XmlModelDataTable
    {
        public string Title { get; private set; }
        public List<Dictionary<string,string>> Records { get; private set; }

        public XmlModelDataTable(string title, List<Dictionary<string, string>> records)
        {
            Title = title;
            Records = records;
        }
    }
}
