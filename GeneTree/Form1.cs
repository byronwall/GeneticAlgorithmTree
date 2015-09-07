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

            //build a simple tree structure

            //build a simple "testing" structure

            //combine the two to make a random tree
        }

        List<DataPoint> dataPoints = new List<DataPoint>();

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            //open the file

            var reader = new StreamReader(new FileStream("data/iris/iris.data", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (line == string.Empty)
                {
                    continue;
                }

                var values = line.Split(',');

                var data = values.Take(values.Length - 1).Select(x => double.Parse(x)).ToArray();

                dataPoints.Add(new DataPoint() { Data = data, Classification = values[values.Length - 1] });
            }
        }

        Random rando = new Random();
        string[] classes;
        Dictionary<int, List<double>> ranges = new Dictionary<int, List<double>>();

        private void btnMakePredictions_Click(object sender, EventArgs e)
        {
            //get a list of all possible groups

            classes = dataPoints.GroupBy(x => x.Classification).Select(x => x.Key).ToArray();

            //randomly select a group
            int correct = 0;

            foreach (var item in dataPoints)
            {

                var intTest = rando.Next(classes.Length);
                var classTest = classes[intTest];

                if (item.Classification == classTest)
                {
                    correct++;
                }
            }

            //determine accuracy
            lblRandoRight.Text = String.Format("{0} of {1} = {2}", correct, dataPoints.Count, 1.0 * correct / dataPoints.Count);

        }

        private void btnRandomTree_Click(object sender, EventArgs e)
        {
            //build a random tree

            var tree = new Tree();

            //create classes and ranges
            classes = dataPoints.GroupBy(x => x.Classification).Select(x => x.Key).ToArray();

            int paramCount = dataPoints[0].Data.Length;

            ranges.Clear();

            for (int i = 0; i < paramCount; i++)
            {
                double max = dataPoints.Max(x => x.Data[i]);
                double min = dataPoints.Min(x => x.Data[i]);

                ranges.Add(i, new List<double> { min, max });
            }

            //need a param and min/max
            var root = new TreeNode();

            var test = new TreeTest();
            test.param = rando.Next(paramCount);
            test.valTest = rando.NextDouble() * (ranges[test.param][1] - ranges[test.param][0]) + ranges[test.param][0];
            test.isLessThanEqualTest = rando.NextDouble() > 0.5;

            root.Test = test;
            tree.root = root;

            Queue<TreeNode> nonTermNodes = new Queue<TreeNode>();
            nonTermNodes.Enqueue(root);

            while (nonTermNodes.Count > 0)
            {
                var node = nonTermNodes.Dequeue();

                //need to create two new nodes, yes and no

                //decided if terminal... if yes, give a group
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

            //determine accuracy
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
        public Dictionary<TreeNode, List<TreeNode>> edges = new Dictionary<TreeNode,List<TreeNode>>();
        public List<TreeNode> nodes = new List<TreeNode>();

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
