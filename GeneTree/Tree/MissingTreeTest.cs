using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace GeneTree
{
	[Serializable]
	public class MissingTreeTest : TreeTest
	{
		public int _param;

		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			var test_copy = new MissingTreeTest();
			test_copy._param = this._param;
			return test_copy;
		}

		public override bool isTrueTest(DataPoint point)
		{
			return point._data[_param]._isMissing;
		}

		#endregion
		public override string ToString()
		{
			return string.Format("{0} missing", _param);
		}
	}
}


