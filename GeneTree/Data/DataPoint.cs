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
		public string _id;
		
		public static DataPoint FromString(string[] raw_data,
			Dictionary<int, string> header_mapping, 
			Dictionary<int, DataColumn> colMapping, 
			DataPointConfiguration configs)
		{			
			DataPoint dp = new DataPoint();
			
			//need to create a data point and deal with the types
			dp._data = new List<DataValue>();
			
			for (int i = 0; i < raw_data.Length; i++)
			{
				var value = raw_data[i];
				
				var dv = new DataValue();
				
				var header = header_mapping[i];
				DataColumn column = colMapping[i];
				var config = configs._types[header];
				
				//TODO abstract this code into multiple classes	
				switch (config)
				{									
					case DataColumn.DataValueTypes.NUMBER:					
						if (value == string.Empty || !double.TryParse(value, out dv._value))
						{
							dv._isMissing = true;
							column._hasMissingValues = true;
							dv._value = double.NaN;
						}						
						
						dp._data.Add(dv);
						
						break;
					case DataColumn.DataValueTypes.CATEGORY:				
						if (value == string.Empty)
						{
							dv._isMissing = true;
							column._hasMissingValues = true;
							dv._value = -2;
						}
						else
						{
							var dataColumn = column as CategoryDataColumn;
							dv._value = dataColumn._codebook.GetOrAddValue(value);
						}
						
						dp._data.Add(dv);
						
						break;
					case DataColumn.DataValueTypes.ID:
						dp._id = value;
						
						break;
					case DataColumn.DataValueTypes.CLASS:
						
						//TODO fix this with the actual codebook for teh column
						dv._value = double.Parse(value);
						dp._classification = dv;

						break;
					
					default:
						throw new ArgumentOutOfRangeException();
				}
				
				if (column != null)
				{
					column._values.Add(dv);
				}
			}
			
			return dp;
		}
	}
}
