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
		GeneticAlgorithmManager ga_mgr = new GeneticAlgorithmManager();
		
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
			string path = "data/iris/iris.data";
			ga_mgr.LoadDataFile(path);
		}		

		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			ga_mgr.ProcessTheNextGeneration();

			MessageBox.Show("the test is completed");
		}		

		private void btnLoadSecondData_Click(object sender, EventArgs e)
		{
			string path = "data/balance-scale/data.csv";
			ga_mgr.LoadDataFile(path);
		}

		private void btnDataBig_Click(object sender, EventArgs e)
		{
			string path = "data/otto/otto.csv";
			ga_mgr.LoadDataFile(path);
		}
		void Btn_loadAnyFileClick(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				string path = ofd.FileName;
				
				ga_mgr.LoadDataFile(path);
			}
		}
	}
}
