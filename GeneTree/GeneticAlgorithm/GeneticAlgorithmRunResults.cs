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
		
		public void ProcessScoresAfterTraverse()
		{
			double totalLoss = 0.0;
			foreach (var item in node_assoc)
			{
				//determine the correct class for data point
				double pt_class = item.Item1._classification._value;
				pt_class = Math.Min(Math.Max(1E-15, pt_class), 1 - 1E-15);
				
				//get the probability for the leaf node (score)
				double leaf_score = item.Item2.matrix.PositiveClassProbability;
				leaf_score = Math.Min(Math.Max(1E-15, leaf_score), 1 - 1E-15);
				
				item.Item2.ProbPrediction = item.Item2.matrix.PositiveClassProbability;
				
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

				double node_large_number = 10000;
				double kappa_number = 10;
				
				double class_number = (this.GetPercentClassified < ga_mgr._gaOptions.Eval_percentClass_min) ?
					((this.GetPercentClassified < ga_mgr._gaOptions.Eval_percentClass_min / 2) ? 0 : ga_mgr._gaOptions.Eval_percentClass_min) : 3;
				
				double score = 1.0 / AverageLoss *
				               (class_number / (1 - this.GetPercentClassified + class_number)) *
				               (node_large_number / (tree_nodeCount + node_large_number)) *
				               (kappa_number / (1 - this._matrix.GetKappa() + kappa_number));
				
				//TODO might need a check for NaN, not sure how that affects sorting
				
				return score;
			}
		}
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			
			sb.AppendLine(string.Format("[Score={0}]", GetMetricResult));
			sb.AppendLine(string.Format("[LogLoss={0}]", AverageLoss));
			sb.AppendLine(string.Format("[Kappa={0}]", _matrix.GetKappa()));
			sb.AppendLine(string.Format("[PercClassed={0:0.000}]", GetPercentClassified));
			sb.AppendLine(string.Format("[Matrix={0}]", _matrix));
			sb.AppendLine(string.Format("[Count_allData={0}]", count_allData));
			sb.AppendLine(string.Format("[Count_classedData={0}]", count_classedData));
			sb.AppendLine(string.Format("[NodeCount={0}]", tree_nodeCount));
			
			return sb.ToString();
		}


	}
}

