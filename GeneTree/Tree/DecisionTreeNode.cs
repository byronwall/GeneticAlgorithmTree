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
	public class DecisionTreeNode : TreeNode
	{
		public override TreeNode TraverseData(DataPoint point, GeneticAlgorithmRunResults results)
		{
			this._traverseCount++;
			
			return this.Test.isTrueTest(point) ?
				this._trueNode.TraverseData(point, results) :
				this._falseNode.TraverseData(point, results);
		}

		public TreeTest Test;
		public TreeNode _trueNode;
		public TreeNode _falseNode;
		
		public override bool IsTerminal
		{
			get
			{
				return false;
			}
		}

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
			new_node.matrix = new ConfusionMatrix(this.matrix._size);
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
}
