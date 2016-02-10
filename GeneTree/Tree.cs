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
		public List<TreeNode> _nodes = new List<TreeNode>();
		
		public double _prevScore = double.MinValue;

		public Tree Copy()
		{
			//create a new tree
			Tree new_tree = new Tree();
			new_tree.root = this.root.ReturnFullyLinkedCopyOfSelf();			
			new_tree.AddNodeWithChildren(new_tree.root);
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
		
		public void AddNodeWithChildren(TreeNode node)
		{
			_nodes.Add(node);
			node._tree = this;
			
			if (!node.IsTerminal)
			{
				AddNodeWithChildren(node._trueNode);
				AddNodeWithChildren(node._falseNode);				
			}
		}
		public bool TraverseData(DataPoint point)
		{
			return TraverseData(root, point);
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
					return TraverseData(node._trueNode, point);
				}
				else
				{
					//1 will be no
					return TraverseData(node._falseNode, point);
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
					nodes.Push(node._trueNode);
					nodes.Push(node._falseNode);
				}
			}

			return sb.ToString();
		}
	}
}

