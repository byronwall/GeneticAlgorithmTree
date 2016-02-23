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
	public static class GeneticOperations
	{
		public static IEnumerable<Tree> SwapNodesBetweenTrees(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			Random rando = ga_mgr.rando;
			//node swap
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree2 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			
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
				yield return tree1_copy;
			}
			if (tree2_copy._nodes.Count > 0)
			{
				yield return tree2_copy;
			}
		}

		[Obsolete]
		public static IEnumerable<Tree> DeleteNodeFromTree(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//node deletion
			Random rando = ga_mgr.rando;
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree1_copy = tree1.Copy();
			TreeNode node1 = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
			TreeNode node1_rando_term = TreeNode.TreeNodeFactory(ga_mgr, true, tree1_copy);
			//stick the new node into the old one's spot
			node1_rando_term._parent = node1._parent;
			tree1_copy.RemoveNodeWithChildren(node1);
			if (node1_rando_term._parent != null && tree1_copy._nodes.Count > 0)
			{
				node1_rando_term._parent.UpdateChildReference(node1, node1_rando_term);
				tree1_copy._source = "delete";
				yield return tree1_copy;
			}
		}

		public static IEnumerable<Tree> ChangeValueForNode(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//node parameter/value change
			Random rando = ga_mgr.rando;
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree1_copy = tree1.Copy();
			
			TreeNode node1_copy = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
			node1_copy.ApplyRandomChangeToNodeValue(ga_mgr);
			
			yield return tree1_copy;
		}

		public static IEnumerable<Tree> CreateRandomTree(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			Tree tree = ga_mgr.CreateRandomTree();
			tree._source = "new tree";
			yield return tree;
		}

		public static IEnumerable<Tree> SplitNodeWithMostPopularClasses(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//find a grab a tree
			Tree tree1 = treesInPopulation[ga_mgr.rando.Next(treesInPopulation.Count())];
			TreeNode node1 = tree1.GetNodesOfType<ClassificationTreeNode>().MaxBy(c => c._traverseCount);
			tree1.SetStructuralLocationsForNodes();
			var matrix_rows = node1.matrix.GetRowsOrderedByCount().ToList();
			
			//trap is here in case there are fewer than 2 "top" rows to split on
			if (matrix_rows.Count >= 2)
			{
			
				//TODO improve this structural business to be cleaner and more obvious if it belongs to the Node or Tree
				Tree tree1_copy = tree1.Copy();			
				TreeNode node1_copy = tree1_copy.GetNodeAtStructualLocation(node1._structuralLocation);
			
				//grab the node with the greatest traverse count (that is terminal)
				//determine the two most popular classes from there (rows in the confusion table)
				//create a random test for the current node (change from classification to decision)
				//TODO create a proper factory for this code
				var node1_decision = new DecisionTreeNode();
				node1_decision.CreateRandom(ga_mgr);
				node1_decision.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1_decision._parent = node1_copy._parent;
				
				//create the two classification nodes
				var node1a_class = new ClassificationTreeNode();
				node1a_class.Classification = matrix_rows[0];
				node1a_class.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1a_class._parent = node1_decision;
				
				var node1b_class = new ClassificationTreeNode();
				node1b_class.Classification = matrix_rows[1];
				node1b_class.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1b_class._parent = node1_decision;
				
				node1_decision._trueNode = node1a_class;
				node1_decision._falseNode = node1b_class;
				
				//add the two nodes with the most popular classes
				//TODO create a "replace node" operation to standardize this code
				tree1_copy.RemoveNodeWithChildren(node1_copy);
				
				node1_copy.UpdateParentReference(node1_decision);
				tree1_copy.AddNodeWithChildren(node1_decision);
				//return the new tree with that change
			
				tree1_copy._source = "node split";
			
				yield return tree1_copy;
			}
		}
	}
}

