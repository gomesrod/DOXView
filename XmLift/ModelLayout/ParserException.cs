using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmLift.ModelLayout
{
    public class ParserException : Exception
    {
		public ParserException(String msg) : base(msg)
		{
		}

		public ParserException(String msg, Exception cause) : base(msg, cause)
		{
		}
    }
}
