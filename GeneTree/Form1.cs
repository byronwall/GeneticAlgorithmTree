﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

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
			var action = e.Argument as Action;
			if (action != null)
			{
				Debug.WriteLine("background worker is started on {0}", new object[]{ action.Method.Name });
				action();
			}
			else
			{
				throw new Exception("did you intend to have a method call in here somehwere?");
			}
		}

		void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			prog_ongoing.Value = Math.Max(Math.Min(e.ProgressPercentage, prog_ongoing.Maximum), prog_ongoing.Minimum);
			txt_status.Text = last_status.status;
			prop_gaOptions.Refresh();
		}
		
		void UpdateProgressAndStatus(GeneticAlgorithmUpdateStatus data)
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
			
			Debug.WriteLine("background worker is completed doing whatever");
			prog_ongoing.Value = 100;
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
			UpdateProgressAndStatus(e.Data);
		}

		public Form1()
		{
			InitializeComponent();			
			InitBackgroundWorker();
			
			LoadBnpDataLocations();
			
			//set up the property grid
			prop_gaOptions.SelectedObject = ga_mgr._gaOptions;
						
			ga_mgr.ProgressUpdated += ga_mgr_ProgressUpdated;
			
			/* code will load the browser with a d3 viz
			ChromiumWebBrowser browser;
			Cef.Initialize(new CefSettings());
			browser = new ChromiumWebBrowser("http://bl.ocks.org/mbostock/4062045");
			
			tabBrowser.Controls.Add(browser);
			browser.Dock = DockStyle.Fill;
			*/
		}
		
		private void btnPoolRando_Click(object sender, EventArgs e)
		{
			StartBackgroundWorker(new Action(ga_mgr.CreatePoolOfGoodTrees));
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
			var action = new Action(() =>
				{
					ga_mgr.LoadDataFile(txt_dataFile.Text, txt_configFile.Text);
					//ga_mgr.dataPointMgr.OutputCodebooks();
				});
			StartBackgroundWorker(action);
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

		void LoadBnpDataLocations()
		{
			//TODO delete this later
			txt_dataFile.Text = @"C:\projects\bnp-kaggle\train-noid.csv";
			txt_configFile.Text = @"C:\projects\bnp-kaggle\train-noid_config.txt";
		}

		void Button2Click(object sender, EventArgs e)
		{
			txt_dataFile.Text = @"C:\projects\bnp-kaggle\test.csv";
			txt_configFile.Text = @"C:\projects\bnp-kaggle\test_config.txt";
		}
		void Btn_predictAllClick(object sender, EventArgs e)
		{
			if (Directory.Exists(activeFileOrFolder))
			{
				StartBackgroundWorker(new Action(() => ga_mgr.DoAllPredictions(activeFileOrFolder)));
			}
			else
			{
				MessageBox.Show("drop a valid folder for predictions");
			}
		}
		void Btn_predictClick(object sender, EventArgs e)
		{
			if (File.Exists(activeFileOrFolder))
			{
				StartBackgroundWorker(new Action(() => ga_mgr.DoSomePrediction(activeFileOrFolder)));
			}
			else
			{
				MessageBox.Show("drop a valid file");
			}
		}
		
		void StartBackgroundWorker(Action action)
		{
			if (bw.IsBusy)
			{
				Debug.WriteLine("backgroundworker is busy... double click?");
				return;
			}
			
			bw.RunWorkerAsync(action);
		}
		
		string activeFileOrFolder;
		
		void Form1DragDrop(object sender, DragEventArgs e)
		{
			var data = e.Data.GetData(DataFormats.FileDrop) as string[];
			
			if (data != null)
			{
				activeFileOrFolder = data[0];
				statusText.Text = activeFileOrFolder;
			}
		}
		void Form1DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}
	}
}
