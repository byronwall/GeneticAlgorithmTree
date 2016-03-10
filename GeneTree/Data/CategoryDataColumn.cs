using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
namespace GeneTree
{
	public class CategoryDataColumn : DataColumn
	{
		public CodeBook _codebook;
		
		private List<double> _testValues;
		
		public override double GetTestValue(Random rando)
		{
			//pick a random value from the values
			if (rando.NextDouble() < 0.05)
			{
				return -1;
			}
			else
			{
				if (_testValues == null)
				{
					if (_hasMissingValues)
					{
					
						_testValues = _values.Where(c => !c._isMissing).Select(c => c._value).ToList();
					}
					else
					{
						_testValues = _values.Select(c => c._value).ToList();
					}
					
				}
				
				return _testValues[rando.Next(_testValues.Count)];
			}
		}

		public CategoryDataColumn()
		{
			this._codebook = new CodeBook();
			this._type = DataValueTypes.CATEGORY;
		}
		
		public override string GetSummaryString()
		{
			return string.Format("[CategoryDataColumn Header={0}, Categories={1}]", this._header, 
				this._codebook.GetCategoryNames().ToDelimitedString(","));
		}

	}
}




