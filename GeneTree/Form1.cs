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

		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			ga_mgr.ProcessTheNextGeneration();

			MessageBox.Show("the test is completed");
		}
		
		DataPointConfiguration config;
		
		void Btn_configDefaultClick(object sender, EventArgs e)
		{
			//create default file based on data file
			string data_path = txt_dataFile.Text;
			
			//HACK this is just to allow copy/paste straight from Explorer
			data_path = data_path.Replace("\"", string.Empty);
			
			config = DataPointConfiguration.CreateDefaultFromFile(data_path);			
			config.SaveToFile(Path.GetDirectoryName(data_path) + @"\" + Path.GetFileNameWithoutExtension(data_path) + "_config.txt");
		}
		
		void Btn_loadWithConfigClick(object sender, EventArgs e)
		{
			ga_mgr.LoadDataFile(txt_dataFile.Text, txt_configFile.Text);
		}
	}
}
