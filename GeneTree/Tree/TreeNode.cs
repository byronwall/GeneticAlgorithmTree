using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GeneTree
{
	[Serializable]
	[XmlInclude(typeof(DecisionTreeNode))]
	[XmlInclude(typeof(ClassificationTreeNode))]
	[XmlInclude(typeof(YesNoMissingTreeNode))]
	public abstract class TreeNode
	{
		[XmlIgnore]
		public TreeNode _parent;
		
		[XmlIgnore]
		public ConfusionMatrix matrix;
		
		[XmlIgnore]
		public Tree _tree;
		public int _traverseCount;
		
		[XmlIgnore]
		public int _structuralLocation;
		
		public abstract TreeNode CopyNonLinkingData();
		public abstract void CreateRandom(GeneticAlgorithmManager ga_mgr);
		public abstract bool TraverseData(DataPoint point, GeneticAlgorithmRunResults results);
		public abstract bool UpdateChildReference(TreeNode curRef, TreeNode newRef);
		public abstract void ApplyRandomChangeToNodeValue(GeneticAlgorithmManager ga_mgr);
		
		public abstract bool IsTerminal{ get; }
		
		public virtual bool UpdateParentReference(TreeNode newRef)
		{
			if (this._parent != null)
			{
				return this._parent.UpdateChildReference(this, newRef);
			}
			
			return false;
		}
				
		public virtual void FillNodeWithRandomChildrenIfNeeded(GeneticAlgorithmManager ga_mgr)
		{
			return;
		}
		
		public virtual IEnumerable<TreeNode> _subNodes
		{
			get
			{
				yield break;
			}
		}
		
		public virtual TreeNode ReturnFullyLinkedCopyOfSelf()
		{
			return this.CopyNonLinkingData();
		}
		
		public virtual void ProcessResultFromClassification(double actual, double predicted)
		{
			//sends things up the chain
			//TODO clean up this null check.  was added for the case where running through tree outside of normal method (for predictions)
			if (this.matrix != null)
			{
				this.matrix.AddItem(actual, predicted);
			}
			
			if (this._parent != null)
			{
				this._parent.ProcessResultFromClassification(actual, predicted);
			}
		}
		
		public static TreeNode TreeNodeFactory(GeneticAlgorithmManager ga_mgr, bool ShouldForceTerminal, Tree tree)
		{	
			TreeNode node_output;
			
			bool term_node = ga_mgr.rando.NextDouble() > ga_mgr._gaOptions.prob_node_terminal;
			
			//TODO: consider changing this or using some other scheme to prevent runaway initial trees.					
			if (term_node || ShouldForceTerminal || tree._nodes.Count > ga_mgr._gaOptions.max_node_count_for_new_tree)
			{
				var node = new ClassificationTreeNode();
				node.CreateRandom(ga_mgr);

				node_output = node;
			}
			else
			{
				var node = new YesNoMissingTreeNode();
				node.CreateRandom(ga_mgr);
				
				node_output = node;
			}
			
			//TODO there might be a better place for this
			node_output.matrix = new ConfusionMatrix(ga_mgr.dataPointMgr.classes.Length);

			tree.AddNodeWithoutChildren(node_output);			
			return node_output;
		}
		
		public static bool SwapNodesInTrees(TreeNode node1, TreeNode node2)
		{
			//TODO get this method out of this class.  It looks quite out of place.
			//TODO handle this better where the node to swap is the root, right now just exists with no change
			if (node1._parent == null || node2._parent == null)
			{
				return false ;
			}
			
			var tree1 = node1._tree;
			var tree2 = node2._tree;
			
			//remove nodes from trees
			node1._tree.RemoveNodeWithChildren(node1);
			node2._tree.RemoveNodeWithChildren(node2);
			
			//temps
			TreeNode node1_parent = node1._parent;
			
			//node1 parent swaps
			node2._parent.UpdateChildReference(node2, node1);
			node1._parent = node2._parent;
			
			node1_parent.UpdateChildReference(node1, node2);
			node2._parent = node1_parent;
			
			//add nodes back to new trees
			tree1.AddNodeWithChildren(node2);
			tree2.AddNodeWithChildren(node1);
			
			return true;
		}
	}
}

