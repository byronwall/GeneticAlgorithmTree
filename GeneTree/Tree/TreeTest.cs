using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
			
			var col_param = rando.Next(dataPointMgr._columns.Count);
			DataColumn column = dataPointMgr._columns[col_param];
			
			double prob_missing_test = 0.3;
			double prob_test_category_subset = 0.25;
			
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
					var cat_column = column as CategoryDataColumn;
					
					var categories = cat_column._codebook.GetCategories();
					
					var category_count = categories.Count();
					//toss a coin to decide on subsetter
					if (category_count >= 3 && rando.NextDouble() < prob_test_category_subset)
					{
						var subset_test = new CategorySubsetTreeTest();
						subset_test._param = col_param;
						
						//ensure at least two choices here... otherwise defaults to equality
						int categories_to_keep = 2 + rando.Next(category_count - 2);
						subset_test._values.AddRange(categories.OrderBy(c => rando.NextDouble()).Take(categories_to_keep));
						
						return subset_test;
					}
					else
					{
						EqualTreeTest test_eq = new EqualTreeTest();					
						test_eq._param = col_param;					
						test_eq._valTest = column.GetTestValue(rando);
						return test_eq;
					}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}