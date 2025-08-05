namespace EMMC_Tool
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblEmmcStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblConnection = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnInitEmmc = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.btnDump = new System.Windows.Forms.Button();
            this.btnWriteBlock = new System.Windows.Forms.Button();
            this.btnReadBlock = new System.Windows.Forms.Button();
            this.txtBlockAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.hexViewer = new System.Windows.Forms.TextBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblEmmcStatus);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lblConnection);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 85);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblEmmcStatus
            // 
            this.lblEmmcStatus.AutoSize = true;
            this.lblEmmcStatus.ForeColor = System.Drawing.Color.Red;
            this.lblEmmcStatus.Location = new System.Drawing.Point(120, 55);
            this.lblEmmcStatus.Name = "lblEmmcStatus";
            this.lblEmmcStatus.Size = new System.Drawing.Size(82, 15);
            this.lblEmmcStatus.TabIndex = 3;
            this.lblEmmcStatus.Text = "Not Initialized";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "eMMC Status:";
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.ForeColor = System.Drawing.Color.Red;
            this.lblConnection.Location = new System.Drawing.Point(120, 27);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(89, 15);
            this.lblConnection.TabIndex = 1;
            this.lblConnection.Text = "Not Connected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnClearLog);
            this.groupBox2.Controls.Add(this.btnInitEmmc);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Location = new System.Drawing.Point(258, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(530, 85);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Device Control";
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(380, 34);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(120, 30);
            this.btnClearLog.TabIndex = 2;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnInitEmmc
            // 
            this.btnInitEmmc.Enabled = false;
            this.btnInitEmmc.Location = new System.Drawing.Point(188, 34);
            this.btnInitEmmc.Name = "btnInitEmmc";
            this.btnInitEmmc.Size = new System.Drawing.Size(120, 30);
            this.btnInitEmmc.TabIndex = 1;
            this.btnInitEmmc.Text = "Initialize eMMC";
            this.btnInitEmmc.UseVisualStyleBackColor = true;
            this.btnInitEmmc.Click += new System.EventHandler(this.btnInitEmmc_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(17, 34);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(120, 30);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.btnLoadFile);
            this.groupBox3.Controls.Add(this.btnDump);
            this.groupBox3.Controls.Add(this.btnWriteBlock);
            this.groupBox3.Controls.Add(this.btnReadBlock);
            this.groupBox3.Controls.Add(this.txtBlockAddress);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(12, 103);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(776, 70);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "eMMC Operations";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(626, 27);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 30);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear Data";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(500, 27);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(120, 30);
            this.btnLoadFile.TabIndex = 5;
            this.btnLoadFile.Text = "Load File";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnDump
            // 
            this.btnDump.Enabled = false;
            this.btnDump.Location = new System.Drawing.Point(383, 27);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(111, 30);
            this.btnDump.TabIndex = 4;
            this.btnDump.Text = "Dump Blocks";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // btnWriteBlock
            // 
            this.btnWriteBlock.Enabled = false;
            this.btnWriteBlock.Location = new System.Drawing.Point(275, 27);
            this.btnWriteBlock.Name = "btnWriteBlock";
            this.btnWriteBlock.Size = new System.Drawing.Size(102, 30);
            this.btnWriteBlock.TabIndex = 3;
            this.btnWriteBlock.Text = "Write Block";
            this.btnWriteBlock.UseVisualStyleBackColor = true;
            this.btnWriteBlock.Click += new System.EventHandler(this.btnWriteBlock_Click);
            // 
            // btnReadBlock
            // 
            this.btnReadBlock.Enabled = false;
            this.btnReadBlock.Location = new System.Drawing.Point(171, 27);
            this.btnReadBlock.Name = "btnReadBlock";
            this.btnReadBlock.Size = new System.Drawing.Size(98, 30);
            this.btnReadBlock.TabIndex = 2;
            this.btnReadBlock.Text = "Read Block";
            this.btnReadBlock.UseVisualStyleBackColor = true;
            this.btnReadBlock.Click += new System.EventHandler(this.btnReadBlock_Click);
            // 
            // txtBlockAddress
            // 
            this.txtBlockAddress.Enabled = false;
            this.txtBlockAddress.Location = new System.Drawing.Point(108, 31);
            this.txtBlockAddress.Name = "txtBlockAddress";
            this.txtBlockAddress.Size = new System.Drawing.Size(57, 23);
            this.txtBlockAddress.TabIndex = 1;
            this.txtBlockAddress.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Block Address:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer1.Location = new System.Drawing.Point(0, 179);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.hexViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.logBox);
            this.splitContainer1.Size = new System.Drawing.Size(800, 271);
            this.splitContainer1.SplitterDistance = 135;
            this.splitContainer1.TabIndex = 3;
            // 
            // hexViewer
            // 
            this.hexViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexViewer.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexViewer.Location = new System.Drawing.Point(0, 0);
            this.hexViewer.Multiline = true;
            this.hexViewer.Name = "hexViewer";
            this.hexViewer.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.hexViewer.Size = new System.Drawing.Size(800, 135);
            this.hexViewer.TabIndex = 0;
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point(0, 0);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(800, 132);
            this.logBox.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "FX2LP eMMC Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblEmmcStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnInitEmmc;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.Button btnWriteBlock;
        private System.Windows.Forms.Button btnReadBlock;
        private System.Windows.Forms.TextBox txtBlockAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox hexViewer;
        private System.Windows.Forms.TextBox logBox;
    }
}