using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public abstract class TreeTest
	{
		public abstract TreeTest Copy();
		public abstract bool isTrueTest(DataPoint point);
		
		public static TreeTest TreeTestFactory(DataPointManager dataPointMgr, Random rando)
		{			
			var col_param = rando.Next(dataPointMgr.DataColumnCount);
			DataColumn column = dataPointMgr._columns[col_param];
			
			switch (column._type)
			{
				case DataColumn.DataValueTypes.NUMBER:
					LessThanEqualTreeTest test = new LessThanEqualTreeTest();					
					test.param = col_param;					
					test.valTest = column.GetTestValue(rando);
					return test;
					
					break;
				case DataColumn.DataValueTypes.CATEGORY:
					EqualTreeTest test_eq = new EqualTreeTest();					
					test_eq._param = col_param;					
					test_eq._valTest = column.GetTestValue(rando);
					return test;
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
	
	public class EqualTreeTest : TreeTest
	{
		public double _valTest;
		public int _param;
		
		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			throw new NotImplementedException();
		}
		public override bool isTrueTest(DataPoint point)
		{
			return point._data[_param]._value == _valTest;
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("{0} == {1}", param, valTest);
		}
	}
	
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
	}
}
