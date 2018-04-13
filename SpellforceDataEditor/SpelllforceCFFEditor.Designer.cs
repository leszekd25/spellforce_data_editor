namespace SpellforceDataEditor
{
    partial class SpelllforceCFFEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameDatacffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CategorySelect = new System.Windows.Forms.ComboBox();
            this.ElementSelect = new System.Windows.Forms.ListBox();
            this.OpenGameData = new System.Windows.Forms.OpenFileDialog();
            this.SFControlPanel = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(944, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadGameDatacffToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadGameDatacffToolStripMenuItem
            // 
            this.loadGameDatacffToolStripMenuItem.Name = "loadGameDatacffToolStripMenuItem";
            this.loadGameDatacffToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.loadGameDatacffToolStripMenuItem.Text = "Load GameData.cff";
            this.loadGameDatacffToolStripMenuItem.Click += new System.EventHandler(this.loadGameDatacffToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // CategorySelect
            // 
            this.CategorySelect.Enabled = false;
            this.CategorySelect.FormattingEnabled = true;
            this.CategorySelect.Location = new System.Drawing.Point(12, 27);
            this.CategorySelect.Name = "CategorySelect";
            this.CategorySelect.Size = new System.Drawing.Size(295, 21);
            this.CategorySelect.TabIndex = 1;
            this.CategorySelect.SelectedIndexChanged += new System.EventHandler(this.CategorySelect_SelectedIndexChanged);
            // 
            // ElementSelect
            // 
            this.ElementSelect.Enabled = false;
            this.ElementSelect.FormattingEnabled = true;
            this.ElementSelect.Location = new System.Drawing.Point(12, 54);
            this.ElementSelect.Name = "ElementSelect";
            this.ElementSelect.Size = new System.Drawing.Size(295, 329);
            this.ElementSelect.TabIndex = 2;
            this.ElementSelect.SelectedIndexChanged += new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
            // 
            // OpenGameData
            // 
            this.OpenGameData.FileName = "GameData.cff";
            this.OpenGameData.Filter = "CFF file(*.cff)|*.cff";
            // 
            // SFControlPanel
            // 
            this.SFControlPanel.Location = new System.Drawing.Point(395, 27);
            this.SFControlPanel.Name = "SFControlPanel";
            this.SFControlPanel.Size = new System.Drawing.Size(500, 520);
            this.SFControlPanel.TabIndex = 4;
            // 
            // SpelllforceCFFEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 601);
            this.Controls.Add(this.SFControlPanel);
            this.Controls.Add(this.ElementSelect);
            this.Controls.Add(this.CategorySelect);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpelllforceCFFEditor";
            this.Text = "SpellforceDataEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameDatacffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox CategorySelect;
        private System.Windows.Forms.ListBox ElementSelect;
        private System.Windows.Forms.OpenFileDialog OpenGameData;
        private System.Windows.Forms.Panel SFControlPanel;
    }
}