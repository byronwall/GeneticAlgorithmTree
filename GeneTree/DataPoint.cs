using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public class DataPoint
	{
		//TODO DataPoint needs to be upgraded to handle various data types instead of double

		public IEnumerable<string> _rawData;
		public List<DataValue> _data;
		public DataValue<string> _classification;
		
		public static DataPoint FromString(IEnumerable<string> raw_data, IEnumerable<DataValueTypes> types)
		{			
			DataPoint dp = new DataPoint();
			
			//need to create a data point and deal with the types
			dp._data = new List<DataValue>();
			dp._rawData = raw_data;
			
			for (int i = 0; i < raw_data.Count(); i++)
			{
				var value = raw_data.ElementAt(i);
				
				bool isClassification = false;
				if (i == raw_data.Count() - 1)
				{
					isClassification = true;
				}
				//TODO need to be able to deal with non-double data, load data into raw_data as string and then process, issue #7
				
				switch (types.ElementAt(i))
				{
					case DataValueTypes.DOUBLE:
						var dv = new DataValue<double>();
						dv._rawValue = value;
						
						if (!double.TryParse(value, out dv._value))
						{
							dv._isMissing = true;
						}						
						
						if (isClassification)
						{
							throw new Exception("cannot have a non-string for teh classification");
						}
						dp._data.Add(dv);
						
						break;
					case DataValueTypes.STRING:
						var dv_str = new DataValue<string>();
						dv_str._rawValue = value;					
						dv_str._value = value;										
						
						if (!isClassification)
						{
							dp._data.Add(dv_str);
						}
						else
						{
							dp._classification = dv_str;
						}
						
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			
			return dp;
		}
	}
    
	
	
	
}
