using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using MoreLinq;
namespace GeneTree
{
	public static class GeneticOperations
	{
		public delegate IEnumerable<Tree> GeneticOperation(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation);
		
		public static IEnumerable<Tree> SwapNodesBetweenTrees(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			Random rando = ga_mgr.rando;
			//node swap
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree2 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			
			if (tree1._root.IsTerminal || tree2._root.IsTerminal)
			{
				//terminal node for a root means no useful swapping to be done
				yield break;
			}
			
			tree1.SetStructuralLocationsForNodes();
			tree2.SetStructuralLocationsForNodes();
			
			Tree tree1_copy = tree1.Copy();
			Tree tree2_copy = tree2.Copy();
			
			tree1_copy._source = "swap";
			tree2_copy._source = "swap";
			
			//tries to pick good nodes to swap around
			//TODO look into a better way to pick nodes		
			//TODO make it easier to select a node with equal weight			
			var tree1_node_picker = new WeightedSelector<YesNoMissingTreeNode>(
				                        tree1.GetNodesOfType<YesNoMissingTreeNode>().Select(c => Tuple.Create(c, 1.0)));
			
			var tree2_node_picker = new WeightedSelector<YesNoMissingTreeNode>(
				                        tree2.GetNodesOfType<YesNoMissingTreeNode>().Select(c => Tuple.Create(c, 1.0)));
			
			var node1_picked = tree1_node_picker.PickRandom(ga_mgr.rando);
			var node2_picked = tree2_node_picker.PickRandom(ga_mgr.rando);
			
			if (node1_picked == null || node2_picked == null)
			{
				//trap exists for those trees where there are no classification nodes
				yield break;
			}
			
			TreeNode node1 = tree1_copy.GetNodeAtStructualLocation(node1_picked._structuralLocation);
			TreeNode node2 = tree2_copy.GetNodeAtStructualLocation(node2_picked._structuralLocation);
			
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

		public static IEnumerable<Tree> DeleteNodeFromTree(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//node deletion
			Random rando = ga_mgr.rando;
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree1_copy = tree1.Copy();
			
			TreeNode node1 = tree1_copy._nodes[rando.Next(tree1_copy._nodes.Count)];
			
			var node1_rando_term = TreeNode.TreeNodeFactory(ga_mgr, true, tree1_copy) as ClassificationTreeNode;
			
			//stick the new node into the old one's spot
			
			node1_rando_term.Classification = -1;
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
		
		public static IEnumerable<Tree> OptimizeClassesForTree(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			Random rando = ga_mgr.rando;
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			
			tree1.SetStructuralLocationsForNodes();
			
			Tree tree1_copy = tree1.Copy();
			
			//chose a node that is a classification node (choose on tree 1 to check classes)
			
			var nodes_to_choose_from = tree1.GetNodesOfType<ClassificationTreeNode>();
			
			if (!nodes_to_choose_from.Any())
			{
				//empty collection test
				yield break;
			}
			
			//iterate through all nodes, find best class, store struct location for that node along with best class
			//iterate through that list, replace classes for foudn nodes
			
			
			bool changeMade = false;
			foreach (var node in nodes_to_choose_from)
			{
				double bestRow = node.matrix.GetRowWithMaxCount();
				if (node.Classification != bestRow)
				{
					var node1_copy = tree1_copy.GetNodeAtStructualLocation(node._structuralLocation) as ClassificationTreeNode;
				
					if (node1_copy != null)
					{
						node1_copy.Classification = bestRow;
						changeMade = true;
					}
				}
			}
			
			if (changeMade)
			{
				tree1_copy._source = "optimize tree classes";
				yield return tree1_copy;
			}
			else
			{
				yield break;
			}
		}
		
		public static IEnumerable<Tree> OptimizeSplitForNode(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//pick a random tree
			Random rando = ga_mgr.rando;
			
			Tree tree1 = treesInPopulation[rando.Next(treesInPopulation.Count())];
			Tree tree1_copy = tree1.Copy();
			
			var nodes_to_choose_from = tree1_copy.GetNodesOfType<YesNoMissingTreeNode>().Where(c => c.Test is LessThanEqualTreeTest);
			
			if (!nodes_to_choose_from.Any())
			{
				//empty collection test
				yield break;
			}
			
			var node_picker = new WeightedSelector<YesNoMissingTreeNode>(
				                  nodes_to_choose_from.Select(c => Tuple.Create(c, 1.0))
			                  );
			
			YesNoMissingTreeNode node1_copy = node_picker.PickRandom(rando);
			
			if (node1_copy == null)
			{
				yield break;
			}
			
			//iterate through all values for the node and use the one with the best impurity
			
			if (OptimizeTest(node1_copy, ga_mgr))
			{
				
				tree1_copy._source = "optimize value";
				
				yield return tree1_copy;
			}
			
			yield break;
			
		}
		
		public static bool OptimizeTest(YesNoMissingTreeNode node1_copy, GeneticAlgorithmManager ga_mgr)
		{
			if (node1_copy.Test is LessThanEqualTreeTest)
			{
				LessThanEqualTreeTest test = node1_copy.Test as LessThanEqualTreeTest;
			
				if (test == null)
				{
					return false;
				}
				//iterate through all values, make split, test impurity
				var values = ga_mgr.dataPointMgr._pointsToTest.Select(c => c._data[test.param]);
				var all_uniques = values.Where(c => !c._isMissing).Select(c => c._value).Distinct().OrderBy(c => c).ToArray();
				
				List<double> all_splits = new List<double>();
				for (int i = 1; i < all_uniques.Length; i++)
				{
					all_splits.Add(0.5 * (all_uniques[i] + all_uniques[i - 1]));
				}
				
				double best_split = double.NaN;
				double best_purity = double.MinValue;
				
				//TODO improve this selection for how many split points to consider
				foreach (var split in all_splits.TakeEvery(all_splits.Count / 10 + 1))
				{
					//change the test value and find the best purity
					test.valTest = split;
					
					var results = new GeneticAlgorithmRunResults(ga_mgr);
					
					//reset the node
					node1_copy._tree._root.ResetTrackingDetails(ga_mgr, true);
					
					//TODO really need to remove this usage of gettestpoints
					foreach (var dataPoint in ga_mgr.dataPointMgr._pointsToTest)
					{
						//TODO this probably does not need to go all the way down the tree
						node1_copy._tree._root.TraverseData(dataPoint, results);
					}
					
					//check the result of the split
					var gini_d = node1_copy.matrix.GiniImpuritySqrt;
					
					double gini_split = 0.0;
					int count = 0;
					
					foreach (var node in node1_copy._subNodes)
					{
						gini_split += node.matrix._count * node.matrix.GiniImpuritySqrt;
						count += node.matrix._count;
					}
					
					gini_split /= count;
					
					double gini_gain = gini_d - gini_split;
					
					if (gini_gain > best_purity)
					{
						best_split = split;
						best_purity = gini_gain;
					}
				}
				
				test.valTest = best_split;
			}
			else if (node1_copy.Test is EqualTreeTest)
			{
				EqualTreeTest test = node1_copy.Test as EqualTreeTest;
			
				if (test == null)
				{
					return false;
				}
				//iterate through all values, make split, test impurity
				var values = ga_mgr.dataPointMgr._pointsToTest.Select(c => c._data[test._param]);
				var all_uniques = values.Where(c => !c._isMissing).Select(c => c._value).Distinct().OrderBy(c => c).ToArray();
				
				List<double> all_splits = new List<double>();
				for (int i = 1; i < all_uniques.Length; i++)
				{
					all_splits.Add(0.5 * (all_uniques[i] + all_uniques[i - 1]));
				}
				
				double best_split = double.NaN;
				double best_purity = double.MinValue;
				
				//TODO improve this selection for how many split points to consider
				foreach (var split in all_splits.TakeEvery(all_splits.Count / 10 + 1))
				{
					//change the test value and find the best purity
					test._valTest = split;
					
					var results = new GeneticAlgorithmRunResults(ga_mgr);
					
					//reset the node
					node1_copy._tree._root.ResetTrackingDetails(ga_mgr, true);
					
					//TODO really need to remove this usage of gettestpoints
					foreach (var dataPoint in ga_mgr.dataPointMgr._pointsToTest)
					{
						//TODO this probably does not need to go all the way down the tree
						node1_copy._tree._root.TraverseData(dataPoint, results);
					}
					
					//check the result of the split
					var gini_d = node1_copy.matrix.GiniImpuritySqrt;
					
					double gini_split = 0.0;
					int count = 0;
					
					foreach (var node in node1_copy._subNodes)
					{
						gini_split += node.matrix._count * node.matrix.GiniImpuritySqrt;
						count += node.matrix._count;
					}
					
					gini_split /= count;
					
					double gini_gain = gini_d - gini_split;
					
					if (gini_gain > best_purity)
					{
						best_split = split;
						best_purity = gini_gain;
					}
				}
				
				test._valTest = best_split;
			}
			else if (node1_copy.Test is LinearComboTreeTest)
			{
				LinearComboTreeTest test = node1_copy.Test as LinearComboTreeTest;
				
				if (test == null)
				{
					return false;
				}
				//iterate through all values, make split, test impurity
				var all_uniques = ga_mgr.dataPointMgr._pointsToTest
					.Where(c => !test.IsMissingTest(c))
					.Select(c => test.GetValue(c))
					.Distinct()
					.OrderBy(c => c).ToArray();
				
				List<double> all_splits = new List<double>();
				all_splits.AddRange(all_uniques);
				
				double best_split = double.NaN;
				double best_purity = double.MinValue;
				
				if (all_splits.Count > 15)
				{
					return false;
				}
				
				//TODO improve this selection for how many split points to consider
				foreach (var split in all_splits)
				{
					//change the test value and find the best purity
					test.intercept = split;
					
					var results = new GeneticAlgorithmRunResults(ga_mgr);
					
					//reset the node
					node1_copy._tree._root.ResetTrackingDetails(ga_mgr, true);
					
					//TODO really need to remove this usage of gettestpoints
					foreach (var dataPoint in ga_mgr.dataPointMgr._pointsToTest)
					{
						//TODO this probably does not need to go all the way down the tree
						node1_copy._tree._root.TraverseData(dataPoint, results);
					}
					
					//check the result of the split
					var gini_d = node1_copy.matrix.GiniImpuritySqrt;
					
					double gini_split = 0.0;
					int count = 0;
					
					foreach (var node in node1_copy._subNodes)
					{
						gini_split += node.matrix._count * node.matrix.GiniImpuritySqrt;
						count += node.matrix._count;
					}
					
					gini_split /= count;
					
					double gini_gain = gini_d - gini_split;
					
					if (gini_gain > best_purity)
					{
						best_split = split;
						best_purity = gini_gain;
					}
				}
				
				test.intercept = best_split;
			}
			else
			{
				return false;
			}
			
			return true;
		}

		public static IEnumerable<Tree> SplitNodeAndOptimizeTests(GeneticAlgorithmManager ga_mgr, List<Tree> treesInPopulation)
		{
			//find a grab a tree
			Tree tree1 = treesInPopulation[ga_mgr.rando.Next(treesInPopulation.Count())];
			
			//uses the traversal count for selecting
			var node_picker = new WeightedSelector<ClassificationTreeNode>(
				                  tree1.GetNodesOfType<ClassificationTreeNode>().Select(c => Tuple.Create(c, (double)c._traverseCount))
			                  );
			
			ClassificationTreeNode node1 = node_picker.PickRandom(ga_mgr.rando);
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
				var node1_decision = new YesNoMissingTreeNode();
				
				node1_decision.CreateRandom(ga_mgr);
				node1_decision.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1_decision._parent = node1_copy._parent;
				
				var class_picker = new WeightedSelector<int>(matrix_rows);
				
				var item1 = node1.Classification;
				var item2 = class_picker.PickRandom(ga_mgr.rando);
				
				while (item1 == item2)
				{
					item2 = class_picker.PickRandom(ga_mgr.rando);
				}
				
				var item3 = 0;
				
				//create the two classification nodes
				var node1a_class = new ClassificationTreeNode();
				node1a_class.Classification = item1;
				node1a_class.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1a_class._parent = node1_decision;
				
				var node1b_class = new ClassificationTreeNode();
				node1b_class.Classification = item2;
				node1b_class.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1b_class._parent = node1_decision;
				
				var node1c_class = new ClassificationTreeNode();
				node1c_class.Classification = item3;
				node1c_class.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);
				node1c_class._parent = node1_decision;
				
				node1_decision._trueNode = node1a_class;
				node1_decision._falseNode = node1b_class;
				node1_decision._missingNode = node1c_class;
				
				//add the two nodes with the most popular classes
				//TODO create a "replace node" operation to standardize this code
				tree1_copy.RemoveNodeWithChildren(node1_copy);
				
				node1_copy.UpdateParentReference(node1_decision);
				tree1_copy.AddNodeWithChildren(node1_decision);
				//return the new tree with that change
			
				//try to optimize the node
				bool opTest = OptimizeTest(node1_decision, ga_mgr);
				
				tree1_copy._source = "node split";
				
				if (opTest)
				{
					tree1_copy._source += " w/ op";
				}
			
				yield return tree1_copy;
			}
		}
	}
}

