using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KevinComponent.Exceptions
{
	public class ParseException : Exception
	{
		public ParseException(string message)
			: base(message)
		{
		}
	}
}
