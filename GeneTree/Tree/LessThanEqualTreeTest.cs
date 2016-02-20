using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace GeneTree
{
	[Serializable]
	public class LessThanEqualTreeTest : TreeTest
	{
		public int param;

		public double valTest;

		//TODO add the ability to test against another value in the data, will work against the balance scale data
		public override TreeTest Copy()
		{
			LessThanEqualTreeTest test_copy = new LessThanEqualTreeTest();
			test_copy.param = this.param;
			test_copy.valTest = this.valTest;
			return test_copy;
		}

		public override bool isTrueTest(DataPoint point)
		{
			return point._data[param]._value <= valTest;
		}

		public override string ToString()
		{
			return string.Format("{0} <= {1}", param, valTest);
		}

		public override bool ChangeTestValue(GeneticAlgorithmManager mgr)
		{
			//try a simple percent test first
			double change = (mgr.rando.NextDouble() * 2 - 1.0) * mgr._gaOptions.test_value_change;
			this.valTest += this.valTest * change;
			return true;
		}

		public override bool CanChangeValue
		{
			get
			{
				return true;
			}
		}
	}
}


