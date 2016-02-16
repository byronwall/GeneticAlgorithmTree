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
	
	public class GeneticAlgorithmUpdate
	{
		public int progress;
		public string status;
	}
	
	public class GeneticAlgorithmOptions
	{
		//TODO fill this class out with options and then create a PropertyGrid to change options
	}
	
	public class GeneticAlgorithmRunResults{
		//TODO fill this class out with the results from an evaluation, should allow for a better metric evaluation
	}
	
	public class GeneticAlgorithmManager
	{
		Random rando = new Random();
		DataPointManager dataPointMgr = new DataPointManager();
		
		public event EventHandler<EventArg<GeneticAlgorithmUpdate>> ProgressUpdated;
		
		public void OnProgressUpdated(int progress)
		{
			var update = new GeneticAlgorithmUpdate();
			update.progress = progress;
			update.status = Logger.GetStringAndFlush();
			
			var handler = ProgressUpdated;
			if (handler != null)
			{
				handler(this, new EventArg<GeneticAlgorithmUpdate>(update));
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
				ConfusionMatrix cm = tree.ProcessDataThroughTree(dataPointMgr);
				
				//TODO improve this score to consider # classified
				//TODO create a separate class to handle GA metrics
				double score = cm.GetKappa();
				
				//TODO improve override for non improving score
				if (!tree._isDirty || score > tree._prevScore || rando.NextDouble() > 0.95)
				{
					//only add the new tree to the results if the score improved
					keeps.Add(tree);
				}
				
				tree._prevScore = score;
				tree._isDirty = false;
			}
			
			return keeps;
		}
		
		//TODO create a GeneticOptions and move these options over to it
		double prob_node_terminal = 0.5;
		double prob_ops_change = 0.9;
		double prob_ops_delete = 0.4;
		double prob_ops_swap = 0.2;
		double prob_to_keep_data = 1;
		
		int generations = 10;
		int max_node_count_for_new_tree = 40;
		int populationSize = 200;
		
		public void CreatePoolOfGoodTrees()
		{
			int inner_run = 10;
			int outer_run = 1;
			
			List<Tree> theBest = new List<Tree>();
			
			var new_dir = Directory.CreateDirectory("tree outputs\\" + DateTime.Now.Ticks);
			
			//HACK: data update only at start to ensure that trees are improving on same data
			dataPointMgr.UpdateSubsetOfDatapoints(prob_to_keep_data, rando);
			
			for (int j = 0; j < outer_run; j++)
			{
				List<Tree> keepers = new List<Tree>();
				
				populationSize = 1000;
				generations = 30;
				
				for (int run = 0; run < inner_run; run++)
				{
					var trees = ProcessTheNextGeneration();					
					trees[0].WriteToXmlFile(Path.Combine(new_dir.FullName, string.Format("{0} - {1}.xml", j, run)));					
					keepers.AddRange(trees);
				}
				
				populationSize *= inner_run;
				generations = 50;
				
				theBest.AddRange(ProcessTheNextGeneration(keepers));
			}
			
			populationSize *= outer_run;
			generations = 100;
			
			ProcessTheNextGeneration(theBest);
		}
		
		public List<Tree> ProcessTheNextGeneration()
		{
			var starter = CreateRandomPoolOfTrees(populationSize);
			
			return ProcessTheNextGeneration(starter);
		}

		public List<Tree> ProcessTheNextGeneration(List<Tree> starter)
		{
			//TODO move the processing code into a GeneticOperations class to handle it all
			
			//TODO add a step to check for "convergence" and stop iterating
			for (int generationNumber = 0; generationNumber < generations; generationNumber++)
			{
				Logger.WriteLine("generation: " + generationNumber);
				
				starter = ProcessPoolOfTrees(starter, generationNumber);
				
				List<Tree> newCreations = new List<Tree>();
								
				//thin down the herd and take pop size or total
				starter = starter.Distinct().OrderByDescending(c => c._prevScore).Take(populationSize).ToList();
				
				//output some info on best
				Logger.WriteLine(string.Join("\r\n", starter.Take(10).Select(c => c._prevScore.ToString())));
				
				foreach (var tree in starter.Take(1))
				{
					Logger.WriteLine("");
					Logger.WriteLine(tree);
				}
				
				for (int populationNumber = 0; populationNumber < populationSize; populationNumber++)
				{
					double tester = rando.NextDouble();
					//TODO: all of these stubs need to be turned into a new class that abstracts away the behavior
					
					if (tester < prob_ops_swap)
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
					else if (tester < prob_ops_delete)
					{
						//node deletion
						
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree1_copy = tree1.Copy();
						TreeNode node1 = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						
						TreeNode node1_rando_term = CreateRandomNode(tree1_copy, true);
						
						//stick the new node into the old one's spot
						node1_rando_term._parent = node1._parent;
						node1_rando_term._parentTrue = node1._parentTrue;
						
						tree1_copy.RemoveNodeWithChildren(node1);
						
						if (node1_rando_term._parent != null && tree1_copy._nodes.Count > 0)
						{
							if (node1_rando_term._parentTrue)
							{
								node1_rando_term._parent._trueNode = node1_rando_term;
							}
							else
							{
								node1_rando_term._parent._falseNode = node1_rando_term;
							}
							
							tree1_copy._source = "delete";
							newCreations.Add(tree1_copy);
						}
					}
					else if (tester < prob_ops_change)
					{
						//node parameter/value change
						
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree1_copy = tree1.Copy();
						
						TreeNode node1_copy = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						
						//TODO allow for the value to be modifying (add/subtract/etc. to make smaller changes)
						if (node1_copy.IsTerminal)
						{
							node1_copy.Classification = dataPointMgr.GetRandomClassification(rando);
							tree1_copy._source = "new class";
						}
						else
						{
							node1_copy.Test = TreeTest.TreeTestFactory(dataPointMgr, rando);
							tree1_copy._source = "new test";
							
							//TODO improve this part to allow for value changing again, doing a quick fix to make this work
						}
						
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
					Logger.WriteLine(string.Format("{0} has {1} = {2}", element.Key, element.Count(), 1.0 * element.Count() / populationSize));
				}
				
				OnProgressUpdated(100 * generationNumber / generations);
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
		
		public TreeTest CreateRandomTest()
		{
			//TODO remove this method completely
			return TreeTest.TreeTestFactory(dataPointMgr, rando);
		}
		
		public TreeNode CreateRandomNode(Tree tree, bool ShouldForceTerminal = false)
		{
			TreeNode node = new TreeNode();
			
			tree.AddNodeWithoutChildren(node);
			
			node.IsTerminal = rando.NextDouble() > prob_node_terminal;
			
			//TODO: consider changing this or using some other scheme to prevent runaway initial trees.					
			if (ShouldForceTerminal || tree._nodes.Count > max_node_count_for_new_tree)
			{
				node.IsTerminal = true;
			}
			if (node.IsTerminal)
			{
				node.Classification = dataPointMgr.GetRandomClassification(rando);
			}
			else
			{
				//create the test				
				node.Test = CreateRandomTest();
			}			
				
			return node;
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
				
				//need to create two new nodes, yes and no
				
				TreeNode node_true = CreateRandomNode(tree);
				TreeNode node_false = CreateRandomNode(tree);
				
				node._trueNode = node_true;
				node._falseNode = node_false;
				
				node_true._parent = node;
				node_false._parent = node;
				
				node_true._parentTrue = true;
				
				//process the new nodes if they are not terminal
				if (!node_true.IsTerminal)
				{
					nonTermNodes.Enqueue(node_true);
				}
				if (!node_false.IsTerminal)
				{
					nonTermNodes.Enqueue(node_false);
				}
			}			
			return tree;
		}
	}
}