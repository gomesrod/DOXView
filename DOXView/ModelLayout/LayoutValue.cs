using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOXView.ModelLayout
{
    public class LayoutValue
    {
          public string Description {get; private set;}
          public string XPath { get; private set; }
          public Boolean Required { get; private set; }

          public LayoutValue(string desc, string path, Boolean req)
          {
              Description = desc;
              XPath = path;
              Required = req;
          }

          public LayoutValue(string desc, string path)
          {
              Description = desc;
              XPath = path;
              Required = false;
          }
    }
}
