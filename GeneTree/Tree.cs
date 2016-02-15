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
		public TreeNode _root;
		public List<TreeNode> _nodes = new List<TreeNode>();
		public double _prevScore = double.MinValue;
		
		//TODO remove this for better tracking somewhere else
		public string _source;
		
		public bool _isDirty = true;

		public Tree Copy()
		{
			//create a new tree
			Tree new_tree = new Tree();
			new_tree._root = this._root.ReturnFullyLinkedCopyOfSelf();			
			new_tree.AddNodeWithChildren(new_tree._root);
			new_tree._prevScore = this._prevScore;
			
			return new_tree;			
		}
		public void RemoveNodeWithChildren(TreeNode node)
		{
			_nodes.Remove(node);
			
			if (!node.IsTerminal)
			{
				RemoveNodeWithChildren(node._trueNode);
				RemoveNodeWithChildren(node._falseNode);				
			}
		}
		
		public void AddRootToTree(TreeNode node)
		{
			this._root = node;
			
			AddNodeWithoutChildren(node);
		}

		public void AddNodeWithoutChildren(TreeNode node)
		{
			_nodes.Add(node);
			node._tree = this;
		}
		public void AddNodeWithChildren(TreeNode node)
		{
			AddNodeWithoutChildren(node);
			
			if (!node.IsTerminal)
			{
				AddNodeWithChildren(node._trueNode);
				AddNodeWithChildren(node._falseNode);				
			}
		}
		public bool TraverseData(DataPoint point, ConfusionMatrix matrix)
		{
			return TraverseData(_root, point, matrix);
		}
		
		public ConfusionMatrix ProcessDataThroughTree(DataPointManager dataPointMgr)
		{
			//clear out the node counts
			this._nodes.ForEach(c => c._traverseCount = 0);
			
			
			ConfusionMatrix matrix = new ConfusionMatrix(dataPointMgr.classes.Length);
			foreach (var dataPoint in dataPointMgr._pointsToTest)
			{
				TraverseData(dataPoint, matrix);
			}			
			return matrix;
		}

		public bool TraverseData(TreeNode node, DataPoint point, ConfusionMatrix matrix)
		{
			node._traverseCount++;
			
			//start at root, test if correct
			if (node.IsTerminal)
			{
				//-1 will be the no classificaiton route for now
				if (node.Classification == -1)
				{
					return false;
				}
				else
				{
					//these are known to be ints since they are classes from a Codebook
					matrix.AddItem((int)point._classification._value, (int)node.Classification);
					return true;
				}
			}
			else
			{
				//do the test and then traverse
				if (node.Test.isTrueTest(point))
				{
					//0 will be yes
					return TraverseData(node._trueNode, point, matrix);
				}
				else
				{
					//1 will be no
					return TraverseData(node._falseNode, point, matrix);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			Stack<Tuple<TreeNode, int>> nodes = new Stack<Tuple<TreeNode, int>>();
			nodes.Push(new Tuple<TreeNode, int>(_root, 0));
			
			while (nodes.Count > 0)
			{
				var item = nodes.Pop();
				var node = item.Item1;
				
				sb.AppendLine();
				
				for (int i = 0; i < item.Item2; i++)
				{
					sb.Append("  ");
				}
				sb.Append(node.ToString());

				if (!node.IsTerminal)
				{
					nodes.Push(new Tuple<TreeNode, int>(node._trueNode, item.Item2 + 1));
					nodes.Push(new Tuple<TreeNode, int>(node._falseNode, item.Item2 + 1));
				}
			}
			return sb.ToString();
		}
		//TODO consider removing the Hash and Equals, not sure there are any duplicates
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked
			{
				if (_root != null)
					hashCode += 1000000007 * _root.GetHashCode();
				if (_nodes != null)
					hashCode += 1000000009 * _nodes.GetHashCode();
				hashCode += 1000000021 * _prevScore.GetHashCode();
			}
			return hashCode;
		}

		public override bool Equals(object obj)
		{
			Tree other = obj as Tree;
			if (other == null)
				return false;
			return this.ToString() == other.ToString();
		}
	}
	
}

