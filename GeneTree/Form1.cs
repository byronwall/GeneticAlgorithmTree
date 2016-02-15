using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			ga_mgr.CreatePoolOfGoodTrees();
		}

		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			prog_ongoing.Value = e.ProgressPercentage;
		}
		
		void bw_UpdateProgress(GeneticAlgorithmUpdate data)
		{
			if (bw != null && bw.WorkerReportsProgress)
			{
				bw.ReportProgress(data.progress);
			}
			Debug.WriteLine(data.status);
		}

		void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				throw e.Error;
			}
		}
		void InitBackgroundWorker()
		{
			bw = new BackgroundWorker();
			bw.WorkerReportsProgress = true;
			bw.DoWork += bw_DoWork;
			bw.ProgressChanged += bw_ProgressChanged;
			bw.RunWorkerCompleted += bw_RunWorkerCompleted;
		}

		void ga_mgr_ProgressUpdated(object sender, EventArg<GeneticAlgorithmUpdate> e)
		{
			bw_UpdateProgress(e.Data);
		}

		public Form1()
		{
			InitializeComponent();			
			InitBackgroundWorker();
						
			ga_mgr.ProgressUpdated += ga_mgr_ProgressUpdated;
		}
		
		BackgroundWorker bw = new BackgroundWorker();

		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			btnPoolRando.Enabled = false;
			bw.RunWorkerAsync();
			
			btnPoolRando.Enabled = true;
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
			//TODO find a better way to prevent double click
			btn_loadWithConfig.Enabled = false;
			
			ga_mgr.LoadDataFile(txt_dataFile.Text, txt_configFile.Text);
		}
		void Button1Click(object sender, EventArgs e)
		{
			//TODO delete this later
			txt_dataFile.Text = @"C:\projects\gene-tree\GeneTree\bin\Debug\data\iris\iris.data";
			txt_configFile.Text = @"C:\projects\gene-tree\GeneTree\bin\Debug\data\iris\iris_config.txt";
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
	}
}
