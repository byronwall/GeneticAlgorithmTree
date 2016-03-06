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

		public IEnumerable<string> GetCategoryNames()
		{
			return _mappings.Keys;
		}
		
		public double GetMapping(string value)
		{
			return _mappings[value];
		}
		
		public IEnumerable<double> GetCategories()
		{
			return this._mappings.Values;
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
		
		public override string ToString()
		{
			return string.Join(",", _mappings.OrderBy(c => c.Value).Select(c => c.Key));
		}
		public void PopulateFromString(string data)
		{
			//this allows for things to be input before they are needed
			//TODO can this approach be used to short circuit data loading?
			var parts = data.Split(',');
			
			foreach (var part in parts)
			{
				AddToCodebook(part);
			}
		}
	}
}




