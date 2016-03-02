using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public class DataPoint
	{
		public List<DataValue> _data;
		public DataValue _classification;
		
		public static DataPoint FromString(string[] raw_data, List<DataColumn> columns)
		{			
			DataPoint dp = new DataPoint();
			
			//need to create a data point and deal with the types
			dp._data = new List<DataValue>();
			
			for (int i = 0; i < raw_data.Length; i++)
			{
				var value = raw_data[i];
				
				bool isClassification = false;
				if (i == raw_data.Length - 1)
				{
					isClassification = true;
				}
				
				var dv = new DataValue();
				
				switch (columns[i]._type)
				{
				//TODO abstract this code into multiple classes						
					case DataColumn.DataValueTypes.NUMBER:					
						if (value == string.Empty || !double.TryParse(value, out dv._value))
						{
							dv._isMissing = true;
							columns[i]._hasMissingValues = true;
						}						
						
						if (isClassification)
						{
							dp._classification = dv;
						}
						else
						{
							dp._data.Add(dv);
						}
						
						break;
					case DataColumn.DataValueTypes.CATEGORY:				
						//TODO add a check here for a missing value, String.empty
						
						if (value == string.Empty)
						{
							dv._isMissing = true;
							columns[i]._hasMissingValues = true;
						}
						
						var dataColumn = columns[i] as CategoryDataColumn;
						dv._value = dataColumn._codebook.GetOrAddValue(value);
						
						if (!isClassification)
						{
							dp._data.Add(dv);
						}
						else
						{
							dp._classification = dv;
						}
						
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				
				columns[i]._values.Add(dv);
			}
			
			return dp;
		}
	}
}
