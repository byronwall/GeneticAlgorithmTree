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
	[XmlInclude(typeof(EqualTreeTest))]
	[XmlInclude(typeof(LessThanEqualTreeTest))]
	[XmlInclude(typeof(MissingTreeTest))]
	public abstract class TreeTest
	{
		public virtual bool CanChangeValue{ get { return false; } }
		
		public abstract TreeTest Copy();
		public abstract bool isTrueTest(DataPoint point);
		public virtual bool ChangeTestValue(GeneticAlgorithmManager mgr, Random rando)
		{
			return false;
		}
		
		public static TreeTest TreeTestFactory(DataPointManager dataPointMgr, Random rando)
		{			
			//TODO clean up this mess once the DataColumns quit using the TYPE part
			
			var col_param = rando.Next(dataPointMgr.DataColumnCount);
			DataColumn column = dataPointMgr._columns[col_param];
			
			double prob_missing_test = 0.3;
			
			if (column._hasMissingValues && rando.NextDouble() < prob_missing_test)
			{
				MissingTreeTest test = new MissingTreeTest();
				test._param = col_param;					
				return test;
			}
			
			switch (column._type)
			{
				case DataColumn.DataValueTypes.NUMBER:					
					LessThanEqualTreeTest test = new LessThanEqualTreeTest();
					test.param = col_param;					
					test.valTest = column.GetTestValue(rando);
					return test;
				case DataColumn.DataValueTypes.CATEGORY:
					EqualTreeTest test_eq = new EqualTreeTest();					
					test_eq._param = col_param;					
					test_eq._valTest = column.GetTestValue(rando);
					return test_eq;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
	
	[Serializable]
	public class MissingTreeTest : TreeTest
	{
		public int _param;
		
		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			var test_copy = new MissingTreeTest();
			
			test_copy._param = this._param;
			
			return test_copy;
		}
		public override bool isTrueTest(DataPoint point)
		{
			return point._data[_param]._isMissing;
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("{0} missing", _param);
		}
	}
	
	[Serializable]
	public class EqualTreeTest : TreeTest
	{
		//TODO create a test for doing group subset or a multi group test, ensure that not all groups end up in it, N-1 max
		
		public double _valTest;
		public int _param;
		
		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			var test_copy = new EqualTreeTest();
			
			test_copy._param = this._param;
			test_copy._valTest = this._valTest;
			
			return test_copy;
		}
		public override bool isTrueTest(DataPoint point)
		{
			return point._data[_param]._value == _valTest;
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("{0} == {1}", _param, _valTest);
		}
	}
	
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
		
		public override bool ChangeTestValue(GeneticAlgorithmManager mgr, Random rando)
		{
			//try a simple percent test first
			double change = (rando.NextDouble() * 2 - 1.0) * mgr._gaOptions.test_value_change;
			
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
