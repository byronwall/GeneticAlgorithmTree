using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            Console.WriteLine(tree.ToString());

            //run the data through and report predictions
            int correct = 0;
            foreach (var item in dataPoints)
            {
                if (tree.TraverseData(item))
                {
                    correct++;
                }
            }

            //report accuracy
            Console.WriteLine("{0} of {1} = {2}", correct, dataPoints.Count, 1.0 * correct / dataPoints.Count);
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

        public bool TraverseData(DataPoint point)
        {
            return TraverseData(root, point);
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
