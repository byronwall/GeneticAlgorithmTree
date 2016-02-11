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
		
		//class will do all of the heavy lifting for the GA
		private List<Tree> CreateRandomPoolOfTrees(int size)
		{
			//create a number of random trees and report results from all of them
			List<Tree> results = new List<Tree>();
			for (int i = 0; i < size; i++)
			{
				results.Add(CreateRandomTree());
			}
			return results;
		}

		private List<Tree> ProcessPoolOfTrees(IEnumerable<Tree> trees)
		{
			//create a number of random trees and report results from all of them
			
			List<Tree> keeps = new List<Tree>();
			
			Trace.WriteLine("doing the eval: ratio");
			
			var dataPoints = dataPointMgr.GetSubsetOfDatapoints(0.05, rando).ToList();
			
			foreach (var tree in trees)
			{					
				int correct = tree.ProcessDataThroughTree(dataPoints);				
				double ratio = 1.0 * correct / dataPoints.Count;
				
				//TODO improve the hueristic for evaluation				
				double score = Math.Pow(ratio, 2);
				
				//TODO improve override for non improving score
				if (score > tree._prevScore || rando.NextDouble() > 0.9)
				{				
					//only add the new tree to the results if the score improved
					keeps.Add(tree);
				}
				
				tree._prevScore = score;
			}
			
			//TODO improve the selection here to not just take the top half, maybe iterate them all and select based on the score
			return keeps;
		}

		//TODO create a GeneticOptions and move these options over to it
		int populationSize = 2000;
		int generations = 100;
		int max_node_count_for_new_tree = 12;
			
		public void ProcessTheNextGeneration()
		{
			//TODO move the processing code into a GeneticOperations class to handle it all		
			
			//start with a list of trees and trim it down
			var starter = CreateRandomPoolOfTrees(populationSize);
			starter = ProcessPoolOfTrees(starter);
			//do the gene operations
			
			List<Tree> newCreations = new List<Tree>();
			//TODO add a step to check for "convergence" and stop iterating
			for (int generationNumber = 0; generationNumber < generations; generationNumber++)
			{
				//thin down the herd and take pop size or total
				starter = starter.OrderByDescending(c => c._prevScore).Take(Math.Min(populationSize, starter.Count)).ToList();
				
				//output some info on best
				Trace.WriteLine(string.Join("\r\n", starter.Take(10).Select(c => c._prevScore.ToString())));
				Trace.WriteLine(starter.First());				
				Trace.WriteLine("generation: " + generationNumber);
				
				for (int populationNumber = 0; populationNumber < populationSize; populationNumber++)
				{
					//make a new one!
					double tester = rando.NextDouble();
					//TODO: all of these stubs need to be turned into a new class that abstracts away the behavior
					if (tester < 0.4)
					{
						//node swap						
						Tree tree1 = starter[rando.Next(starter.Count())];						
						Tree tree2 = starter[rando.Next(starter.Count())];
						
						Tree tree1_copy = tree1.Copy();
						Tree tree2_copy = tree2.Copy();
							
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
					/*else if (false && tester < 0.35)
					{
						//TODO implement deletion or some sort of trimming operation, deleting nodes may not be best w/ terminal nodes
						//node deletion, not implemented for now				
						
					}*/
					else if (tester < 0.8)
					{
						//node parameter/value change
						
						Tree tree1 = starter[rando.Next(starter.Count())];
						Tree tree1_copy = tree1.Copy();						
						
						TreeNode node1_copy = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
						
						//TODO split out changing the parameter and value
						//TODO allow for the value to be modifying (add/subtract/etc. to make smaller changes)
						
						
						if (node1_copy.IsTerminal)
						{
							node1_copy.Classification = dataPointMgr.classes[rando.Next(dataPointMgr.classes.Length)];
						}
						else
						{
							var test = node1_copy.Test;
							test.param = rando.Next(dataPointMgr.DataColumnCount);
							DataColumn col = dataPointMgr._columns[test.param];
							test.valTest = col.GetTestValue(rando);
						}
						
						newCreations.Add(tree1_copy);
					}
					else
					{
						//all random new
						newCreations.Add(CreateRandomTree());
					}
				
				}
				//thin down the new creations
				newCreations = ProcessPoolOfTrees(newCreations);
				
				//add the good ones back to the main population
				starter.AddRange(newCreations);
				
				//TODO evaluate the trees somehow to determine the similarity and overlap of them all
				
				OnProgressUpdated(100 * generationNumber / generations);
			}
			//TODO add a step at the end to verify the results with a hold out data set
			//TODO add the ability to use the best tree for prediction and generate a results file w/ the predictions
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
		public TreeNode CreateRandomNode(Tree tree)
		{
			TreeNode node = new TreeNode();
			
			tree.AddNodeWithoutChildren(node);
			
			//TODO take this and other probabilities and move them somewhere central
			node.IsTerminal = rando.NextDouble() > 0.5;
			
			//TODO: consider changing this or using some other scheme to prevent runaway initial trees.					
			if (tree._nodes.Count > max_node_count_for_new_tree)
			{
				node.IsTerminal = true;
			}
			if (node.IsTerminal)
			{
				//TODO move this random class picking somewhere else
				node.Classification = dataPointMgr.classes[rando.Next(dataPointMgr.classes.Length)];
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



