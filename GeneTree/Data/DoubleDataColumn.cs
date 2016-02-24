using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public class DoubleDataColumn : DataColumn
	{
		public double _min = double.MaxValue;
		public double _max = double.MinValue;
		
		public override double GetTestValue(Random rando)
		{
			return rando.NextDouble() * (_max - _min) + _min;
		}

		public DoubleDataColumn()
		{
			this._type = DataValueTypes.NUMBER;
		}
		
		public override void ProcessRanges()
		{
			_min = _values.Min(x => x._value);
			_max = _values.Max(x => x._value);
		}
		public override string ToString()
		{
			return string.Format("[DoubleDataColumn Min={0}, Max={1}, Header={2}]", _min, _max, this._header);
		}

	}
}




