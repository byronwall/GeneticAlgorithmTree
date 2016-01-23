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
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			Trace.Listeners.Clear();

			TextWriterTraceListener twtl = new TextWriterTraceListener(string.Format("trace files/trace {0}.txt", DateTime.Now.Ticks));
			twtl.Name = "TextLogger";
			twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

			Trace.Listeners.Add(twtl);
			Trace.AutoFlush = true;
		}



		private void btnLoadData_Click(object sender, EventArgs e)
		{
			//TODO allow the program to accept aribitrary data location
			string path = "data/iris/iris.data";
			LoadDataFile(path);
		}

		private void LoadDataFile(string path)
		{
			//TODO this method should split the data into test/train in order to better evaluate the tree
			var reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

			bool firstLine = true;
			
			//parse the CSV data and create data points
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();

				if (line == string.Empty)
				{
					continue;
				}

				//this part is hard coded to IRIS data file
				var values = line.Split(',');
				
				//TODO fully process the header row, issue #6
				if(firstLine){
					dataPointMgr.SetHeaders(values);
					firstLine = false;
					continue;
				}				
				
				//TODO need to be able to deal with non-double data, load data into raw_data as string and then process, issue #7				
				var data = values.Take(values.Length - 1).Select(x => double.Parse(x)).ToArray();
				
				//TODO allow the assignment of classification columns to be user supplied
				dataPointMgr.dataPoints.Add(new DataPoint() { Data = data, Classification = values[values.Length - 1] });
			}

			//create classes and ranges
			dataPointMgr.DetermineClasses();

			//get min/max ranges for the data
			dataPointMgr.DetermineRanges();
		}

		Random rando = new Random();

		DataPointManager dataPointMgr = new DataPointManager();

		private Tree CreateRandomTree()
		{
			//build a random tree
			var tree = new Tree();
			var root = new TreeNode();
			var test = new TreeTest();
			test.param = rando.Next(dataPointMgr.paramCount);
			test.valTest = rando.NextDouble() * (dataPointMgr.ranges[test.param][1] - dataPointMgr.ranges[test.param][0]) + dataPointMgr.ranges[test.param][0];
			test.isLessThanEqualTest = rando.NextDouble() > 0.5;

			root.Test = test;
			tree.root = root;

			//run a queue to create children for non-terminal nodes
			Queue<TreeNode> nonTermNodes = new Queue<TreeNode>();
			nonTermNodes.Enqueue(root);

			while (nonTermNodes.Count > 0)
			{
				var node = nonTermNodes.Dequeue();

				//need to create two new nodes, yes and no
				List<TreeNode> yesNoNodes = new List<TreeNode>();

				for (int i = 0; i < 2; i++)
				{
					var nodeYes = new TreeNode();
					nodeYes.IsTerminal = rando.NextDouble() > 0.5;

					//TODO: consider changing this or using some other scheme to prevent runaway initial trees.
					if (tree.edges.Count > 12)
					{
						nodeYes.IsTerminal = true;
					}

					if (nodeYes.IsTerminal)
					{
						//class
						nodeYes.Classification = dataPointMgr.classes[rando.Next(dataPointMgr.classes.Length)];
					}
					else
					{
						var testYes = new TreeTest();
						testYes.param = rando.Next(dataPointMgr.paramCount);
						testYes.valTest = rando.NextDouble() * (dataPointMgr.ranges[testYes.param][1] - dataPointMgr.ranges[testYes.param][0]) + dataPointMgr.ranges[testYes.param][0];
						testYes.isLessThanEqualTest = rando.NextDouble() > 0.5;

						nodeYes.Test = testYes;

						nonTermNodes.Enqueue(nodeYes);
					}

					yesNoNodes.Add(nodeYes);
				}

				tree.edges.Add(node, yesNoNodes);
			}

			//output the tree structure
			//Trace.WriteLine(tree.ToString());

			return tree;
		}

		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			//CreateRandomPoolOfTrees(20);

			ProcessTheNextGeneration();

			MessageBox.Show("the test is completed");
		}

		private List<Tree> CreateRandomPoolOfTrees(int size)
		{
			//create a number of random trees and report results from all of them
			List<Tree> results = new List<Tree>();

			for (int i = 0; i < size; i++)
			{
				results.Add(CreateRandomTree());
			}

			return results;
		}

		private List<Tree> ProcessPoolOfTrees(IEnumerable<Tree> trees, int generationNumber)
		{
			//create a number of random trees and report results from all of them
			List<Tuple<double, Tree>> results = new List<Tuple<double, Tree>>();

			Trace.WriteLine("doing the eval: ratio");

			foreach (var tree in trees)
			{
				int correct = 0;
				foreach (var item in dataPointMgr.dataPoints)
				{
					//TODO add a Rand test here to only select some portion of the data for testing, maybe do a "big" test every so often
					if (tree.TraverseData(item))
					{
						correct++;
					}
				}

				double ratio = 1.0 * correct / dataPointMgr.dataPoints.Count;

				//report accuracy
				//Trace.WriteLine(String.Format("{0} of {1} = {2}", correct, dataPoints.Count, ratio));

				//TODO improve the hueristic for evaluation
				//TODO consider splitting this to consider tree size later in the game
				double score = (generationNumber > 25) ?
                    Math.Pow(ratio, 4) / Math.Log10(tree.edges.Count + 1) :
                    Math.Pow(ratio, 4) / Math.Log10(2);

				//score = ratio;

				results.Add(new Tuple<double, Tree>(score, tree));
			}

			Trace.WriteLine(String.Join("\r\n", results.Select(x => x.Item1).OrderByDescending(x => x).Take(Math.Min(trees.Count() / 2, 10))));
			Trace.WriteLine(results.OrderByDescending(x => x.Item1).Select(x => x.Item2).First());

			//TODO improve the selection here to not just take the top half, maybe iterate them all and select based on the score
			return results.OrderByDescending(x => x.Item1).Select(x => x.Item2).Take(trees.Count() / 2).ToList();
		}
		//TODO move the processing code into a GeneticOperations class to handle it all
		private void ProcessTheNextGeneration()
		{
			int populationSize = 200;

			//start with a list of trees and trim it down
			var starter = CreateRandomPoolOfTrees(populationSize);
			starter = ProcessPoolOfTrees(starter, 1);

			//do the gene operations
			int generations = 10;

			List<Tree> newCreations = new List<Tree>();

			//TODO add a step to check for "convergence" and stop iterating
			for (int generationNumber = 0; generationNumber < generations; generationNumber++)
			{
				Trace.WriteLine("generation: " + generationNumber);
				for (int populationNumber = 0; populationNumber < populationSize; populationNumber++)
				{
					//make a new one!
					var tester = rando.NextDouble();

					//TODO: all of these stubs need to be turned into a new class that abstracts away the behavior

					if (tester < 0.4)
					{
						//node swap

						//TODO, this needs to swap entire chunks of trees -or- create a new method that does that

						//pick a tree
						//pick a node in that tree
						Tree tree1 = starter[rando.Next(starter.Count())];

						//pick another tree
						//pick a node in that tree
						Tree tree2 = starter[rando.Next(starter.Count())];
						var edge2 = tree2.edges.ElementAt(rando.Next(tree2.edges.Count));

						//TODO: clean up this trap for a node that already exists in the tree
						if (tree1.ContainsNodeOrChildren(tree2, edge2.Key))
						{

						}
						else
						{
							//create a new tree which is a copy of node 1's tree (or randomly pick which one)
							Tree tree3 = tree1.Copy();
							var edge1 = tree3.edges.ElementAt(rando.Next(tree3.edges.Count));

							if (tree3.root == edge1.Key)
							{
								tree3.root = edge2.Key;
								tree3.RemoveNode(edge1.Key);
								tree3.AddNodeWithChildrenFromTree(tree2, edge2.Key);
							}
							else
							{
								//find the parent of node 1 in the new tree
								var parent1 = tree3.FindParentNode(edge1.Key);


								//change the edge for the parent to point to node 2 instead of node 1
								for (int k = 0; k < tree3.edges[parent1].Count; k++)
								{
									if (tree3.edges[parent1][k] == edge1.Key)
									{
										//swap the parent to the new node
										//bring the new node's edge into this tree
										//remove the old node from the edges
										tree3.edges[parent1][k] = edge2.Key;
										tree3.RemoveNode(edge1.Key);
										tree3.AddNodeWithChildrenFromTree(tree2, edge2.Key);

										//TODO, how does this step really work if the child nodes are not brought over?

										break;
									}
								}
							}

							tree2.TraverseData(dataPointMgr.dataPoints[0]);
							newCreations.Add(tree2);
						}

						//TODO: consider a check to prevent the same node from being added more than once

					}
                    //TODO remove this bypass on node deletion, the problem seems to be related to deleting terminal nodes?
                    else if (false && tester < 0.35)
					{
						//node deletion

						//pick a tree
						Tree tree1 = starter[rando.Next(starter.Count())];

						//TODO: clean up this trap for a root only tree
						if (tree1.edges.Count == 1)
						{

						}
						else
						{
							//make a copy of that tree
							Tree tree2 = tree1.Copy();
							//pick a node in that copy

							KeyValuePair<TreeNode, List<TreeNode>> edge2;
							do
							{
								edge2 = tree2.edges.ElementAt(rando.Next(tree2.edges.Count));

							} while (tree2.root == edge2.Key);


							//find the parent of that node
							var parent2 = tree2.FindParentNode(edge2.Key);

							//edit the edge to point to one of the children of chosen node
							for (int k = 0; k < tree2.edges[parent2].Count; k++)
							{
								if (tree2.edges[parent2][k] == edge2.Key)
								{
									tree2.edges[parent2][k] = edge2.Value[0];
									tree2.RemoveNode(edge2.Key);
									break;
								}
							}

							tree2.TraverseData(dataPointMgr.dataPoints[0]);
							newCreations.Add(tree2);
						}

					}
					else if (tester < 0.8)
					{
						//node parameter/value change

						//pick a tree
						Tree tree1 = starter[rando.Next(starter.Count())];
						//make a copy of that tree
						Tree tree2 = tree1.Copy();
						//pick a node in that copy
						var edge2 = tree2.edges.ElementAt(rando.Next(tree2.edges.Count));

						//TODO turn this into a general helper method to create a new random node
						//create a new node with random param and value
						var nodeNew = new TreeNode();
						var test = new TreeTest();
						test.param = rando.Next(dataPointMgr.ranges.Count);
						test.valTest = rando.NextDouble() * (dataPointMgr.ranges[test.param][1] - dataPointMgr.ranges[test.param][0]) + dataPointMgr.ranges[test.param][0];
						test.isLessThanEqualTest = rando.NextDouble() > 0.5;

						nodeNew.Test = test;

						if (tree2.root == edge2.Key)
						{
							tree2.root = nodeNew;
							tree2.edges.Add(nodeNew, new List<TreeNode>(tree2.edges[edge2.Key]));
							tree2.edges.Remove(edge2.Key);
						}
						else
						{
							//find the parent of that node
							var parent2 = tree2.FindParentNode(edge2.Key);

							//edit the edge to point to one of the children of chosen node
							for (int k = 0; k < tree2.edges[parent2].Count; k++)
							{
								//edit the edge for the parent to point to the new node
								//edit the edge for the new node to point to the same children as the chosen node
								if (tree2.edges[parent2][k] == edge2.Key)
								{
									tree2.edges[parent2][k] = nodeNew;
									tree2.edges.Add(nodeNew, new List<TreeNode>(tree2.edges[edge2.Key]));
									tree2.edges.Remove(edge2.Key);
									break;
								}
							}
						}

						tree2.TraverseData(dataPointMgr.dataPoints[0]);
						newCreations.Add(tree2);
					}
					else
					{
						//all random new
						var tree2 = CreateRandomTree();
						tree2.TraverseData(dataPointMgr.dataPoints[0]);
						newCreations.Add(tree2);
					}
				}

				starter.AddRange(newCreations);
				starter = ProcessPoolOfTrees(starter, generationNumber);

				//TODO evaluate the trees somehow to determine the similarity and overlap of them all
			}

			//TODO add a step at the end to verify the results with a hold out data set

			//TODO add the ability to use the best tree for prediction and generate a results file w/ the predictions
		}

		private void btnLoadSecondData_Click(object sender, EventArgs e)
		{
			string path = "data/balance-scale/data.csv";
			LoadDataFile(path);
		}

		private void btnDataBig_Click(object sender, EventArgs e)
		{
			string path = "data/otto/otto.csv";
			LoadDataFile(path);
		}
		void Btn_loadAnyFileClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string path = ofd.FileName;
				
				LoadDataFile(path);
			}
		}
	}

}
