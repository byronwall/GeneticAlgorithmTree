using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GeneTree
{
	[Serializable]
	public class Tree
	{
		public TreeNode _root;

		[XmlIgnore]
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
			//TODO this should return a full GeneticAlgorithmRunResults which can be processed
			
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
		
		public void WriteToXmlFile(string filePath)
		{
			TextWriter writer = null;
			try
			{
				var serializer = new XmlSerializer(this.GetType());
				writer = new StreamWriter(filePath);
				serializer.Serialize(writer, this);
			}
			finally
			{
				if (writer != null)
				{
					writer.Close();
				}
			}
		}
		/// <summary>
		/// Reads a Tree in from an XML file.  Has to process the nodes and hierarchy after the fact to avoid circular references.
		/// </summary>
		/// <param name="filePath">location of XML file</param>
		/// <returns>a fully assembled Tree</returns>
		public static Tree ReadFromXmlFile(string filePath)
		{
			TextReader reader = null;
			Tree tree_read = null;
			try
			{
				var serializer = new XmlSerializer(typeof(Tree));
				reader = new StreamReader(filePath);
				tree_read = (Tree)serializer.Deserialize(reader);
				
				if (tree_read != null)
				{
					var nodes_to_process = new Stack<Tuple<TreeNode, TreeNode>>();
					nodes_to_process.Push(Tuple.Create(tree_read._root, (TreeNode)null));
				
					while (nodes_to_process.Count > 0)
					{
						var node_parent = nodes_to_process.Pop();
					
						var node = node_parent.Item1;
						tree_read._nodes.Add(node);
						node._parent = node_parent.Item2;					
						node._tree = tree_read;
					
						if (!node.IsTerminal)
						{
							nodes_to_process.Push(Tuple.Create(node._trueNode, node));
							nodes_to_process.Push(Tuple.Create(node._falseNode, node));
						}
					}
				}
				else
				{
					throw new Exception("something went wrong reading the tree back");
				}
			
				return tree_read;
			}
			finally
			{
				if (reader != null)
					reader.Close();
			}
		}
	}
	
}

