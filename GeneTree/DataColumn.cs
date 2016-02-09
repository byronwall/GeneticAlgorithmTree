using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public abstract class DataColumn
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
		
		public abstract double GetTestValue(Random rando);
		
		
		public void ProcessRanges()
		{
			_min = _values.Min(x => x._value);
			_max = _values.Max(x => x._value);
		}
		
	}
	public class DoubleDataColumn : DataColumn
	{
		public override double GetTestValue(Random rando)
		{
			return rando.NextDouble() * (_max - _min) + _min;
		}
		
		public DoubleDataColumn()
		{
			this._type = DataValueTypes.NUMBER;
		}
	}
	public class CategoryDataColumn : DataColumn
	{
		#region implemented abstract members of DataColumn
		public override double GetTestValue(Random rando)
		{
			//pick a random value from the values
			return _values[rando.Next(_values.Count)]._value;
		}
		#endregion
		
		public CategoryDataColumn()
		{
			this._codebook = new CodeBook();
			this._type = DataValueTypes.CATEGORY;
		}
		
	}
	
}


