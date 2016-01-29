using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GeneTree
{
	public interface IDataValue
	{
		bool IsLessThanOrEqualTo(object obj);
	}
	
	public abstract class DataValue : IDataValue
	{
		public abstract bool IsLessThanOrEqualTo(object obj);
	}
	
	public class DataValue<T> : DataValue
	{
		public bool _isMissing;
		public T _value;
		public string _rawValue;
		
		#region implemented abstract members of DataValue
		public override bool IsLessThanOrEqualTo(object obj)
		{
			return string.Compare(_value.ToString(), obj.ToString()) <= 0;
		}
		#endregion
	}
	
	public enum DataValueTypes
	{
		DOUBLE,
		STRING
	}
}


