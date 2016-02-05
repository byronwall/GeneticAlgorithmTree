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
		public double[] classes;
		
		public List<DataPoint> dataPoints = new List<DataPoint>();
		public List<DataColumn> _columns = new List<DataColumn>();
		public DataColumn _classifications;
		
		public int DataColumnCount
		{
			get
			{
				return _columns.Count - 1;
			}
		}

		public void DetermineClasses()
		{
			//TODO get rid of this method.  it does not seem necessary
			classes = dataPoints.GroupBy(x => x._classification._value).Select(x => x.Key).ToArray();
		}
        
		public void SetHeaders(string str_headers)
		{
			//TODO fully process the header row, issue #6
			
			var headers_from_csv = str_headers.Split(',');
			
			foreach (var header in headers_from_csv)
			{
				//TODO pull this info from a config file or the GUI, maybe a first step to load headers and confirm data type
				DataColumn col = new DataColumn();
				col._header = header;
				col._type = DataColumn.DataValueTypes.CATEGORY;		
				col._codebook = new CodeBook();
				
				//HACK: figure out a better way to identify the last item
				if (header == headers_from_csv.Last())
				{
					col._type = DataColumn.DataValueTypes.CATEGORY;
					col._codebook = new CodeBook();
					
					_classifications = col;
				}				
				
				_columns.Add(col);				
			}
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
				DataPoint dp = DataPoint.FromString(values, _columns);								
				dataPoints.Add(dp);
			}

			foreach (var column in _columns)
			{
				column.ProcessRanges();
			}
			
			//create classes and ranges
			DetermineClasses();
		}        
	}
}
