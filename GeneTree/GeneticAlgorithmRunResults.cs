using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GeneTree
{
	public class GeneticAlgorithmRunResults
	{
		public ConfusionMatrix _matrix;

		public int count_allData;

		public int count_classedData;

		public int tree_nodeCount;
		
		public GeneticAlgorithmRunResults(DataPointManager mgr)
		{
			_matrix = new ConfusionMatrix(mgr.classes.Length);
		}
		
		public double GetPercentClassified
		{
			get
			{
				return 1.0 * count_classedData / count_allData;
			}
		}

		public double GetMetricResult
		{
			get
			{
				if (this.GetPercentClassified < 0.1)
				{
					return 0;
				}				
				
				return _matrix.GetKappa() *
				Math.Pow(this.GetPercentClassified, 0.1) *
				Math.Pow(1.0 * _matrix._columnsWithData / _matrix._size, 0.1);
			}
		}
		
		public override string ToString()
		{
			return string.Format("[GeneticAlgorithmRunResults Score={0}, Kappa={1}, Matrix={2}, Count_allData={3}, Count_classedData={4}]",
				GetMetricResult, _matrix.GetKappa(), _matrix, count_allData, count_classedData);
		}


	}
}

