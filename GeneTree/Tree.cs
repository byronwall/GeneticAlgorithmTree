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
	public class Tree
	{
		public TreeNode root;
		public Dictionary<TreeNode, List<TreeNode>> edges = new Dictionary<TreeNode, List<TreeNode>>();

		public Tree Copy()
		{
			Tree copy = new Tree();
			copy.root = this.root;
			copy.edges = new Dictionary<TreeNode, List<TreeNode>>();

			foreach (var item in this.edges)
			{
				copy.edges[item.Key] = new List<TreeNode>(item.Value);
			}

			return copy;
		}
		public void RemoveNode(TreeNode node)
		{
			if (edges.ContainsKey(node))
			{
				//find the node in edges
				foreach (var child in edges[node])
				{
					//remove each child recursively
					RemoveNode(child);
				}

				//remove the node from edges
				edges.Remove(node);
			}
		}

		public void AddNodeWithChildrenFromTree(Tree sourceTree, TreeNode node)
		{
			//add the node to the current list of edges
			if (sourceTree.edges.ContainsKey(node))
			{
				edges.Add(node, new List<TreeNode>(sourceTree.edges[node]));

				foreach (var child in sourceTree.edges[node])
				{
					AddNodeWithChildrenFromTree(sourceTree, child);
				}

				//go through the edges in teh source tree and duplicate with new lists

				//do this recursively
			}
		}

		public bool ContainsNodeOrChildren(Tree sourceTree, TreeNode node)
		{
			if (edges.ContainsKey(node))
			{
				return true;
			}

			if (!sourceTree.edges.ContainsKey(node))
			{
				return false;
			}

			foreach (var child in sourceTree.edges[node])
			{
				if (ContainsNodeOrChildren(sourceTree, child))
				{
					return true;
				}
			}

			return false;

		}

		public bool TraverseData(DataPoint point)
		{
			/* TODO improve the speed of this call (and overload).
 			* it's by far the most expensive.
			* will probably do better to get rid of dictionary.
			* the GetItem calls are the worst part 
			*/
			return TraverseData(root, point);
		}

		public TreeNode FindParentNode(TreeNode child)
		{
			return FindParentNode(child, root);
		}
		public TreeNode FindParentNode(TreeNode child, TreeNode possibleParent)
		{
			//stop if child is in list of edges
			if (possibleParent.IsTerminal)
			{
				return null;
			}

			if (edges[possibleParent].Contains(child))
			{
				return possibleParent;
			}
			else
			{
				foreach (var nextPossibleParent in edges[possibleParent])
				{
					var temp = FindParentNode(child, nextPossibleParent);

					if (temp != null)
					{
						return temp;
					}
				}
			}

			return null;
		}

		public bool TraverseData(TreeNode node, DataPoint point)
		{
			//start at root, test if correct
			if (node.IsTerminal)
			{
				return node.Classification == point._classification._value;
			}
			else
			{
				//do the test and then traverse
				if (node.Test.isTrueTest(point))
				{
					//0 will be yes
					return TraverseData(edges[node][0], point);
				}
				else
				{
					//1 will be no
					return TraverseData(edges[node][1], point);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			Stack<TreeNode> nodes = new Stack<TreeNode>();
			nodes.Push(root);

			while (nodes.Count > 0)
			{
				var node = nodes.Pop();
				sb.AppendLine(node.ToString());

				if (!node.IsTerminal)
				{
					edges[node].ForEach(nodes.Push);
				}
			}

			return sb.ToString();
		}
	}
}
