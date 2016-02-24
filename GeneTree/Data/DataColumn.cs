using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		
		public abstract double GetTestValue(Random rando);
		
		public virtual void ProcessRanges()
		{
			return;
		}
	}
}


