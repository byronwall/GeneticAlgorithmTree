using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeneTree
{
	class DataPointConfiguration
	{
		public Dictionary<string, DataColumn.DataValueTypes> _types = new Dictionary<string, DataColumn.DataValueTypes>();

		public static DataPointConfiguration CreateDefaultFromHeaders(string headers)
		{
			var config = new DataPointConfiguration();
			var parts = headers.Split(',');
			foreach (var header in parts)
			{
				config._types.Add(header, DataColumn.DataValueTypes.CATEGORY);
			}
			return config;
		}
		
		public static DataPointConfiguration CreateDefaultFromFile(string path){
			using (StreamReader sr = new StreamReader(File.OpenRead(path)))
			{
				string headers = sr.ReadLine();
				
				return DataPointConfiguration.CreateDefaultFromHeaders(headers);
			}
		}
		
		public static DataPointConfiguration LoadFromFile(string path)
		{
			DataPointConfiguration config = new DataPointConfiguration();
			
			using (StreamReader sr = new StreamReader(File.OpenRead(path)))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();
					
					var parts = line.Split('|');
					
					config._types.Add(parts[0], (DataColumn.DataValueTypes)Enum.Parse(typeof(DataColumn.DataValueTypes), parts[1]));
				}
			}
			
			return config;
		}
		
		public void SaveToFile(string path)
		{
			using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.ReadWrite)))
			{
				foreach (var item in _types)
				{
					sw.WriteLine("{0}|{1}", item.Key, item.Value);
				}
			}					
		}
	}
}


