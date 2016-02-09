using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public class DataPoint
	{
		public IEnumerable<string> _rawData;
		public List<DataValue> _data;
		public DataValue _classification;
		
		public static DataPoint FromString(string[] raw_data, List<DataColumn> columns)
		{			
			DataPoint dp = new DataPoint();
			
			//need to create a data point and deal with the types
			dp._data = new List<DataValue>();
			dp._rawData = raw_data;
			
			for (int i = 0; i < raw_data.Length; i++)
			{
				var value = raw_data[i];
				
				bool isClassification = false;
				if (i == raw_data.Length - 1)
				{
					isClassification = true;
				}
				
				var dv = new DataValue();
				dv._rawValue = value;
				
				switch (columns[i]._type)
				{
				//TODO abstract this code into multiple classes						
					case DataColumn.DataValueTypes.NUMBER:					
						
						if (!double.TryParse(value, out dv._value))
						{
							dv._isMissing = true;
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
						dv._value = columns[i]._codebook.GetOrAddValue(value);
						
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
