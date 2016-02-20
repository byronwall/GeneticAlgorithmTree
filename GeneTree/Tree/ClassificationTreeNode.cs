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
				
				this.ProcessResultFromClassification(point._classification._value, this.Classification);
				
				return true;
			}
		}

		public double Classification;
		
		public override bool IsTerminal
		{
			get
			{
				return true;
			}
		}

		public override TreeNode CopyNonLinkingData()
		{
			ClassificationTreeNode new_node = new ClassificationTreeNode();
			new_node.Classification = this.Classification;
			new_node.matrix = new ConfusionMatrix(this.matrix._size);
			return new_node;
		}

		public override string ToString()
		{
			string output = Classification == -1.0 ? string.Format("NO CLASS") : string.Format("TERM to {0}", Classification);
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
}



