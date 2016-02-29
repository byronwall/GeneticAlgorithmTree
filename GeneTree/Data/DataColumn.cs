using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace GeneTree
{
	public abstract class DataColumn
	{
		//TODO this enum should probably become an inherited class
		public enum DataValueTypes
		{
			NUMBER,
			CATEGORY
		}
		
		public DataValueTypes _type;
		public List<DataValue> _values = new List<DataValue>();
		public string _header;
		
		public bool _hasMissingValues;
		
		public virtual double GetTestValue(Random rando)
		{
			int index = rando.Next(_values.Count);
			
			return _values[index]._value;
		}
		
		public virtual void ProcessRanges()
		{
			return;
		}
	}
	public class GeneratedDataColumn : DoubleDataColumn
	{
		//this will work to create linear functions of doubles first
		public double _scaling = 1.0;
		public DataColumn _baseColumn;
		
		public void CreateValues(DataPointManager dp_mgr)
		{
			foreach (var value in _baseColumn._values)
			{
				DataValue dv_new = new DataValue();
				dv_new._value = value._value * _scaling;
				
				this._values.Add(dv_new);
			}
			
			//do a zip with the original DataPoints
			var items = dp_mgr._dataPoints.Zip(this._values, (dp, dv) =>
				{
					dp._data.Add(dv);
					return true;
				});
			
			//TODO remove this sloppy bit that forces execution above
			foreach (var item in items)
			{
			}
			
			this.ProcessRanges();
		}
		
		public static GeneratedDataColumn CreateNewRandom(GeneticAlgorithmManager ga_mgr)
		{
			//do some work to create the new column
			var column = new GeneratedDataColumn();
			
			column._scaling = ga_mgr.rando.NextDouble() * 20 - 10.0;
			
			//forces to be a double data column here
			do
			{
				int index = ga_mgr.rando.Next(ga_mgr.dataPointMgr._columns.Count);
				column._baseColumn = ga_mgr.dataPointMgr._columns[index];
			} while(column._baseColumn._type != DataValueTypes.NUMBER);
			
			column._type = DataValueTypes.NUMBER;
			column._header = string.Format("generated from {0} w/ factor {1}", column._baseColumn._header, column._scaling);
			
			column.CreateValues(ga_mgr.dataPointMgr);
			
			return column;
		}
	}
}


