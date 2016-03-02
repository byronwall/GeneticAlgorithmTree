using System;
using System.Collections.Generic;
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
		
		public override double GetTestValue(Random rando)
		{
			//pick a random value from the values
			if (rando.NextDouble() < 0.05)
			{
				return -1;
			}
			else
			{
				return _values[rando.Next(_values.Count)]._value;
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




