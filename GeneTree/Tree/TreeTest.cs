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
	[Serializable]
	[XmlInclude(typeof(EqualTreeTest))]
	[XmlInclude(typeof(LessThanEqualTreeTest))]
	[XmlInclude(typeof(MissingTreeTest))]
	[XmlInclude(typeof(CategorySubsetTreeTest))]
	[XmlInclude(typeof(LinearComboTreeTest))]
	public abstract class TreeTest
	{
		public virtual bool CanChangeValue{ get { return false; } }
		
		[XmlIgnore]
		public DataColumn _testCol;
		
		public abstract TreeTest Copy();
		public abstract bool isTrueTest(DataPoint point);
		public virtual bool IsMissingTest(DataPoint point)
		{
			return false;
		}
		
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
			//TODO this shoudl take a ga_mgr instead of the parts
			//TODO clean up this mess once the DataColumns quit using the TYPE part
			
			TreeTest output;
			
			var col_param = rando.Next(dataPointMgr._columns.Count);
			DataColumn column = dataPointMgr._columns[col_param];
			
			switch (column._type)
			{
				case DataColumn.DataValueTypes.NUMBER:
					LessThanEqualTreeTest test = new LessThanEqualTreeTest();
					test.param = col_param;
					test.valTest = column.GetTestValue(rando);
					output = test;
					
					break;
				case DataColumn.DataValueTypes.CATEGORY:
					var cat_column = column as CategoryDataColumn;
					
					var categories = cat_column._codebook.GetCategories();
					
					var category_count = categories.Count();
					//toss a coin to decide on subsetter
					EqualTreeTest test_eq = new EqualTreeTest();
					test_eq._param = col_param;
					test_eq._valTest = column.GetTestValue(rando);
					output = test_eq;
					break;
					
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			output._testCol = column;
			return output;
		}
	}
}