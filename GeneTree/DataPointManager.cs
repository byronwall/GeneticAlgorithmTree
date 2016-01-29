using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	class DataPointManager
	{
		public string[] classes;
		public Dictionary<int, List<double>> ranges = new Dictionary<int, List<double>>();
		public List<DataPoint> dataPoints = new List<DataPoint>();
		public List<string> headers = new List<string>();
		public int paramCount;
        
		public List<DataValueTypes> columnTypes = new List<DataValueTypes>();

		public void DetermineClasses()
		{
			classes = dataPoints.GroupBy(x => x._classification._value).Select(x => x.Key).ToArray();
		}

		public void DetermineRanges()
		{
			int paramCount = dataPoints[0]._data.Count;
			ranges.Clear();

			
			for (int i = 0; i < columnTypes.Count; i++)
			{
				var colType = columnTypes[i];
				switch (colType)
				{
					case DataValueTypes.DOUBLE:
						double max = dataPoints.Max(x => ((DataValue<double>)x._data[i])._value);
						double min = dataPoints.Min(x => ((DataValue<double>)x._data[i])._value);
						ranges.Add(i, new List<double>
							{
								min,
								max
							});
						break;
					case DataValueTypes.STRING:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
        
		public void SetHeaders(string str_headers)
		{
			//TODO fully process the header row, issue #6
			
			var headers_from_csv = str_headers.Split(',');
			headers.AddRange(headers_from_csv);
			
			//know how many headers, create the DataTypes			
			for (int i = 0; i < str_headers.Count(); i++)
			{
				columnTypes.Add(DataValueTypes.STRING);
			}
			
			//at this point, the headers are set up and data is ready to be processed... send it back to the loader
		}
        
		public void LoadFromCsv(string path)
		{
			//TODO this method should split the data into test/train in order to better evaluate the tree
			var reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			
			var header_line = reader.ReadLine();
			SetHeaders(header_line);
			
			//parse the CSV data and create data points
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();

				if (line == string.Empty)
				{
					//skip empty lines until the file is done... mainly for last line of file
					continue;
				}
				
				var values = line.Split(',');

				//create data point from the string line
				DataPoint dp = DataPoint.FromString(values, columnTypes);								
				dataPoints.Add(dp);
			}

			//create classes and ranges
			DetermineClasses();

			//get min/max ranges for the data
			DetermineRanges();
		}
        
        
	}
}
