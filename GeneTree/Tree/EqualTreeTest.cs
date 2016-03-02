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
	public class EqualTreeTest : TreeTest
	{
		//TODO create a test for doing group subset or a multi group test, ensure that not all groups end up in it, N-1 max
		public double _valTest;
		public int _param;

		#region implemented abstract members of TreeTest
		public override TreeTest Copy()
		{
			var test_copy = new EqualTreeTest();
			test_copy._param = this._param;
			test_copy._valTest = this._valTest;
			test_copy._testCol = this._testCol;
			return test_copy;
		}

		public override bool isTrueTest(DataPoint point)
		{
			return point._data[_param]._value == _valTest;
		}
		
		public override bool IsMissingTest(DataPoint point)
		{
			return point._data[_param]._isMissing;
		}

		#endregion
		public override string ToString()
		{
			return string.Format("{0} == {1}", _param, _valTest);
		}
	}
}


