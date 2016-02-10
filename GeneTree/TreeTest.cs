using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public class TreeTest
	{
		public int param;
		//data array index to test
		public double valTest;

		//TODO add the ability to test against another value in the data, will work against the balance scale data
        
		public TreeTest Copy()
		{
			TreeTest test_copy = new TreeTest();
			
			test_copy.param = this.param;
			test_copy.valTest = this.valTest;
			
			return test_copy;
		}

		public bool isTrueTest(DataPoint point)
		{
			return point._data[param]._value <= valTest;
            
		}

		public override string ToString()
		{
			return string.Format("param {0} <= {1}", param, valTest);
		}
	}
}
