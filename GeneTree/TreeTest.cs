using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
    public class TreeTest
    {
        public int param; //data array index to test
        public double valTest; // value to test, low
        public bool isLessThanEqualTest; //returns true if

        //TODO add the ability to test against another value in the data, will work against the balance scale data

        public bool isTrueTest(DataPoint point)
        {
        	var lessThanTest = point._data[param]._value <= valTest;
            if (isLessThanEqualTest)
            {				
				return lessThanTest;
            }
            else
            {
				return !lessThanTest;
            }
        }

        public override string ToString()
        {
            return string.Format("param {0}, LTE test {1}, val {2}", param, isLessThanEqualTest, valTest);
        }
    }
}
