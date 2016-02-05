using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public class DataColumn
	{
		//TODO this enum should probably become an inherited class
		public enum DataValueTypes
		{
			NUMBER,
			CATEGORY
		}
		
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
	
}


