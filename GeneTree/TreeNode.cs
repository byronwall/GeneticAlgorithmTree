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
	public class ClassificationTreeNode : TreeNode
	{
		public override bool TraverseData(DataPoint point, GeneticAlgorithmRunResults results)
		{
			this._traverseCount++;
			
			//-1 will be the no classificaiton route for now
			if (this.Classification == -1.0)
			{					
				return false;
			}
			else
			{
				//these are known to be ints since they are classes from a Codebook
				results.count_classedData++;
				results._matrix.AddItem((int)point._classification._value, (int)this.Classification);
				return true;
			}
		}

		public double Classification;
		
		public override TreeNode CopyNonLinkingData()
		{
			ClassificationTreeNode new_node = new ClassificationTreeNode();
			new_node.Classification = this.Classification;
			
			return new_node;
		}
		
		public override string ToString()
		{
			string output = Classification == -1.0 ? 
				string.Format("NO CLASS") : 
				string.Format("TERM to {0}", Classification);
				
			return output + string.Format(" ({0})", this._traverseCount);
		}
		public override void CreateRandom(GeneticAlgorithmManager ga_mgr)
		{
			this.Classification = ga_mgr.dataPointMgr.GetRandomClassification(ga_mgr.rando);
		}
		
		public override bool UpdateChildReference(TreeNode curRef, TreeNode newRef)
		{
			return false;			
		}
		
		public override void ApplyRandomChangeToNodeValue(GeneticAlgorithmManager ga_mgr)
		{
			//can get away with just doing the random thing here
			this.CreateRandom(ga_mgr);
			this._tree._source = "new class";
		}
	}
	
	[Serializable]
	public class DecisionTreeNode : TreeNode
	{
		public override bool TraverseData(DataPoint point, GeneticAlgorithmRunResults results)
		{
			this._traverseCount++;
			
			return this.Test.isTrueTest(point) ?
				this._trueNode.TraverseData(point, results) : 
				this._falseNode.TraverseData(point, results);
		}

		public TreeTest Test;
		public TreeNode _trueNode;
		public TreeNode _falseNode;
		
		public override void FillNodeWithRandomChildrenIfNeeded(GeneticAlgorithmManager ga_mgr)
		{
			this._trueNode = TreeNode.TreeNodeFactory(ga_mgr, false, this._tree);
			this._trueNode._parent = this;
			
			this._falseNode = TreeNode.TreeNodeFactory(ga_mgr, false, this._tree);
			this._falseNode._parent = this;
		}
		
		public override void ApplyRandomChangeToNodeValue(GeneticAlgorithmManager ga_mgr)
		{
			//TODO add somet logic here to handle the different test type... maybe pass this into the test next
			if (ga_mgr.rando.NextDouble() < 0.8)
			{
				//just change the value
				bool result = this.Test.ChangeTestValue(ga_mgr);
				this._tree._source = "new test value";
			}
			else
			{
				this.Test = TreeTest.TreeTestFactory(ga_mgr);
				this._tree._source = "new test";
			}
		}
		
		public override bool UpdateChildReference(TreeNode curRef, TreeNode newRef)
		{
			if (curRef == _trueNode)
			{
				_trueNode = newRef;
				return true;
			}
			else if (curRef == _falseNode)
			{
				_falseNode = newRef;
				return true;
			}
			
			throw new Exception("should not be able to get to this point");
		}
		
		public override IEnumerable<TreeNode> _subNodes
		{
			get
			{
				yield return _trueNode;
				yield return _falseNode;
			}
		}
		
		public override TreeNode CopyNonLinkingData()
		{
			DecisionTreeNode new_node = new DecisionTreeNode();
			
			new_node.Test = this.Test.Copy();
			
			return new_node;
		}
		public override void CreateRandom(GeneticAlgorithmManager ga_mgr)
		{
			this.Test = TreeTest.TreeTestFactory(ga_mgr);
		}
		
		public override string ToString()
		{
			return Test + string.Format(" ({0})", this._traverseCount);
		}
		
		public override TreeNode ReturnFullyLinkedCopyOfSelf()
		{
			//know that it is a decision tree since it is self
			DecisionTreeNode self_copy = (DecisionTreeNode)this.CopyNonLinkingData();
			
			TreeNode true_copy = _trueNode.ReturnFullyLinkedCopyOfSelf();
			TreeNode false_copy = _falseNode.ReturnFullyLinkedCopyOfSelf();
			
			self_copy._trueNode = true_copy;
			self_copy._falseNode = false_copy;
			
			true_copy._parent = self_copy;
			false_copy._parent = self_copy;
				
			return self_copy;
		}
	}
	
	[Serializable]
	[XmlInclude(typeof(DecisionTreeNode))]
	[XmlInclude(typeof(ClassificationTreeNode))]
	public abstract class TreeNode
	{
		[XmlIgnore]
		public TreeNode _parent;
		
		public virtual bool IsTerminal
		{ 
			get { return true; }
		}
        
		[XmlIgnore]
		public Tree _tree;
		public int _traverseCount;
		
		public abstract TreeNode CopyNonLinkingData();
		public abstract void CreateRandom(GeneticAlgorithmManager ga_mgr);
		public abstract bool TraverseData(DataPoint point, GeneticAlgorithmRunResults results);
		public abstract bool UpdateChildReference(TreeNode curRef, TreeNode newRef);
		public abstract void ApplyRandomChangeToNodeValue(GeneticAlgorithmManager ga_mgr);
		public virtual void FillNodeWithRandomChildrenIfNeeded(GeneticAlgorithmManager ga_mgr){
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
				var node = new DecisionTreeNode();
				node.CreateRandom(ga_mgr);
				
				node_output = node;
			}

			tree.AddNodeWithoutChildren(node_output);			
			return node_output;
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
			TreeNode node1_parent = node1._parent;
			
			//node1 parent swaps
			node2._parent.UpdateChildReference(node2, node1);
			node1._parent = node2._parent;
			
			node1_parent.UpdateChildReference(node1, node2);
			node2._parent = node1_parent;
			
			//add nodes back to new trees
			node1._tree.AddNodeWithChildren(node1);
			node2._tree.AddNodeWithChildren(node2);
		}
	}
}

