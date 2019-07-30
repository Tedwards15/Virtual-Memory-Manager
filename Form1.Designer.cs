namespace vm_cs_fresh
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.operationProgress = new System.Windows.Forms.ProgressBar();
            this.hitFaultRatios = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.thingToView = new System.Windows.Forms.TextBox();
            this.addressesOpener = new System.Windows.Forms.OpenFileDialog();
            this.viewOpener = new System.Windows.Forms.OpenFileDialog();
            this.answerSaver = new System.Windows.Forms.SaveFileDialog();
            this.backingStoreOpener = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.operationProgress);
            this.splitContainer1.Panel1.Controls.Add(this.hitFaultRatios);
            this.splitContainer1.Panel1.Controls.Add(this.btnSave);
            this.splitContainer1.Panel1.Controls.Add(this.btnProcess);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.thingToView);
            this.splitContainer1.Size = new System.Drawing.Size(584, 449);
            this.splitContainer1.SplitterDistance = 113;
            this.splitContainer1.TabIndex = 1;
            // 
            // operationProgress
            // 
            this.operationProgress.Location = new System.Drawing.Point(6, 67);
            this.operationProgress.Name = "operationProgress";
            this.operationProgress.Size = new System.Drawing.Size(578, 23);
            this.operationProgress.TabIndex = 40;
            // 
            // hitFaultRatios
            // 
            this.hitFaultRatios.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hitFaultRatios.AutoSize = true;
            this.hitFaultRatios.Location = new System.Drawing.Point(176, 51);
            this.hitFaultRatios.Name = "hitFaultRatios";
            this.hitFaultRatios.Size = new System.Drawing.Size(180, 13);
            this.hitFaultRatios.TabIndex = 39;
            this.hitFaultRatios.Text = "Statistics: (Addresses not processed)";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 31;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(93, 12);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 33;
            this.btnProcess.Text = "Process Results";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // thingToView
            // 
            this.thingToView.AcceptsReturn = true;
            this.thingToView.AcceptsTab = true;
            this.thingToView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thingToView.Location = new System.Drawing.Point(0, 0);
            this.thingToView.Multiline = true;
            this.thingToView.Name = "thingToView";
            this.thingToView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.thingToView.Size = new System.Drawing.Size(584, 332);
            this.thingToView.TabIndex = 23;
            // 
            // addressesOpener
            // 
            this.addressesOpener.Filter = "Text Files|*.txt|All|*.*";
            // 
            // viewOpener
            // 
            this.viewOpener.Filter = "Text Files|*.txt|All|*.*";
            // 
            // answerSaver
            // 
            this.answerSaver.Filter = "Text Files|.*txt|All|*.*";
            // 
            // backingStoreOpener
            // 
            this.backingStoreOpener.Filter = "BIN Files|*.bin|All|*.*";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 449);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label hitFaultRatios;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.TextBox thingToView;
        private System.Windows.Forms.OpenFileDialog addressesOpener;
        private System.Windows.Forms.OpenFileDialog viewOpener;
        private System.Windows.Forms.SaveFileDialog answerSaver;
        private System.Windows.Forms.OpenFileDialog backingStoreOpener;
        private System.Windows.Forms.ProgressBar operationProgress;
    }
}

