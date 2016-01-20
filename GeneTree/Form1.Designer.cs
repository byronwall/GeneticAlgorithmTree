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
        	this.btnLoadData = new System.Windows.Forms.Button();
        	this.btnPoolRando = new System.Windows.Forms.Button();
        	this.btnLoadSecondData = new System.Windows.Forms.Button();
        	this.btnDataBig = new System.Windows.Forms.Button();
        	this.btn_loadAnyFile = new System.Windows.Forms.Button();
        	this.SuspendLayout();
        	// 
        	// btnLoadData
        	// 
        	this.btnLoadData.Location = new System.Drawing.Point(46, 25);
        	this.btnLoadData.Name = "btnLoadData";
        	this.btnLoadData.Size = new System.Drawing.Size(100, 56);
        	this.btnLoadData.TabIndex = 0;
        	this.btnLoadData.Text = "load iris data";
        	this.btnLoadData.UseVisualStyleBackColor = true;
        	this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
        	// 
        	// btnPoolRando
        	// 
        	this.btnPoolRando.Location = new System.Drawing.Point(46, 87);
        	this.btnPoolRando.Name = "btnPoolRando";
        	this.btnPoolRando.Size = new System.Drawing.Size(309, 51);
        	this.btnPoolRando.TabIndex = 1;
        	this.btnPoolRando.Text = "build pool of randoms";
        	this.btnPoolRando.UseVisualStyleBackColor = true;
        	this.btnPoolRando.Click += new System.EventHandler(this.btnPoolRando_Click);
        	// 
        	// btnLoadSecondData
        	// 
        	this.btnLoadSecondData.Location = new System.Drawing.Point(149, 25);
        	this.btnLoadSecondData.Name = "btnLoadSecondData";
        	this.btnLoadSecondData.Size = new System.Drawing.Size(100, 56);
        	this.btnLoadSecondData.TabIndex = 0;
        	this.btnLoadSecondData.Text = "load second data";
        	this.btnLoadSecondData.UseVisualStyleBackColor = true;
        	this.btnLoadSecondData.Click += new System.EventHandler(this.btnLoadSecondData_Click);
        	// 
        	// btnDataBig
        	// 
        	this.btnDataBig.Location = new System.Drawing.Point(255, 25);
        	this.btnDataBig.Name = "btnDataBig";
        	this.btnDataBig.Size = new System.Drawing.Size(100, 56);
        	this.btnDataBig.TabIndex = 0;
        	this.btnDataBig.Text = "load BIG!!data";
        	this.btnDataBig.UseVisualStyleBackColor = true;
        	this.btnDataBig.Click += new System.EventHandler(this.btnDataBig_Click);
        	// 
        	// btn_loadAnyFile
        	// 
        	this.btn_loadAnyFile.Location = new System.Drawing.Point(419, 25);
        	this.btn_loadAnyFile.Name = "btn_loadAnyFile";
        	this.btn_loadAnyFile.Size = new System.Drawing.Size(100, 56);
        	this.btn_loadAnyFile.TabIndex = 0;
        	this.btn_loadAnyFile.Text = "load data file";
        	this.btn_loadAnyFile.UseVisualStyleBackColor = true;
        	this.btn_loadAnyFile.Click += new System.EventHandler(this.Btn_loadAnyFileClick);
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(621, 174);
        	this.Controls.Add(this.btnPoolRando);
        	this.Controls.Add(this.btn_loadAnyFile);
        	this.Controls.Add(this.btnDataBig);
        	this.Controls.Add(this.btnLoadSecondData);
        	this.Controls.Add(this.btnLoadData);
        	this.Name = "Form1";
        	this.Text = "GeneTree";
        	this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Button btnPoolRando;
        private System.Windows.Forms.Button btnLoadSecondData;
        private System.Windows.Forms.Button btnDataBig;
        private System.Windows.Forms.Button btn_loadAnyFile;
    }
}

