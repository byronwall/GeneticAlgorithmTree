namespace GeneTree
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        	this.btnPoolRando = new System.Windows.Forms.Button();
        	this.txt_dataFile = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.label2 = new System.Windows.Forms.Label();
        	this.txt_configFile = new System.Windows.Forms.TextBox();
        	this.btn_configDefault = new System.Windows.Forms.Button();
        	this.btn_loadWithConfig = new System.Windows.Forms.Button();
        	this.button1 = new System.Windows.Forms.Button();
        	this.statusStrip1 = new System.Windows.Forms.StatusStrip();
        	this.prog_ongoing = new System.Windows.Forms.ToolStripProgressBar();
        	this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        	this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.txt_status = new System.Windows.Forms.TextBox();
        	this.prop_gaOptions = new System.Windows.Forms.PropertyGrid();
        	this.btn_dataSummary = new System.Windows.Forms.Button();
        	this.button2 = new System.Windows.Forms.Button();
        	this.btn_predictAll = new System.Windows.Forms.Button();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.tabControl1 = new System.Windows.Forms.TabControl();
        	this.tabStatus = new System.Windows.Forms.TabPage();
        	this.tabBrowser = new System.Windows.Forms.TabPage();
        	this.statusStrip1.SuspendLayout();
        	this.menuStrip1.SuspendLayout();
        	this.panel1.SuspendLayout();
        	this.tabControl1.SuspendLayout();
        	this.tabStatus.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// btnPoolRando
        	// 
        	this.btnPoolRando.Location = new System.Drawing.Point(12, 45);
        	this.btnPoolRando.Name = "btnPoolRando";
        	this.btnPoolRando.Size = new System.Drawing.Size(123, 26);
        	this.btnPoolRando.TabIndex = 1;
        	this.btnPoolRando.Text = "build pool of randoms";
        	this.btnPoolRando.UseVisualStyleBackColor = true;
        	this.btnPoolRando.Click += new System.EventHandler(this.btnPoolRando_Click);
        	// 
        	// txt_dataFile
        	// 
        	this.txt_dataFile.Location = new System.Drawing.Point(12, 100);
        	this.txt_dataFile.Multiline = true;
        	this.txt_dataFile.Name = "txt_dataFile";
        	this.txt_dataFile.Size = new System.Drawing.Size(123, 58);
        	this.txt_dataFile.TabIndex = 2;
        	this.txt_dataFile.Text = "C:\\projects\\gene-tree\\GeneTree\\bin\\Debug\\data\\prudential\\train-no-id.csv";
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(11, 74);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(52, 23);
        	this.label1.TabIndex = 3;
        	this.label1.Text = "data file";
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(11, 162);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(52, 23);
        	this.label2.TabIndex = 5;
        	this.label2.Text = "config file";
        	// 
        	// txt_configFile
        	// 
        	this.txt_configFile.Location = new System.Drawing.Point(12, 188);
        	this.txt_configFile.Multiline = true;
        	this.txt_configFile.Name = "txt_configFile";
        	this.txt_configFile.Size = new System.Drawing.Size(123, 68);
        	this.txt_configFile.TabIndex = 4;
        	this.txt_configFile.Text = "C:\\projects\\gene-tree\\GeneTree\\bin\\Debug\\data\\prudential\\train_config.txt";
        	// 
        	// btn_configDefault
        	// 
        	this.btn_configDefault.Location = new System.Drawing.Point(12, 262);
        	this.btn_configDefault.Name = "btn_configDefault";
        	this.btn_configDefault.Size = new System.Drawing.Size(117, 23);
        	this.btn_configDefault.TabIndex = 6;
        	this.btn_configDefault.Text = "create default config";
        	this.btn_configDefault.UseVisualStyleBackColor = true;
        	this.btn_configDefault.Click += new System.EventHandler(this.Btn_configDefaultClick);
        	// 
        	// btn_loadWithConfig
        	// 
        	this.btn_loadWithConfig.Location = new System.Drawing.Point(12, 291);
        	this.btn_loadWithConfig.Name = "btn_loadWithConfig";
        	this.btn_loadWithConfig.Size = new System.Drawing.Size(117, 23);
        	this.btn_loadWithConfig.TabIndex = 7;
        	this.btn_loadWithConfig.Text = "load based on config";
        	this.btn_loadWithConfig.UseVisualStyleBackColor = true;
        	this.btn_loadWithConfig.Click += new System.EventHandler(this.Btn_loadWithConfigClick);
        	// 
        	// button1
        	// 
        	this.button1.Location = new System.Drawing.Point(12, 16);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(60, 23);
        	this.button1.TabIndex = 8;
        	this.button1.Text = "iris data";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// statusStrip1
        	// 
        	this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.prog_ongoing});
        	this.statusStrip1.Location = new System.Drawing.Point(0, 409);
        	this.statusStrip1.Name = "statusStrip1";
        	this.statusStrip1.Size = new System.Drawing.Size(830, 22);
        	this.statusStrip1.TabIndex = 9;
        	this.statusStrip1.Text = "statusStrip1";
        	// 
        	// prog_ongoing
        	// 
        	this.prog_ongoing.Name = "prog_ongoing";
        	this.prog_ongoing.Size = new System.Drawing.Size(200, 16);
        	// 
        	// menuStrip1
        	// 
        	this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.toolsToolStripMenuItem,
			this.helpToolStripMenuItem});
        	this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        	this.menuStrip1.Name = "menuStrip1";
        	this.menuStrip1.Size = new System.Drawing.Size(830, 24);
        	this.menuStrip1.TabIndex = 10;
        	this.menuStrip1.Text = "menuStrip1";
        	// 
        	// fileToolStripMenuItem
        	// 
        	this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.exitToolStripMenuItem});
        	this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        	this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
        	this.fileToolStripMenuItem.Text = "&File";
        	// 
        	// exitToolStripMenuItem
        	// 
        	this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        	this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
        	this.exitToolStripMenuItem.Text = "E&xit";
        	this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
        	// 
        	// toolsToolStripMenuItem
        	// 
        	this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.optionsToolStripMenuItem});
        	this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        	this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
        	this.toolsToolStripMenuItem.Text = "&Tools";
        	// 
        	// optionsToolStripMenuItem
        	// 
        	this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
        	this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
        	this.optionsToolStripMenuItem.Text = "&Options";
        	// 
        	// helpToolStripMenuItem
        	// 
        	this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.aboutToolStripMenuItem});
        	this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        	this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
        	this.helpToolStripMenuItem.Text = "&Help";
        	// 
        	// aboutToolStripMenuItem
        	// 
        	this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        	this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
        	this.aboutToolStripMenuItem.Text = "&About...";
        	// 
        	// txt_status
        	// 
        	this.txt_status.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.txt_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.txt_status.Location = new System.Drawing.Point(3, 3);
        	this.txt_status.Multiline = true;
        	this.txt_status.Name = "txt_status";
        	this.txt_status.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        	this.txt_status.Size = new System.Drawing.Size(401, 353);
        	this.txt_status.TabIndex = 11;
        	this.txt_status.WordWrap = false;
        	// 
        	// prop_gaOptions
        	// 
        	this.prop_gaOptions.Dock = System.Windows.Forms.DockStyle.Right;
        	this.prop_gaOptions.Location = new System.Drawing.Point(560, 24);
        	this.prop_gaOptions.Name = "prop_gaOptions";
        	this.prop_gaOptions.Size = new System.Drawing.Size(270, 385);
        	this.prop_gaOptions.TabIndex = 12;
        	// 
        	// btn_dataSummary
        	// 
        	this.btn_dataSummary.Location = new System.Drawing.Point(12, 319);
        	this.btn_dataSummary.Name = "btn_dataSummary";
        	this.btn_dataSummary.Size = new System.Drawing.Size(116, 23);
        	this.btn_dataSummary.TabIndex = 13;
        	this.btn_dataSummary.Text = "output summary";
        	this.btn_dataSummary.UseVisualStyleBackColor = true;
        	this.btn_dataSummary.Click += new System.EventHandler(this.Btn_dataSummaryClick);
        	// 
        	// button2
        	// 
        	this.button2.Location = new System.Drawing.Point(78, 16);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(57, 23);
        	this.button2.TabIndex = 8;
        	this.button2.Text = "bnp data";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// btn_predictAll
        	// 
        	this.btn_predictAll.Location = new System.Drawing.Point(12, 348);
        	this.btn_predictAll.Name = "btn_predictAll";
        	this.btn_predictAll.Size = new System.Drawing.Size(116, 23);
        	this.btn_predictAll.TabIndex = 14;
        	this.btn_predictAll.Text = "predict all trees";
        	this.btn_predictAll.UseVisualStyleBackColor = true;
        	this.btn_predictAll.Click += new System.EventHandler(this.Btn_predictAllClick);
        	// 
        	// panel1
        	// 
        	this.panel1.Controls.Add(this.button1);
        	this.panel1.Controls.Add(this.btn_predictAll);
        	this.panel1.Controls.Add(this.button2);
        	this.panel1.Controls.Add(this.btnPoolRando);
        	this.panel1.Controls.Add(this.btn_dataSummary);
        	this.panel1.Controls.Add(this.txt_dataFile);
        	this.panel1.Controls.Add(this.label1);
        	this.panel1.Controls.Add(this.txt_configFile);
        	this.panel1.Controls.Add(this.label2);
        	this.panel1.Controls.Add(this.btn_configDefault);
        	this.panel1.Controls.Add(this.btn_loadWithConfig);
        	this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
        	this.panel1.Location = new System.Drawing.Point(0, 24);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(145, 385);
        	this.panel1.TabIndex = 15;
        	// 
        	// tabControl1
        	// 
        	this.tabControl1.Controls.Add(this.tabStatus);
        	this.tabControl1.Controls.Add(this.tabBrowser);
        	this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.tabControl1.Location = new System.Drawing.Point(145, 24);
        	this.tabControl1.Name = "tabControl1";
        	this.tabControl1.SelectedIndex = 0;
        	this.tabControl1.Size = new System.Drawing.Size(415, 385);
        	this.tabControl1.TabIndex = 16;
        	// 
        	// tabStatus
        	// 
        	this.tabStatus.Controls.Add(this.txt_status);
        	this.tabStatus.Location = new System.Drawing.Point(4, 22);
        	this.tabStatus.Name = "tabStatus";
        	this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
        	this.tabStatus.Size = new System.Drawing.Size(407, 359);
        	this.tabStatus.TabIndex = 0;
        	this.tabStatus.Text = "status";
        	this.tabStatus.UseVisualStyleBackColor = true;
        	// 
        	// tabBrowser
        	// 
        	this.tabBrowser.Location = new System.Drawing.Point(4, 22);
        	this.tabBrowser.Name = "tabBrowser";
        	this.tabBrowser.Padding = new System.Windows.Forms.Padding(3);
        	this.tabBrowser.Size = new System.Drawing.Size(407, 359);
        	this.tabBrowser.TabIndex = 1;
        	this.tabBrowser.Text = "browser";
        	this.tabBrowser.UseVisualStyleBackColor = true;
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(830, 431);
        	this.Controls.Add(this.tabControl1);
        	this.Controls.Add(this.panel1);
        	this.Controls.Add(this.prop_gaOptions);
        	this.Controls.Add(this.statusStrip1);
        	this.Controls.Add(this.menuStrip1);
        	this.MainMenuStrip = this.menuStrip1;
        	this.Name = "Form1";
        	this.Text = "GeneTree";
        	this.Load += new System.EventHandler(this.Form1Load);
        	this.statusStrip1.ResumeLayout(false);
        	this.statusStrip1.PerformLayout();
        	this.menuStrip1.ResumeLayout(false);
        	this.menuStrip1.PerformLayout();
        	this.panel1.ResumeLayout(false);
        	this.panel1.PerformLayout();
        	this.tabControl1.ResumeLayout(false);
        	this.tabStatus.ResumeLayout(false);
        	this.tabStatus.PerformLayout();
        	this.ResumeLayout(false);
        	this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPoolRando;
        private System.Windows.Forms.TextBox txt_dataFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_configFile;
        private System.Windows.Forms.Button btn_configDefault;
        private System.Windows.Forms.Button btn_loadWithConfig;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar prog_ongoing;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TextBox txt_status;
        private System.Windows.Forms.PropertyGrid prop_gaOptions;
        private System.Windows.Forms.Button btn_dataSummary;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btn_predictAll;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStatus;
        private System.Windows.Forms.TabPage tabBrowser;
    }
}

