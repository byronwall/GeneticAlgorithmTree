using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MoreLinq;
namespace GeneTree
{
	public class LinearComboTreeTest : TreeTest
	{
		//need to decide on two parameters
		public double scaling = 0.0;
		public double intercept = 0.0;

		public int param1;

		public int param2;

		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			LinearComboTreeTest test_copy = new LinearComboTreeTest();
			test_copy.scaling = this.scaling;
			test_copy.intercept = this.intercept;
			test_copy.param1 = this.param1;
			test_copy.param2 = this.param2;
			return test_copy;
		}

		public double GetValue(DataPoint point)
		{
			return point._data[param1]._value * this.scaling + point._data[param2]._value;
		}

		public override bool isTrueTest(DataPoint point)
		{
			return GetValue(point) >= intercept;
		}

		public override bool IsMissingTest(DataPoint point)
		{
			return point._data[this.param1]._isMissing || point._data[this.param2]._isMissing;
		}

		public override string ToString()
		{
			return string.Format("{0:0.000} * {2}+ {3} + {1:0.000} >=0", scaling, intercept, param1, param2);
		}
		#endregion
	}
}

