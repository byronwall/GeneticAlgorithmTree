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
		
		public List<Tuple<DataPoint, ClassificationTreeNode>> node_assoc = new List<Tuple<DataPoint, ClassificationTreeNode>>();
		
		
		public GeneticAlgorithmRunResults(GeneticAlgorithmManager ga_mgr)
		{
			this.ga_mgr = ga_mgr;
			_matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
		}
		
		const double EPSILON = 1E-15;
		const double LARGE_EPSILON = 1 - EPSILON;
		
		public void ProcessScoresAfterTraverse()
		{
			double totalLoss = 0.0;
				
			foreach (var item in node_assoc)
			{
				//determine the correct class for data point
				double pt_class = item.Item1._classification._value;
				
				//get the probability for the leaf node (score)
				double node_prob = item.Item2.matrix.PositiveClassProbability;
				
				double leaf_score = Math.Min(Math.Max(EPSILON, node_prob), LARGE_EPSILON);
				
				item.Item2.ProbPrediction = node_prob;
				
				//peform a loss function on those margins (use exp loss for now)
				double loss = pt_class * Math.Log(leaf_score) + (1 - pt_class) * Math.Log(1 - leaf_score);
				
				totalLoss += -loss;
			}
			
			averageLoss = totalLoss / node_assoc.Count;
		}
		
		double averageLoss = double.MinValue;
		public double AverageLoss
		{
			get
			{
				return averageLoss;
			}
			set
			{
				averageLoss = value;
			}
		}
		
		public double PercentClassified
		{
			get
			{
				return 1.0 * count_classedData / count_allData;
			}
		}

		public double MetricResult
		{
			get
			{
				if (this.PercentClassified < ga_mgr._gaOptions.Eval_percentClass_min)
				{
					return double.MinValue;
				}

				double node_large_number = 100000;
				double kappa_number = 12;
				double class_number = 5;
				
				double score = 1.0 / AverageLoss; /* *
				               (class_number / (1 - this.PercentClassified + class_number)) *
				               (node_large_number / (tree_nodeCount + node_large_number)) *
				               (kappa_number / (1 - this._matrix.GetKappa() + kappa_number));*/
				
				return score;
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(string.Format("[Score={0}]", MetricResult));
			sb.AppendLine(string.Format("[LogLoss={0}]", AverageLoss));
			sb.AppendLine(string.Format("[Kappa={0}]", _matrix.GetKappa()));
			sb.AppendLine(string.Format("[PercClassed={0:0.000}]", PercentClassified));
			sb.AppendLine(string.Format("[Matrix={0}]", _matrix));
			sb.AppendLine(string.Format("[Count_allData={0}]", count_allData));
			sb.AppendLine(string.Format("[Count_classedData={0}]", count_classedData));
			sb.AppendLine(string.Format("[NodeCount={0}]", tree_nodeCount));
			
			return sb.ToString();
		}
	}
}
