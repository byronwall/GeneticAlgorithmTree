using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace GeneTree
{
	public class GeneticAlgorithmManager
	{
		Random rando = new Random();
		DataPointManager dataPointMgr = new DataPointManager();
		
		public event EventHandler<EventArg<int>> ProgressUpdated;
		
		public void OnProgressUpdated(int progress)
		{
			var handler = ProgressUpdated;
			if (handler != null)
			{
				handler(this, new EventArg<int>(progress));
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
			
			double prob_to_keep_data = 0.02;
			
			dataPointMgr.UpdateSubsetOfDatapoints(prob_to_keep_data, rando);
			
			Trace.WriteLine("point count: " + dataPointMgr._pointsToTest.Count);
			
			foreach (var tree in trees)
			{				
				ConfusionMatrix cm = tree.ProcessDataThroughTree(dataPointMgr);				
				double score = cm.GetKappa();
				
				//TODO improve override for non improving score
				if (score > tree._prevScore || rando.NextDouble() > 0.98)
				{				
					//only add the new tree to the results if the score improved
					keeps.Add(tree);
				}
				
				tree._prevScore = score;
			}
			
			return keeps;
		}

		//TODO create a GeneticOptions and move these options over to it
		int populationSize = 200;
		int generations = 10;
		int max_node_count_for_new_tree = 40;
		
		public void CreatePoolOfGoodTrees()
		{
			int times_to_run = 10;
			
			List<Tree> theBest = new List<Tree>();
			
			for (int j = 0; j < times_to_run; j++)
			{	
				List<Tree> keepers = new List<Tree>();
				populationSize = 1000;
				
				for (int run = 0; run < times_to_run; run++)
				{
					keepers.AddRange(ProcessTheNextGeneration());
				}
			
				populationSize *= 10;
				generations = 25;
			
				theBest.AddRange(ProcessTheNextGeneration(keepers));
			}
			
			populationSize *= 10;
			generations = 25;
			
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
			starter = ProcessPoolOfTrees(starter, 1);
			
			//TODO add a step to check for "convergence" and stop iterating
			for (int generationNumber = 0; generationNumber < generations; generationNumber++)
			{
				List<Tree> newCreations = new List<Tree>();
				
				//remove duplicate trees if any
				starter = starter.Distinct().ToList();
				
				//thin down the herd and take pop size or total
				starter = starter.OrderByDescending(c => c._prevScore).Take(Math.Min(populationSize, starter.Count)).ToList();
				
				//output some info on best
				Trace.WriteLine(string.Join("\r\n", starter.Take(10).Select(c => c._prevScore.ToString())));
				
				foreach (var tree in starter.Take(1))
				{
					Trace.WriteLine("");
					Trace.WriteLine(tree);
				}
				
				Trace.WriteLine("generation: " + generationNumber);	
				
				for (int populationNumber = 0; populationNumber < populationSize; populationNumber++)
				{
					//make a new one!
					double tester = rando.NextDouble();
					//TODO: all of these stubs need to be turned into a new class that abstracts away the behavior
					if (tester < 0.2)
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
					else if (tester < 0.4)
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
					else if (tester < 0.9)
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
							var test = node1_copy.Test;							
							tree1_copy._source = "new test value";
							
							if (rando.NextDouble() < 0.5)
							{
								//change param test
								test.param = rando.Next(dataPointMgr.DataColumnCount);
								tree1_copy._source = "new param";
							}
							
							test.valTest = dataPointMgr._columns[test.param].GetTestValue(rando);
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
				//thin down the new creations
				newCreations = ProcessPoolOfTrees(newCreations, generationNumber);
				
				//output some data about the keepers
				foreach (var element in newCreations.GroupBy(c=>c._source))
				{
					Trace.WriteLine(string.Format("{0} has {1} = {2}", element.Key, element.Count(), 1.0 * element.Count() / populationSize));
				} 
				
				//add the good ones back to the main population
				starter.AddRange(newCreations);
				
				OnProgressUpdated(100 * generationNumber / generations);
			}
			//TODO add a step at the end to verify the results with a hold out data set
			//TODO add the ability to use the best tree for prediction and generate a results file w/ the predictions
			OnProgressUpdated(100);
			
			return starter;
		}

		public void LoadDataFile(string csv_path, string config_path)
		{
			dataPointMgr.SetConfiguration(config_path);
			dataPointMgr.LoadFromCsv(csv_path);
		}
		public void LoadDataFile(string path)
		{
			dataPointMgr.LoadFromCsv(path);
		}

		public TreeTest CreateRandomTest()
		{
			TreeTest testYes = new TreeTest();
			testYes.param = rando.Next(dataPointMgr.DataColumnCount);
			DataColumn column = dataPointMgr._columns[testYes.param];
			testYes.valTest = column.GetTestValue(rando);
			return testYes;
		}
		public TreeNode CreateRandomNode(Tree tree, bool ShouldForceTerminal = false)
		{
			TreeNode node = new TreeNode();
			
			tree.AddNodeWithoutChildren(node);
			
			//TODO take this and other probabilities and move them somewhere central
			node.IsTerminal = rando.NextDouble() > 0.5;
			
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