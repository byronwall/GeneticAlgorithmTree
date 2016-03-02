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
		//HACK this is being passed around to get around Thread restrictions
		GeneticAlgorithmUpdateStatus last_status = new GeneticAlgorithmUpdateStatus();
		DataPointConfiguration config;
		BackgroundWorker bw = new BackgroundWorker();

		void bw_DoWork(object sender, DoWorkEventArgs e)
		{
			ga_mgr.CreatePoolOfGoodTrees();
		}

		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			prog_ongoing.Value = Math.Max(Math.Min(e.ProgressPercentage, prog_ongoing.Maximum), prog_ongoing.Minimum);
			txt_status.Text = last_status.status;
			prop_gaOptions.Refresh();
		}
		
		void bw_UpdateProgress(GeneticAlgorithmUpdateStatus data)
		{
			last_status = data;
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
				Debug.Write(e.Error.ToString());
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

		void ga_mgr_ProgressUpdated(object sender, EventArg<GeneticAlgorithmUpdateStatus> e)
		{
			bw_UpdateProgress(e.Data);
		}

		public Form1()
		{
			InitializeComponent();			
			InitBackgroundWorker();
			
			//set up the property grid
			prop_gaOptions.SelectedObject = ga_mgr._gaOptions;
						
			ga_mgr.ProgressUpdated += ga_mgr_ProgressUpdated;
		}
		
		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			if (bw.IsBusy)
			{
				return;
			}
			
			btnPoolRando.Enabled = false;
			bw.RunWorkerAsync();
			
			btnPoolRando.Enabled = true;
		}
		
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
		void Btn_dataSummaryClick(object sender, EventArgs e)
		{
			Debug.Print(ga_mgr.dataPointMgr.GetSummaryOfClasses());
		}
		void Button2Click(object sender, EventArgs e)
		{
			//TODO delete this later
			txt_dataFile.Text = @"C:\projects\bnp-kaggle\train-noid.csv";
			txt_configFile.Text = @"C:\projects\bnp-kaggle\train-noid_config.txt";
		}
	}
}
