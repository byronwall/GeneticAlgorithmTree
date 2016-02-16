using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GeneTree
{
	[Serializable]	
	public class TreeNode
	{
		public TreeTest Test;
		
		//TODO, make IsTerminal a different type of node instead of bool switch
		public bool IsTerminal;
        
		public double Classification;
		
		public TreeNode _trueNode;
		public TreeNode _falseNode;
		
		[XmlIgnore]
		public TreeNode _parent;
		
		[XmlIgnore]
		public Tree _tree;
		
		public bool _parentTrue;
		
		public int _traverseCount;
		
		public override string ToString()
		{
			string output = "";
			
			if (IsTerminal)
			{
				if (Classification == -1)
				{
					output = string.Format("NO CLASS");	
				}
				else
				{
				
					output = string.Format("TERM to {0}", Classification);				
				}
				
			}
			else
			{
				output = Test.ToString();
			}
			
			return output + string.Format(" ({0})", this._traverseCount);
		}
		
		public TreeNode CopyNonLinkingData()
		{
			TreeNode new_node = new TreeNode();
			
			if (!IsTerminal)
			{
				new_node.Test = this.Test.Copy();
			}
			else
			{
				new_node.IsTerminal = this.IsTerminal;
				new_node.Classification = this.Classification;
			}
			
			return new_node;
		}
		
		public TreeNode ReturnFullyLinkedCopyOfSelf()
		{
			TreeNode self_copy = this.CopyNonLinkingData();
			
			if (!IsTerminal)
			{
				//not terminal, need to get copies of children first
				TreeNode true_copy = _trueNode.ReturnFullyLinkedCopyOfSelf();
				TreeNode false_copy = _falseNode.ReturnFullyLinkedCopyOfSelf();
			
				self_copy._trueNode = true_copy;
				self_copy._falseNode = false_copy;
			
				true_copy._parent = self_copy;
				false_copy._parent = self_copy;
				
				true_copy._parentTrue = true;
			}
			return self_copy;
		}
		public static void SwapNodesInTrees(TreeNode node1, TreeNode node2)
		{
			//TODO handle this better where the node to swap is the root, right now just exists with no change
			if (node1._parent == null || node2._parent == null)
			{
				return;
			}
			
			//remove nodes from trees
			node1._tree.RemoveNodeWithChildren(node1);
			node2._tree.RemoveNodeWithChildren(node2);
			
			//temps
			var node1_parent = node1._parent;
			var node1_parentTrue = node1._parentTrue;
			
			//node1 parent swaps
			if (node2._parentTrue)
			{
				node2._parent._trueNode = node1;
			}
			else
			{
				node2._parent._falseNode = node1;
			}
			node1._parent = node2._parent;
			node1._parentTrue = node2._parentTrue;
			
			if (node1._parentTrue)
			{
				node1._parent._trueNode = node2;
			}
			else
			{
				node1._parent._falseNode = node2;
			}
			node2._parent = node1_parent;
			node2._parentTrue = node1_parentTrue;
			
			//add nodes back to new trees
			node1._tree.AddNodeWithChildren(node1);
			node2._tree.AddNodeWithChildren(node2);
		}
	}
}

