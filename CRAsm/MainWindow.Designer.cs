namespace CRAsm
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SourceTab = new System.Windows.Forms.TabPage();
            this.sourceText = new System.Windows.Forms.TextBox();
            this.ListingTab = new System.Windows.Forms.TabPage();
            this.ListingText = new System.Windows.Forms.TextBox();
            this.HexTab = new System.Windows.Forms.TabPage();
            this.hexText = new System.Windows.Forms.TextBox();
            this.hexFileTab = new System.Windows.Forms.TabPage();
            this.hexFileText = new System.Windows.Forms.TextBox();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.fontDialog2 = new System.Windows.Forms.FontDialog();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.LoadButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.CompileButton = new System.Windows.Forms.ToolStripButton();
            this.CopyButton = new System.Windows.Forms.ToolStripButton();
            this.PasteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.GoButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1.SuspendLayout();
            this.SourceTab.SuspendLayout();
            this.ListingTab.SuspendLayout();
            this.HexTab.SuspendLayout();
            this.hexFileTab.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SourceTab);
            this.tabControl1.Controls.Add(this.ListingTab);
            this.tabControl1.Controls.Add(this.HexTab);
            this.tabControl1.Controls.Add(this.hexFileTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1149, 696);
            this.tabControl1.TabIndex = 0;
            // 
            // SourceTab
            // 
            this.SourceTab.Controls.Add(this.sourceText);
            this.SourceTab.Location = new System.Drawing.Point(4, 23);
            this.SourceTab.Name = "SourceTab";
            this.SourceTab.Padding = new System.Windows.Forms.Padding(3);
            this.SourceTab.Size = new System.Drawing.Size(1141, 669);
            this.SourceTab.TabIndex = 0;
            this.SourceTab.Text = "Source";
            this.SourceTab.UseVisualStyleBackColor = true;
            // 
            // sourceText
            // 
            this.sourceText.AcceptsReturn = true;
            this.sourceText.AcceptsTab = true;
            this.sourceText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceText.Location = new System.Drawing.Point(3, 3);
            this.sourceText.Multiline = true;
            this.sourceText.Name = "sourceText";
            this.sourceText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.sourceText.Size = new System.Drawing.Size(1135, 663);
            this.sourceText.TabIndex = 0;
            // 
            // ListingTab
            // 
            this.ListingTab.Controls.Add(this.ListingText);
            this.ListingTab.Location = new System.Drawing.Point(4, 23);
            this.ListingTab.Name = "ListingTab";
            this.ListingTab.Size = new System.Drawing.Size(1141, 669);
            this.ListingTab.TabIndex = 2;
            this.ListingTab.Text = "Listing";
            this.ListingTab.UseVisualStyleBackColor = true;
            // 
            // ListingText
            // 
            this.ListingText.AcceptsReturn = true;
            this.ListingText.AcceptsTab = true;
            this.ListingText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListingText.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.ListingText.Location = new System.Drawing.Point(0, 0);
            this.ListingText.Multiline = true;
            this.ListingText.Name = "ListingText";
            this.ListingText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ListingText.Size = new System.Drawing.Size(1141, 669);
            this.ListingText.TabIndex = 2;
            // 
            // HexTab
            // 
            this.HexTab.Controls.Add(this.hexText);
            this.HexTab.Location = new System.Drawing.Point(4, 23);
            this.HexTab.Name = "HexTab";
            this.HexTab.Padding = new System.Windows.Forms.Padding(3);
            this.HexTab.Size = new System.Drawing.Size(1141, 669);
            this.HexTab.TabIndex = 1;
            this.HexTab.Text = "Hex";
            this.HexTab.UseVisualStyleBackColor = true;
            // 
            // hexText
            // 
            this.hexText.AcceptsReturn = true;
            this.hexText.AcceptsTab = true;
            this.hexText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexText.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexText.Location = new System.Drawing.Point(3, 3);
            this.hexText.Multiline = true;
            this.hexText.Name = "hexText";
            this.hexText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.hexText.Size = new System.Drawing.Size(1135, 663);
            this.hexText.TabIndex = 0;
            // 
            // hexFileTab
            // 
            this.hexFileTab.Controls.Add(this.hexFileText);
            this.hexFileTab.Location = new System.Drawing.Point(4, 23);
            this.hexFileTab.Name = "hexFileTab";
            this.hexFileTab.Size = new System.Drawing.Size(1141, 669);
            this.hexFileTab.TabIndex = 3;
            this.hexFileTab.Text = "Hex File";
            this.hexFileTab.UseVisualStyleBackColor = true;
            // 
            // hexFileText
            // 
            this.hexFileText.AcceptsReturn = true;
            this.hexFileText.AcceptsTab = true;
            this.hexFileText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexFileText.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexFileText.Location = new System.Drawing.Point(0, 0);
            this.hexFileText.Multiline = true;
            this.hexFileText.Name = "hexFileText";
            this.hexFileText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.hexFileText.Size = new System.Drawing.Size(1141, 669);
            this.hexFileText.TabIndex = 1;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tabControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1149, 696);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1149, 721);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadButton,
            this.SaveButton,
            this.CompileButton,
            this.CopyButton,
            this.PasteButton,
            this.toolStripSeparator1,
            this.GoButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(428, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // LoadButton
            // 
            this.LoadButton.Image = ((System.Drawing.Image)(resources.GetObject("LoadButton.Image")));
            this.LoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(56, 22);
            this.LoadButton.Text = "Open";
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(51, 22);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CompileButton
            // 
            this.CompileButton.Image = ((System.Drawing.Image)(resources.GetObject("CompileButton.Image")));
            this.CompileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CompileButton.Name = "CompileButton";
            this.CompileButton.Size = new System.Drawing.Size(78, 22);
            this.CompileButton.Text = "Assemble";
            this.CompileButton.Click += new System.EventHandler(this.CompileButton_Click);
            // 
            // CopyButton
            // 
            this.CopyButton.Image = ((System.Drawing.Image)(resources.GetObject("CopyButton.Image")));
            this.CopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.Size = new System.Drawing.Size(78, 22);
            this.CopyButton.Text = "Copy Hex";
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // PasteButton
            // 
            this.PasteButton.Image = ((System.Drawing.Image)(resources.GetObject("PasteButton.Image")));
            this.PasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PasteButton.Name = "PasteButton";
            this.PasteButton.Size = new System.Drawing.Size(82, 22);
            this.PasteButton.Text = "Paste Asm";
            this.PasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // GoButton
            // 
            this.GoButton.Image = ((System.Drawing.Image)(resources.GetObject("GoButton.Image")));
            this.GoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(65, 22);
            this.GoButton.Text = "Upload";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1149, 721);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "MainWindow";
            this.Text = "CR Assembler";
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.SourceTab.ResumeLayout(false);
            this.SourceTab.PerformLayout();
            this.ListingTab.ResumeLayout(false);
            this.ListingTab.PerformLayout();
            this.HexTab.ResumeLayout(false);
            this.HexTab.PerformLayout();
            this.hexFileTab.ResumeLayout(false);
            this.hexFileTab.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage SourceTab;
        private System.Windows.Forms.TextBox sourceText;
        private System.Windows.Forms.TabPage HexTab;
        private System.Windows.Forms.TextBox hexText;
        private System.Windows.Forms.TabPage ListingTab;
        private System.Windows.Forms.TextBox ListingText;
        private System.Windows.Forms.TabPage hexFileTab;
        private System.Windows.Forms.TextBox hexFileText;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.FontDialog fontDialog2;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton LoadButton;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripButton CompileButton;
        private System.Windows.Forms.ToolStripButton CopyButton;
        private System.Windows.Forms.ToolStripButton PasteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton GoButton;
    }
}

