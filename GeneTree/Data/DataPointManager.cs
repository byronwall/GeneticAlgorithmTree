using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
	public class DataPointManager
	{
		public double[] classes;
		
		public List<DataPoint> _dataPoints = new List<DataPoint>();
		
		public List<DataColumn> _columns = new List<DataColumn>();
		
		public DataPointConfiguration _config;
		
		public bool _hasClasses = false;
		
		public List<int> classCounts = new List<int>();
		
		public double GetRandomClassification(Random rando)
		{
			double prob_of_no_class = 0.05;
			
			if (rando.NextDouble() < prob_of_no_class)
			{
				return -1;
			}
			
			return classes[rando.Next(classes.Length)];
		}

		public List<DataPoint> _pointsToTest = new List<DataPoint>();
		
		public void UpdateSubsetOfDatapoints()
		{
			UpdateSubsetOfDatapoints(1.0, null);
		}
		
		public void UpdateSubsetOfDatapoints(double fractionToKeep, Random rando)
		{
			//quick trap to force fraction
			fractionToKeep = Math.Min(Math.Max(fractionToKeep, 0), 1);
			
			_pointsToTest.Clear();
			
			foreach (var dataPoint in _dataPoints)
			{
				if (fractionToKeep == 1.0 || rando.NextDouble() < fractionToKeep)
				{
					_pointsToTest.Add(dataPoint);
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
			
			_columnMapping = new Dictionary<string, DataColumn>();
			
			foreach (var column in _config._types)
			{
				//TODO get rid of this switch... push it into the data column as a static factory or such
				DataColumn dc = null;
				switch (column.Value)
				{						
					case DataColumn.DataValueTypes.NUMBER:
						dc = new DoubleDataColumn();
						break;
					case DataColumn.DataValueTypes.CATEGORY:
						dc = new CategoryDataColumn();
						break;
					case DataColumn.DataValueTypes.ID:
						break;
					case DataColumn.DataValueTypes.CLASS:
						_hasClasses = true;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}				
				
				if (dc != null)
				{
					dc._header = column.Key;
					_columns.Add(dc);
					_columnMapping.Add(dc._header, dc);
				}
			}
			
			//at this point, the columns exists, go ahead and load the codebook values
			//TODO really need to generalize this file name and take an input
			
			using (StreamReader sr = new StreamReader("codebook.txt"))
			{
				while (!sr.EndOfStream)
				{
					//TODO move all of this code to the Codebook where it belongs
					var line = sr.ReadLine();
					var parts = line.Split('|');
					
					string header = parts[0];
					string data = parts[1];
					
					var col = _columnMapping[header] as CategoryDataColumn;
					
					col._codebook.PopulateFromString(data);
				}
			}
		}
		
		Dictionary<string, DataColumn> _columnMapping = new Dictionary<string, DataColumn>();

		public void DetermineClasses()
		{
			//TODO fix this check here.  currently a trap to allow for prediction
			if (_hasClasses)
			{
				classes = _dataPoints.GroupBy(x => x._classification._value).Select(x => x.Key).ToArray();
			}
			else
			{
				//TODO fix this hacky part.  assumes that if there are no classes, that we just need 1 row.  makes the ConfusionMatrix work later on
				//TODO currently hardcoded ot binary classifier, needs to know how many classes are possible (or adjust methods later)
				classes = new double[2];
			}
		}
		
		public void OutputCodebooks()
		{
			using (StreamWriter sw = new StreamWriter("codebook.txt"))
			{
				//output header, delim and codebook order
				foreach (var column in _columns)
				{
					CategoryDataColumn col = column as CategoryDataColumn;
					if (col != null)
					{
						sw.WriteLine("{0}|{1}", col._header, col._codebook);
					}
				}
			}
		}
        
		public void LoadFromCsv(string path)
		{
			//TODO this method should split the data into test/train in order to better evaluate the tree
			var reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			
			var header_line = reader.ReadLine();
			string[] headers = header_line.Split(',');
			
			var header_mapping = new 	Dictionary<int, string>();
			
			for (int i = 0; i < headers.Length; i++)
			{
				header_mapping.Add(i, headers[i]);
			}
			
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
				DataPoint dp = DataPoint.FromString(values, header_mapping, _columnMapping, _config);								
				_dataPoints.Add(dp);
			}
			
			foreach (var column in _columns)
			{
				column.ProcessRanges();
			}
			
			//create classes and ranges
			DetermineClasses();
		}
		
		public string GetSummaryOfClasses()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine("all columns summarized:");
			
			foreach (var column in _columns)
			{
				sb.AppendLine(column.GetSummaryString());
			}
			
			return sb.ToString();
		}
	}
}
