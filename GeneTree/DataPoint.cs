using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneTree
{
    public class DataPoint
    {
		//TODO DataPoint needs to be upgraded to handle various data types instead of double

		public IEnumerable<string> raw_data;
        public double?[] Data;
        public string Classification;
        
        public void LoadDataFromRaw(IEnumerable<string> data){
			this.raw_data = data;			
        }
        
        private void ProcessRaw(){
			//for now, just loop through and put a number in place if possible
        	
			Data = new double?[raw_data.Count];
			
			int i =0;
			double temp;
			foreach (var datum in raw_data) {
				if(double.TryParse(datum, ref Data[i])){
					Data[i] = null;
				}
			}
			
        	return;
        }
    }
}
