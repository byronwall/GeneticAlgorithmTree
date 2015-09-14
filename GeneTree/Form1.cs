using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            //ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            //Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;
        }

        List<DataPoint> dataPoints = new List<DataPoint>();

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            //open the file
            var reader = new StreamReader(new FileStream("data/iris/iris.data", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

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
                var data = values.Take(values.Length - 1).Select(x => double.Parse(x)).ToArray();
                dataPoints.Add(new DataPoint() { Data = data, Classification = values[values.Length - 1] });
            }
        }

        Random rando = new Random();
        string[] classes;
        Dictionary<int, List<double>> ranges = new Dictionary<int, List<double>>();

        private void btnRandomTree_Click(object sender, EventArgs e)
        {
            CreateRandomTreeAndPredict();
        }

        private Tree CreateRandomTreeAndPredict()
        {
            //create classes and ranges
            classes = dataPoints.GroupBy(x => x.Classification).Select(x => x.Key).ToArray();

            //get min/max ranges for the data
            int paramCount = dataPoints[0].Data.Length;
            ranges.Clear();
            for (int i = 0; i < paramCount; i++)
            {
                double max = dataPoints.Max(x => x.Data[i]);
                double min = dataPoints.Min(x => x.Data[i]);

                ranges.Add(i, new List<double> { min, max });
            }

            //build a random tree
            var tree = new Tree();
            var root = new TreeNode();
            var test = new TreeTest();
            test.param = rando.Next(paramCount);
            test.valTest = rando.NextDouble() * (ranges[test.param][1] - ranges[test.param][0]) + ranges[test.param][0];
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

                    if (nodeYes.IsTerminal)
                    {
                        //class
                        nodeYes.Classification = classes[rando.Next(classes.Length)];
                    }
                    else
                    {
                        var testYes = new TreeTest();
                        testYes.param = rando.Next(paramCount);
                        testYes.valTest = rando.NextDouble() * (ranges[testYes.param][1] - ranges[testYes.param][0]) + ranges[testYes.param][0];
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
        }

        private List<Tree> CreateRandomPoolOfTrees(int size)
        {
            //create a number of random trees and report results from all of them
            List<Tuple<double, Tree>> results = new List<Tuple<double, Tree>>();

            for (int i = 0; i < size; i++)
            {
                var tree = CreateRandomTreeAndPredict();

                int correct = 0;
                foreach (var item in dataPoints)
                {
                    if (tree.TraverseData(item))
                    {
                        correct++;
                    }
                }

                double ratio = 1.0 * correct / dataPoints.Count;

                //report accuracy
                //Trace.WriteLine(String.Format("{0} of {1} = {2}", correct, dataPoints.Count, ratio));

                results.Add(new Tuple<double, Tree>(Math.Pow(ratio, 2) / tree.edges.Count, tree));
            }

            var temp = results.Select(x => x.Item1).OrderByDescending(x => x).Take(size / 2);

            //Trace.Write(String.Join("\r\n", temp));

            //TODO: remove this selection step
            return results.OrderByDescending(x => x.Item1).Select(x => x.Item2).Take(10).ToList();
        }

        private List<Tree> ProcessPoolOfTrees(IEnumerable<Tree> trees)
        {
            //create a number of random trees and report results from all of them
            List<Tuple<double, Tree>> results = new List<Tuple<double, Tree>>();

            foreach (var tree in trees)
            {
                int correct = 0;
                foreach (var item in dataPoints)
                {
                    if (tree.TraverseData(item))
                    {
                        correct++;
                    }
                }

                double ratio = 1.0 * correct / dataPoints.Count;

                //report accuracy
                //Trace.WriteLine(String.Format("{0} of {1} = {2}", correct, dataPoints.Count, ratio));

                results.Add(new Tuple<double, Tree>(Math.Pow(ratio,4) / Math.Sqrt(tree.edges.Count), tree));
            }

            var temp = results.Select(x => x.Item1).OrderByDescending(x => x).Take(trees.Count() / 2);

            Trace.WriteLine(String.Join("\r\n", temp));

            Trace.WriteLine(results.OrderByDescending(x => x.Item1).Select(x => x.Item2).First());

            //TODO: remove this selection step
            return results.OrderByDescending(x => x.Item1).Select(x => x.Item2).Take(10).ToList();
        }

        private void ProcessTheNextGeneration()
        {
            int populationSize = 50;

            //start with a list of trees
            var starter = CreateRandomPoolOfTrees(populationSize);

            //do the gene operations
            int generations = 50;

            List<Tree> newCreations = new List<Tree>();

            for (int i = 0; i < generations; i++)
            {
                Trace.WriteLine(i);
                for (int j = 0; j < populationSize; j++)
                {
                    //make a new one!
                    var tester = rando.NextDouble();

                    //TODO: write this code for real

                    if (tester < 0.25)
                    {
                        //node swap

                        //pick a tree
                        //pick a node in that tree
                        Tree tree1 = starter[rando.Next(starter.Count())];

                        //pick another tree
                        //pick a node in that tree
                        Tree tree2 = starter[rando.Next(starter.Count())];
                        var edge2 = tree2.edges.ElementAt(rando.Next(tree2.edges.Count));

                        //TODO: clean up this trap for a node that already exists in the tree
                        if (tree1.edges.ContainsKey(edge2.Key))
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
                                tree3.edges.Remove(edge1.Key);
                                tree3.edges.Add(edge2.Key, edge2.Value);
                            }
                            else
                            {
                                //find the parent of node 1 in the new tree
                                var parent1 = tree3.FindParentNode(edge1.Key);


                                //change the edge for the parent to point to node 2 instead of node 1
                                for (int k = 0; k < tree3.edges[parent1].Count; k++)
                                {
                                    var item = tree3.edges[parent1][k];
                                    if (item == edge1.Key)
                                    {
                                        //swap the parent to the new node
                                        //bring the new node's edge into this tree
                                        //remove the old node from the edges
                                        tree3.edges[parent1][k] = edge2.Key;
                                        tree3.edges.Remove(edge1.Key);
                                        tree3.edges.Add(edge2.Key, edge2.Value);

                                        break;
                                    }
                                }
                            }

                            newCreations.Add(tree2);
                        }

                        //TODO: consider a check to prevent the same node from being added more than once

                    }
                    else if (tester < 0.35)
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
                                    tree2.edges.Remove(edge2.Key);
                                    break;
                                }
                            }


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

                        //create a new node with random param and value
                        var nodeNew = new TreeNode();
                        var test = new TreeTest();
                        test.param = rando.Next(ranges.Count);
                        test.valTest = rando.NextDouble() * (ranges[test.param][1] - ranges[test.param][0]) + ranges[test.param][0];
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

                        newCreations.Add(tree2);

                    }
                    else
                    {
                        //all random new
                        var tree2 = CreateRandomTreeAndPredict();

                        newCreations.Add(tree2);

                        //make a random tree and add it to the gene pool
                    }
                }

                starter.AddRange(newCreations);

                starter = ProcessPoolOfTrees(starter);
            }

            //we need to generate X new items

            //for each new item, pick between all the alternatives


            //combine all those new trees and process



            //rinse, repeat

        }
    }
    public class DataPoint
    {
        public double[] Data;
        public string Classification;
    }

    public class TreeTest
    {
        public int param; //data array index to test
        public double valTest; // value to test, low
        public bool isLessThanEqualTest; //returns true if

        public bool isTrueTest(DataPoint point)
        {
            if (isLessThanEqualTest)
            {
                return point.Data[param] <= valTest;
            }
            else
            {
                return point.Data[param] > valTest;
            }
        }

        public override string ToString()
        {
            return string.Format("param {0}, LTE test {1}, val {2}", param, isLessThanEqualTest, valTest);
        }
    }

    public class Tree
    {
        public TreeNode root;
        public Dictionary<TreeNode, List<TreeNode>> edges = new Dictionary<TreeNode, List<TreeNode>>();

        public Tree Copy()
        {
            Tree copy = new Tree();
            copy.root = this.root;
            copy.edges = new Dictionary<TreeNode, List<TreeNode>>(this.edges);

            foreach (var item in this.edges)
            {
                copy.edges[item.Key] = new List<TreeNode>(item.Value);
            }

            return copy;
        }

        public bool TraverseData(DataPoint point)
        {
            return TraverseData(root, point);
        }

        public TreeNode FindParentNode(TreeNode child)
        {
            return FindParentNode(child, root);
        }
        public TreeNode FindParentNode(TreeNode child, TreeNode possibleParent)
        {
            //stop if child is in list of edges
            if (possibleParent.IsTerminal)
            {
                return null;
            }

            if (edges[possibleParent].Contains(child))
            {
                return possibleParent;
            }
            else
            {
                foreach (var nextPossibleParent in edges[possibleParent])
                {
                    var temp = FindParentNode(child, nextPossibleParent);

                    if (temp != null)
                    {
                        return temp;
                    }
                }
            }

            return null;

            //else proces the edges
        }

        public bool TraverseData(TreeNode node, DataPoint point)
        {
            //start at root, test if correct
            if (node.IsTerminal)
            {
                return node.Classification == point.Classification;
            }
            else
            {
                //do the test and then traverse
                if (node.Test.isTrueTest(point))
                {
                    //0 will be yes
                    return TraverseData(edges[node][0], point);
                }
                else
                {
                    //1 will be no
                    return TraverseData(edges[node][1], point);
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
                    edges[node].ForEach(x => nodes.Push(x));
                }
            }

            return sb.ToString();
        }
    }

    public class TreeNode
    {
        public TreeTest Test;
        public bool IsTerminal;
        public string Classification;

        public override string ToString()
        {
            if (IsTerminal)
            {
                return string.Format("terminal to {0}", Classification.ToString());
            }
            else
            {
                return Test.ToString();
            }

        }
    }
}
