using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MoreLinq;
namespace GeneTree
{
	public class PredictionManager
	{
		public PredictionManager(GeneticAlgorithmManager geneticAlgorithmManager)
		{
			this.ga_mgr = geneticAlgorithmManager;
			this.data_mgr = ga_mgr.dataPointMgr;
		}

		public void LoadTreeAndGenerateResults()
		{
			//will process all of the data through a single tree
			//need a tree to test
			
			//TODO generalize this file name/folder
			Tree tree = Tree.ReadFromXmlFile(@"C:\projects\gene-tree\GeneTree\bin\Debug\tree outputs\635925414609611681\0 - 0.xml");
			var results = new GeneticAlgorithmRunResults(ga_mgr);
			
			tree.ProcessDataThroughTree(data_mgr, results, data_mgr._dataPoints);
			
			//deal with results
			Logger.WriteLine(results);
		}

		public DataPointManager data_mgr;

		public GeneticAlgorithmManager ga_mgr;

		public void GeneratePredictionsForDataWithAllTrees(string folderPath)
		{
			List<Tree> treesToTest = new List<Tree>();
			
			foreach (var file in Directory.GetFiles(folderPath))
			{
				var tree = Tree.ReadFromXmlFile(file);
				treesToTest.Add(tree);
				Debug.WriteLine(tree);
			}
			//loop through the data points, and then loop through trees
			//will contain the ID and probability
			var probs = new List<Tuple<string, double>>();
			foreach (var dataPoint in data_mgr._dataPoints)
			{
				double pred_value = 0.0;
				var results = new GeneticAlgorithmRunResults(ga_mgr);
				int count = 0;
				foreach (var tree in treesToTest)
				{
					var node = tree._root.TraverseData(dataPoint, results);
					
					ClassificationTreeNode termNode = node as ClassificationTreeNode;
					
					if (termNode == null)
					{
						continue;
					}
					else
					{
						pred_value += termNode.ProbPrediction;
						count++;
					}
				}

				probs.Add(Tuple.Create(dataPoint._id, pred_value / count));
			}
			using (StreamWriter sw = new StreamWriter("submission_" + DateTime.Now.Ticks + ".csv"))
			{
				sw.WriteLine("ID,PredictedProb");
				foreach (var prob in probs)
				{
					sw.WriteLine("{0},{1:0.0000}", prob.Item1, prob.Item2);
				}
			}
		}
	}
}

