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
		
		public List<DataPoint> _dataPoints = new List<DataPoint>();
		public List<DataColumn> _columns = new List<DataColumn>();
		public DataPointConfiguration _config;
		
		public List<int> classCounts = new List<int>();
		
		public int DataColumnCount
		{
			get
			{
				return _columns.Count - 1;
			}
		}
		
		public IEnumerable<DataPoint> GetSubsetOfDatapoints(double fractionToKeep, Random rando)
		{
			//quick trap to force fraction
			fractionToKeep = Math.Min(Math.Max(fractionToKeep, 0), 1);
			
			foreach (var dataPoint in _dataPoints)
			{
				if (rando.NextDouble() < fractionToKeep)
				{
					yield return dataPoint;
				}
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
			classes = _dataPoints.GroupBy(x => x._classification._value).Select(x => x.Key).ToArray();
			
			foreach (var _class in _dataPoints.GroupBy(x => x._classification._value)) {
				
			} 
		}
        
		public void LoadFromCsv(string path)
		{
			//TODO this method should split the data into test/train in order to better evaluate the tree
			var reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			
			var header_line = reader.ReadLine();
			
			//TODO some step to ensure headers are in same order as data, create mapping here possibly
			
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
				_dataPoints.Add(dp);
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
