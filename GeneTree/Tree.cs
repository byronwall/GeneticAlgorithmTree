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
		
		public int ProcessDataThroughTree(IEnumerable<DataPoint> dataPoints, ConfusionMatrix matrix)
		{
			int correct = 0;
			foreach (var dataPoint in dataPoints)
			{
				if (TraverseData(dataPoint, matrix))
				{
					correct++;		
				}
			}			
			return correct;
		}

		public bool TraverseData(TreeNode node, DataPoint point, ConfusionMatrix matrix)
		{
			//start at root, test if correct
			if (node.IsTerminal)
			{
				//these are known to be ints since they are classes from a Codebook
				matrix.AddItem((int)point._classification._value, (int)node.Classification);
				return node.Classification == point._classification._value;
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

			Stack<TreeNode> nodes = new Stack<TreeNode>();
			nodes.Push(_root);

			while (nodes.Count > 0)
			{
				var node = nodes.Pop();
				sb.AppendLine(node.ToString());

				if (!node.IsTerminal)
				{
					nodes.Push(node._trueNode);
					nodes.Push(node._falseNode);
				}
			}
			return sb.ToString();
		}
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

