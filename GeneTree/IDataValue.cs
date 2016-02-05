using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public class DataValue
	{
		public bool _isMissing;
		public double _value;
		public string _rawValue;
		public DataColumn _parent;
	}
	
	public enum DataValueTypes
	{
		NUMBER,
		CATEGORY
	}
	
	public interface IDataColumn<T>
	{
		T GetPossibleValue();
	}
	public class DataColumn
	{
		public DataValueTypes _type;
		public List<DataValue> _values = new List<DataValue>();
		public string _header;
		public CodeBook _codebook;
		
		public double _min = double.MaxValue;
		public double _max = double.MinValue;
		
		public double GetTestValue()
		{
			return 0.0; //some value from the collection
		}
		
		public void ProcessRanges()
		{
			_min = _values.Min(x => x._value);
			_max = _values.Max(x => x._value);
		}
		
	}
	public class CodeBook
	{
		Dictionary<string, double> _mappings = new Dictionary<string, double>();
		
		public double GetMapping(string value)
		{
			return _mappings[value];
		}
		
		public double GetOrAddValue(string rawValue)
		{
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
		
		public void SetMappings(IEnumerable<string> rawValues)
		{
			foreach (var value in rawValues.Distinct())
			{
				_mappings.Add(value, _mappings.Count);
			}
		}
		
	}
}


