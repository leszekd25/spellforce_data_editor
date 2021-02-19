namespace SpellforceDataEditor.special_forms
{
    partial class SaveDataEditorForm
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
            this.TreeChunks = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.RawData = new System.Windows.Forms.RichTextBox();
            this.ButtonUnpack = new System.Windows.Forms.Button();
            this.ButtonExtract = new System.Windows.Forms.Button();
            this.OpenSave = new System.Windows.Forms.OpenFileDialog();
            this.LabelChunkData = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TreeChunks
            // 
            this.TreeChunks.Location = new System.Drawing.Point(12, 27);
            this.TreeChunks.Name = "TreeChunks";
            this.TreeChunks.Size = new System.Drawing.Size(236, 444);
            this.TreeChunks.TabIndex = 0;
            this.TreeChunks.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeChunks_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1032, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 502);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1032, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // RawData
            // 
            this.RawData.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RawData.Location = new System.Drawing.Point(254, 53);
            this.RawData.Name = "RawData";
            this.RawData.ReadOnly = true;
            this.RawData.Size = new System.Drawing.Size(766, 418);
            this.RawData.TabIndex = 3;
            this.RawData.Text = "testing";
            // 
            // ButtonUnpack
            // 
            this.ButtonUnpack.Location = new System.Drawing.Point(254, 477);
            this.ButtonUnpack.Name = "ButtonUnpack";
            this.ButtonUnpack.Size = new System.Drawing.Size(75, 23);
            this.ButtonUnpack.TabIndex = 4;
            this.ButtonUnpack.Text = "Unpack";
            this.ButtonUnpack.UseVisualStyleBackColor = true;
            this.ButtonUnpack.Click += new System.EventHandler(this.ButtonUnpack_Click);
            // 
            // ButtonExtract
            // 
            this.ButtonExtract.Location = new System.Drawing.Point(945, 476);
            this.ButtonExtract.Name = "ButtonExtract";
            this.ButtonExtract.Size = new System.Drawing.Size(75, 23);
            this.ButtonExtract.TabIndex = 5;
            this.ButtonExtract.Text = "Extract";
            this.ButtonExtract.UseVisualStyleBackColor = true;
            this.ButtonExtract.Click += new System.EventHandler(this.ButtonExtract_Click);
            // 
            // OpenSave
            // 
            this.OpenSave.Filter = "All files|*.*|Save files (.sav)|*.sav";
            // 
            // LabelChunkData
            // 
            this.LabelChunkData.AutoSize = true;
            this.LabelChunkData.Location = new System.Drawing.Point(254, 37);
            this.LabelChunkData.Name = "LabelChunkData";
            this.LabelChunkData.Size = new System.Drawing.Size(38, 13);
            this.LabelChunkData.TabIndex = 6;
            this.LabelChunkData.Text = "testing";
            // 
            // SaveDataEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 524);
            this.Controls.Add(this.LabelChunkData);
            this.Controls.Add(this.ButtonExtract);
            this.Controls.Add(this.ButtonUnpack);
            this.Controls.Add(this.RawData);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.TreeChunks);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SaveDataEditorForm";
            this.Text = "SaveDataEditorForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView TreeChunks;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.RichTextBox RawData;
        private System.Windows.Forms.Button ButtonUnpack;
        private System.Windows.Forms.Button ButtonExtract;
        private System.Windows.Forms.OpenFileDialog OpenSave;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Label LabelChunkData;
    }
}