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
		public virtual bool ChangeTestValue(GeneticAlgorithmManager mgr)
		{
			return false;
		}
		
		public static TreeTest TreeTestFactory(GeneticAlgorithmManager ga_mgr)
		{			
			return TreeTestFactory(ga_mgr.dataPointMgr, ga_mgr.rando);
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
}
