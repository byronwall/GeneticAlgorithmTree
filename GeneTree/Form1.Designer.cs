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
        	this.statusStrip1.SuspendLayout();
        	this.menuStrip1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// btnPoolRando
        	// 
        	this.btnPoolRando.Location = new System.Drawing.Point(12, 68);
        	this.btnPoolRando.Name = "btnPoolRando";
        	this.btnPoolRando.Size = new System.Drawing.Size(99, 51);
        	this.btnPoolRando.TabIndex = 1;
        	this.btnPoolRando.Text = "build pool of randoms";
        	this.btnPoolRando.UseVisualStyleBackColor = true;
        	this.btnPoolRando.Click += new System.EventHandler(this.btnPoolRando_Click);
        	// 
        	// txt_dataFile
        	// 
        	this.txt_dataFile.Location = new System.Drawing.Point(70, 135);
        	this.txt_dataFile.Multiline = true;
        	this.txt_dataFile.Name = "txt_dataFile";
        	this.txt_dataFile.Size = new System.Drawing.Size(123, 58);
        	this.txt_dataFile.TabIndex = 2;
        	this.txt_dataFile.Text = "C:\\projects\\gene-tree\\GeneTree\\bin\\Debug\\data\\prudential\\train-no-id.csv";
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(12, 138);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(52, 23);
        	this.label1.TabIndex = 3;
        	this.label1.Text = "data file";
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(12, 202);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(52, 23);
        	this.label2.TabIndex = 5;
        	this.label2.Text = "config file";
        	// 
        	// txt_configFile
        	// 
        	this.txt_configFile.Location = new System.Drawing.Point(70, 199);
        	this.txt_configFile.Multiline = true;
        	this.txt_configFile.Name = "txt_configFile";
        	this.txt_configFile.Size = new System.Drawing.Size(123, 68);
        	this.txt_configFile.TabIndex = 4;
        	this.txt_configFile.Text = "C:\\projects\\gene-tree\\GeneTree\\bin\\Debug\\data\\prudential\\train_config.txt";
        	// 
        	// btn_configDefault
        	// 
        	this.btn_configDefault.Location = new System.Drawing.Point(12, 273);
        	this.btn_configDefault.Name = "btn_configDefault";
        	this.btn_configDefault.Size = new System.Drawing.Size(117, 23);
        	this.btn_configDefault.TabIndex = 6;
        	this.btn_configDefault.Text = "create default config";
        	this.btn_configDefault.UseVisualStyleBackColor = true;
        	this.btn_configDefault.Click += new System.EventHandler(this.Btn_configDefaultClick);
        	// 
        	// btn_loadWithConfig
        	// 
        	this.btn_loadWithConfig.Location = new System.Drawing.Point(12, 302);
        	this.btn_loadWithConfig.Name = "btn_loadWithConfig";
        	this.btn_loadWithConfig.Size = new System.Drawing.Size(117, 23);
        	this.btn_loadWithConfig.TabIndex = 7;
        	this.btn_loadWithConfig.Text = "load based on config";
        	this.btn_loadWithConfig.UseVisualStyleBackColor = true;
        	this.btn_loadWithConfig.Click += new System.EventHandler(this.Btn_loadWithConfigClick);
        	// 
        	// button1
        	// 
        	this.button1.Location = new System.Drawing.Point(12, 39);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(75, 23);
        	this.button1.TabIndex = 8;
        	this.button1.Text = "button1";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// statusStrip1
        	// 
        	this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.prog_ongoing});
        	this.statusStrip1.Location = new System.Drawing.Point(0, 458);
        	this.statusStrip1.Name = "statusStrip1";
        	this.statusStrip1.Size = new System.Drawing.Size(1081, 22);
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
        	this.menuStrip1.Size = new System.Drawing.Size(1081, 24);
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
        	this.txt_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.txt_status.Location = new System.Drawing.Point(199, 39);
        	this.txt_status.Multiline = true;
        	this.txt_status.Name = "txt_status";
        	this.txt_status.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        	this.txt_status.Size = new System.Drawing.Size(625, 416);
        	this.txt_status.TabIndex = 11;
        	this.txt_status.WordWrap = false;
        	// 
        	// prop_gaOptions
        	// 
        	this.prop_gaOptions.Location = new System.Drawing.Point(830, 39);
        	this.prop_gaOptions.Name = "prop_gaOptions";
        	this.prop_gaOptions.Size = new System.Drawing.Size(239, 416);
        	this.prop_gaOptions.TabIndex = 12;
        	// 
        	// btn_dataSummary
        	// 
        	this.btn_dataSummary.Location = new System.Drawing.Point(13, 352);
        	this.btn_dataSummary.Name = "btn_dataSummary";
        	this.btn_dataSummary.Size = new System.Drawing.Size(116, 23);
        	this.btn_dataSummary.TabIndex = 13;
        	this.btn_dataSummary.Text = "output summary";
        	this.btn_dataSummary.UseVisualStyleBackColor = true;
        	this.btn_dataSummary.Click += new System.EventHandler(this.Btn_dataSummaryClick);
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(1081, 480);
        	this.Controls.Add(this.btn_dataSummary);
        	this.Controls.Add(this.prop_gaOptions);
        	this.Controls.Add(this.txt_status);
        	this.Controls.Add(this.statusStrip1);
        	this.Controls.Add(this.menuStrip1);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.btn_loadWithConfig);
        	this.Controls.Add(this.btn_configDefault);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.txt_configFile);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.txt_dataFile);
        	this.Controls.Add(this.btnPoolRando);
        	this.MainMenuStrip = this.menuStrip1;
        	this.Name = "Form1";
        	this.Text = "GeneTree";
        	this.statusStrip1.ResumeLayout(false);
        	this.statusStrip1.PerformLayout();
        	this.menuStrip1.ResumeLayout(false);
        	this.menuStrip1.PerformLayout();
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
    }
}

