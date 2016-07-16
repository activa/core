using System;
using System.Linq;
using System.Text;

namespace Velox.Core
{
    public class SafeStringDictionary<T> : SafeDictionary<string,T>
	{
		public SafeStringDictionary()
		{
		}

		public SafeStringDictionary(bool ignoreCase) : base(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal)
		{
		}
	}
}
