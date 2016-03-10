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
	public class CategorySubsetTreeTest : TreeTest
	{
		public List<double> _values = new List<double>();

		public int _param;

		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			CategorySubsetTreeTest test_copy = new CategorySubsetTreeTest();
			test_copy._values.AddRange(this._values);
			test_copy._param = this._param;
			test_copy._testCol = this._testCol;
			return test_copy;
		}

		public override bool isTrueTest(DataPoint point)
		{
			return _values.Contains(point._data[_param]._value);
		}

		#endregion
		public override string ToString()
		{
			return string.Format("{0} in {1}", _param, _values.ToDelimitedString("|"));
		}

		public override bool ChangeTestValue(GeneticAlgorithmManager mgr)
		{
			//TODO this should only add/remove a single (or up to X) categories
			this._values.Clear();
			var col = mgr.dataPointMgr._columns[this._param] as CategoryDataColumn;
			int category_count = col._codebook.GetCategories().Count();
			int categories_to_keep = 2 + mgr.rando.Next(category_count - 2);
			this._values.AddRange(col._codebook.GetCategories().OrderBy(c => mgr.rando.NextDouble()).Take(categories_to_keep));
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


