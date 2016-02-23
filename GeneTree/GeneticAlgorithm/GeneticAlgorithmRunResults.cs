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
		
		public GeneticAlgorithmManager ga_mgr;
		
		public GeneticAlgorithmRunResults(GeneticAlgorithmManager ga_mgr)
		{
			this.ga_mgr = ga_mgr;
			_matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
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
				if (this.GetPercentClassified < ga_mgr._gaOptions.eval_percentClass_min)
				{
					return 0;
				}				
				
				return _matrix.GetKappa() *
				Math.Pow(this.GetPercentClassified, ga_mgr._gaOptions.eval_class_power) *
				Math.Pow(1.0 * _matrix._columnsWithData / _matrix._size, ga_mgr._gaOptions.eval_coverage_power) *
				Math.Pow(Math.Log(tree_nodeCount), -0.2);
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(string.Format("[Score={0}]", GetMetricResult));
			sb.AppendLine(string.Format("[Kappa={0}]", _matrix.GetKappa()));
			sb.AppendLine(string.Format("[Matrix={0}]", _matrix));
			sb.AppendLine(string.Format("[Count_allData={0}]", count_allData));
			sb.AppendLine(string.Format("[Count_classedData={0}]", count_classedData));
			sb.AppendLine(string.Format("[NodeCount={0}]", tree_nodeCount));
			
			return sb.ToString();
		}


	}
}

