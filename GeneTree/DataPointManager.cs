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
		public DataPointConfiguration _config;
		
		public int DataColumnCount
		{
			get
			{
				return _columns.Count - 1;
			}
		}
		
		/// <summary>
		/// Loads the configuration file and sets up the columns based on it.
		/// </summary>
		/// <param name="config_path"></param>
		public void SetConfiguration(string config_path)
		{			
			_config = DataPointConfiguration.LoadFromFile(config_path);

			foreach (var column in _config._types)
			{
				//TODO get rid of this switch... push it into the data column as a static factory or such
				DataColumn dc;
				switch (column.Value)
				{						
					case DataColumn.DataValueTypes.NUMBER:
						dc = new DoubleDataColumn();
						break;
					case DataColumn.DataValueTypes.CATEGORY:
						dc = new CategoryDataColumn();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}				
				
				dc._header = column.Key;
				
				_columns.Add(dc);
			}			
			
		}

		public void DetermineClasses()
		{
			//TODO get rid of this method.  it does not seem necessary
			classes = dataPoints.GroupBy(x => x._classification._value).Select(x => x.Key).ToArray();
		}
        
		public void SetHeaders(string str_headers)
		{
			//TODO come up with a cleaner architecture than this trap (force default from outside)
			if (_columns.Count > 0)
			{
				return;
			}
			
			var headers_from_csv = str_headers.Split(',');
			
			foreach (var header in headers_from_csv)
			{
				DataColumn col;		
				//TODO pull this info from a config file or the GUI, maybe a first step to load headers and confirm data type						
				//HACK: forces all to be category this way
				//HACK: figure out a better way to identify the last item
				if (true || header == headers_from_csv.Last())
				{
					col = new CategoryDataColumn();
					col._type = DataColumn.DataValueTypes.CATEGORY;
					col._codebook = new CodeBook();
				}
				else
				{
					col = new DoubleDataColumn();
				}
				
				col._header = header;
				
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
				
				string[] values = line.Split(',');

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
