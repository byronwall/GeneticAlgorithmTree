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
	public class GeneticAlgorithmManager
	{
		public GeneticAlgorithmOptions _gaOptions = new GeneticAlgorithmOptions();
		
		//TODO allow this to take a seed for reproducibiltiy later
		public Random rando = new Random();
		public DataPointManager dataPointMgr = new DataPointManager();
		
		public event EventHandler<EventArg<GeneticAlgorithmUpdateStatus>> ProgressUpdated;
		
		public void OnProgressUpdated(int progress)
		{
			var update = new GeneticAlgorithmUpdateStatus();
			update.progress = progress;
			update.status = Logger.GetStringAndFlush();
			
			var handler = ProgressUpdated;
			if (handler != null)
			{
				handler(this, new EventArg<GeneticAlgorithmUpdateStatus>(update));
			}
		}
		
		private List<Tree> CreateRandomPoolOfTrees(int size)
		{
			var trees = new List<Tree>();
			for (int i = 0; i < size; i++)
			{
				trees.Add(CreateRandomTree());
			}
			return trees;
		}

		private List<Tree> ProcessPoolOfTrees(IEnumerable<Tree> trees, int generation)
		{
			List<Tree> keeps = new List<Tree>();
			
			Logger.WriteLine("point count: " + dataPointMgr._pointsToTest.Count);
			
			foreach (var tree in trees)
			{
				GeneticAlgorithmRunResults results = new GeneticAlgorithmRunResults(dataPointMgr);
				tree.ProcessDataThroughTree(dataPointMgr, results);
				
				//TODO improve override for non improving score
				if (!tree._isDirty ||
				    tree._prevResults == null ||
				    results.GetMetricResult > tree._prevResults.GetMetricResult ||
				    rando.NextDouble() > 0.95)
				{
					//only add the new tree to the results if the score improved
					tree.RemoveZeroCountNodes();
					keeps.Add(tree);
				}
				
				tree._isDirty = false;
				tree._prevResults = results;
			}
			
			return keeps;
		}
		
		public void CreatePoolOfGoodTrees()
		{
			List<Tree> theBest = new List<Tree>();
			
			var new_dir = Directory.CreateDirectory("tree outputs\\" + DateTime.Now.Ticks);
			
			//HACK: data update only at start to ensure that trees are improving on same data
			dataPointMgr.UpdateSubsetOfDatapoints(_gaOptions.prob_to_keep_data, rando);
			
			for (int j = 0; j < _gaOptions.seq_outer_run; j++)
			{
				List<Tree> keepers = new List<Tree>();
				
				_gaOptions.populationSize = _gaOptions.seq_inner_population;
				_gaOptions.generations = _gaOptions.seq_inner_generations;
				
				for (int run = 0; run < _gaOptions.seq_inner_run; run++)
				{
					var trees = ProcessTheNextGeneration();					
					trees[0].WriteToXmlFile(Path.Combine(new_dir.FullName, string.Format("{0} - {1}.xml", j, run)));					
					keepers.AddRange(trees);
				}
				
				_gaOptions.populationSize *= _gaOptions.seq_inner_run;
				_gaOptions.generations = _gaOptions.seq_middle_generations;
				
				theBest.AddRange(ProcessTheNextGeneration(keepers));
			}
			
			_gaOptions.populationSize *= _gaOptions.seq_outer_run;
			_gaOptions.generations = _gaOptions.seq_outer_generations;
			
			ProcessTheNextGeneration(theBest);
		}
		
		public List<Tree> ProcessTheNextGeneration()
		{
			var starter = CreateRandomPoolOfTrees(_gaOptions.populationSize);
			
			return ProcessTheNextGeneration(starter);
		}

		public List<Tree> ProcessTheNextGeneration(List<Tree> starter)
		{
			//TODO move the processing code into a GeneticOperations class to handle it all
			
			//TODO add a step to check for "convergence" and stop iterating
			for (int generationNumber = 0; generationNumber < _gaOptions.generations; generationNumber++)
			{
				Logger.WriteLine("generation: " + generationNumber);
				
				starter = ProcessPoolOfTrees(starter, generationNumber);
				
				List<Tree> newCreations = new List<Tree>();
								
				//thin down the herd and take pop size or total
				starter = starter.Distinct()
					.OrderByDescending(c => c._prevResults.GetMetricResult)
					.Take((int)(_gaOptions.populationSize * _gaOptions.prob_population_to_keep))
					.ToList();
				
				//output some info on best
				//Logger.WriteLine(string.Join("\r\n", starter.Take(10).Select(c => c._prevResults.ToString())));
				
				foreach (var tree in starter.Take(1))
				{
					Logger.WriteLine("");
					Logger.WriteLine(tree);
					Logger.WriteLine(tree._currentResults);
				}
				
				for (int populationNumber = 0; populationNumber < _gaOptions.populationSize; populationNumber++)
				{
					double tester = rando.NextDouble();
					//TODO: all of these stubs need to be turned into a new class that abstracts away the behavior
					
					if (tester < _gaOptions.prob_ops_swap)
					{
						//node swap
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree2 = starter[rando.Next(starter.Count())];
						
						Tree tree1_copy = tree1.Copy();
						Tree tree2_copy = tree2.Copy();
						
						tree1_copy._source = "swap";
						tree2_copy._source = "swap";
						
						TreeNode node1 = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						TreeNode node2 = tree2_copy._nodes[rando.Next(tree2_copy._nodes.Count)];
						
						TreeNode.SwapNodesInTrees(node1, node2);
						
						//stick both trees into the next gen
						if (tree1_copy._nodes.Count > 0)
						{
							newCreations.Add(tree1_copy);
						}
						if (tree2_copy._nodes.Count > 0)
						{
							newCreations.Add(tree2_copy);
						}
					}
					else if (tester < _gaOptions.prob_ops_delete)
					{
						//node deletion
						
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree1_copy = tree1.Copy();
						TreeNode node1 = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						
						TreeNode node1_rando_term = CreateRandomNode(tree1_copy, true);
						
						//stick the new node into the old one's spot
						node1_rando_term._parent = node1._parent;
						
						tree1_copy.RemoveNodeWithChildren(node1);
						
						if (node1_rando_term._parent != null && tree1_copy._nodes.Count > 0)
						{
							node1_rando_term._parent.UpdateChildReference(node1, node1_rando_term);
							tree1_copy._source = "delete";
							newCreations.Add(tree1_copy);
						}
					}
					else if (tester < _gaOptions.prob_ops_change)
					{
						//node parameter/value change
						
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree1_copy = tree1.Copy();
						
						TreeNode node1_copy = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						node1_copy.ApplyRandomChangeToNodeValue(this);
						
						newCreations.Add(tree1_copy);
					}
					else
					{
						//all random new
						Tree tree = CreateRandomTree();
						tree._source = "new tree";
						
						newCreations.Add(tree);
					}
				}
				
				//moves things to the master list
				starter.AddRange(newCreations);
				
				//output some data about the keepers
				foreach (var element in newCreations.GroupBy(c=>c._source))
				{
					Logger.WriteLine(string.Format("{0} has {1} = {2}", element.Key, element.Count(), 1.0 * element.Count() / _gaOptions.populationSize));
				}
				
				OnProgressUpdated(100 * generationNumber / _gaOptions.generations);
			}
			//TODO add a step at the end to verify the results with a hold out data set
			OnProgressUpdated(100);
			
			return starter;
		}

		public void LoadDataFile(string csv_path, string config_path)
		{
			dataPointMgr.SetConfiguration(config_path);
			dataPointMgr.LoadFromCsv(csv_path);
		}
		
		public TreeNode CreateRandomNode(Tree tree, bool ShouldForceTerminal = false)
		{
			//TODO remove this completely
			return TreeNode.TreeNodeFactory(this, ShouldForceTerminal, tree);
		}

		private Tree CreateRandomTree()
		{
			//build a random tree
			Tree tree = new Tree();
			
			TreeNode root = CreateRandomNode(tree);
			tree.AddRootToTree(root);
			
			//run a queue to create children for non-terminal nodes
			Queue<TreeNode> nonTermNodes = new Queue<TreeNode>();
			nonTermNodes.Enqueue(root);
			while (nonTermNodes.Count > 0)
			{
				var node = nonTermNodes.Dequeue();
				node.FillNodeWithRandomChildrenIfNeeded(this);
				
				foreach (var subNode in node._subNodes)
				{
					nonTermNodes.Enqueue(subNode);
				}
			}
			return tree;
		}
	}
}