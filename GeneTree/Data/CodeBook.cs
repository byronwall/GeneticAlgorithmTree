using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public class CodeBook
	{
		Dictionary<string, double> _mappings = new Dictionary<string, double>();

		public double GetMapping(string value)
		{
			return _mappings[value];
		}

		public double GetOrAddValue(string rawValue)
		{
			//this is fast enough for now, List is much slower
			if (!_mappings.ContainsKey(rawValue))
			{
				return AddToCodebook(rawValue);
			}
			return _mappings[rawValue];
		}

		private double AddToCodebook(string rawValue)
		{
			double value = _mappings.Count;
			_mappings.Add(rawValue, _mappings.Count);
			return value;
		}
	}
}




