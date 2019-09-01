namespace SpellforceDataEditor.special_forms
{
    partial class ModManagerForm
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
            this.ModList = new System.Windows.Forms.CheckedListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.modsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadModListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeYourOwnModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specifySpellforceexeArgumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusProgreessBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusGameInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.PanelInfo = new System.Windows.Forms.Panel();
            this.LabelModInfo = new System.Windows.Forms.Label();
            this.ButtonApplyMods = new System.Windows.Forms.Button();
            this.SelectOrigCFF = new System.Windows.Forms.OpenFileDialog();
            this.SelectGameDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectOrigEXE = new System.Windows.Forms.OpenFileDialog();
            this.RestoreButton = new System.Windows.Forms.Button();
            this.ButtonApplyAndRun = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.PanelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // ModList
            // 
            this.ModList.FormattingEnabled = true;
            this.ModList.Location = new System.Drawing.Point(294, 27);
            this.ModList.Name = "ModList";
            this.ModList.Size = new System.Drawing.Size(190, 169);
            this.ModList.TabIndex = 0;
            this.ModList.SelectedIndexChanged += new System.EventHandler(this.ModList_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modsToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(496, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // modsToolStripMenuItem
            // 
            this.modsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreToOriginalToolStripMenuItem,
            this.reloadModListToolStripMenuItem,
            this.makeYourOwnModToolStripMenuItem});
            this.modsToolStripMenuItem.Name = "modsToolStripMenuItem";
            this.modsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.modsToolStripMenuItem.Text = "Mods";
            // 
            // restoreToOriginalToolStripMenuItem
            // 
            this.restoreToOriginalToolStripMenuItem.Name = "restoreToOriginalToolStripMenuItem";
            this.restoreToOriginalToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.restoreToOriginalToolStripMenuItem.Text = "Restore game to original";
            this.restoreToOriginalToolStripMenuItem.Click += new System.EventHandler(this.restoreToOriginalToolStripMenuItem_Click);
            // 
            // reloadModListToolStripMenuItem
            // 
            this.reloadModListToolStripMenuItem.Name = "reloadModListToolStripMenuItem";
            this.reloadModListToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.reloadModListToolStripMenuItem.Text = "Reload mod list";
            this.reloadModListToolStripMenuItem.Click += new System.EventHandler(this.reloadModListToolStripMenuItem_Click);
            // 
            // makeYourOwnModToolStripMenuItem
            // 
            this.makeYourOwnModToolStripMenuItem.Name = "makeYourOwnModToolStripMenuItem";
            this.makeYourOwnModToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.makeYourOwnModToolStripMenuItem.Text = "Make your own mod...";
            this.makeYourOwnModToolStripMenuItem.Click += new System.EventHandler(this.makeYourOwnModToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.specifySpellforceexeArgumentsToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // specifySpellforceexeArgumentsToolStripMenuItem
            // 
            this.specifySpellforceexeArgumentsToolStripMenuItem.Name = "specifySpellforceexeArgumentsToolStripMenuItem";
            this.specifySpellforceexeArgumentsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.specifySpellforceexeArgumentsToolStripMenuItem.Text = "SpellForce.exe run arguments";
            this.specifySpellforceexeArgumentsToolStripMenuItem.Click += new System.EventHandler(this.specifySpellforceexeArgumentsToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusProgreessBar,
            this.StatusText,
            this.StatusGameInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 236);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(496, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusProgreessBar
            // 
            this.StatusProgreessBar.Name = "StatusProgreessBar";
            this.StatusProgreessBar.Size = new System.Drawing.Size(100, 16);
            this.StatusProgreessBar.Visible = false;
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(0, 17);
            // 
            // StatusGameInfo
            // 
            this.StatusGameInfo.AutoToolTip = true;
            this.StatusGameInfo.Name = "StatusGameInfo";
            this.StatusGameInfo.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.StatusGameInfo.Size = new System.Drawing.Size(100, 17);
            // 
            // PanelInfo
            // 
            this.PanelInfo.Controls.Add(this.LabelModInfo);
            this.PanelInfo.Location = new System.Drawing.Point(12, 27);
            this.PanelInfo.Name = "PanelInfo";
            this.PanelInfo.Size = new System.Drawing.Size(276, 169);
            this.PanelInfo.TabIndex = 5;
            // 
            // LabelModInfo
            // 
            this.LabelModInfo.AutoEllipsis = true;
            this.LabelModInfo.Location = new System.Drawing.Point(0, 0);
            this.LabelModInfo.Name = "LabelModInfo";
            this.LabelModInfo.Size = new System.Drawing.Size(276, 169);
            this.LabelModInfo.TabIndex = 0;
            // 
            // ButtonApplyMods
            // 
            this.ButtonApplyMods.Location = new System.Drawing.Point(294, 202);
            this.ButtonApplyMods.Name = "ButtonApplyMods";
            this.ButtonApplyMods.Size = new System.Drawing.Size(71, 23);
            this.ButtonApplyMods.TabIndex = 6;
            this.ButtonApplyMods.Text = "Apply mods ";
            this.ButtonApplyMods.UseVisualStyleBackColor = true;
            this.ButtonApplyMods.Click += new System.EventHandler(this.ButtonApplyMods_Click);
            // 
            // SelectOrigCFF
            // 
            this.SelectOrigCFF.AddExtension = false;
            this.SelectOrigCFF.FileName = "GameData.cff";
            this.SelectOrigCFF.Filter = "CFF files|*.cff";
            // 
            // SelectOrigEXE
            // 
            this.SelectOrigEXE.FileName = "SpellForce.exe";
            this.SelectOrigEXE.Filter = "Executable files|*.exe";
            // 
            // RestoreButton
            // 
            this.RestoreButton.Location = new System.Drawing.Point(12, 202);
            this.RestoreButton.Name = "RestoreButton";
            this.RestoreButton.Size = new System.Drawing.Size(125, 23);
            this.RestoreButton.TabIndex = 7;
            this.RestoreButton.Text = "Restore to original";
            this.RestoreButton.UseVisualStyleBackColor = true;
            this.RestoreButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // ButtonApplyAndRun
            // 
            this.ButtonApplyAndRun.Location = new System.Drawing.Point(371, 202);
            this.ButtonApplyAndRun.Name = "ButtonApplyAndRun";
            this.ButtonApplyAndRun.Size = new System.Drawing.Size(113, 23);
            this.ButtonApplyAndRun.TabIndex = 8;
            this.ButtonApplyAndRun.Text = "Apply mods and run";
            this.ButtonApplyAndRun.UseVisualStyleBackColor = true;
            this.ButtonApplyAndRun.Click += new System.EventHandler(this.ButtonApplyAndRun_Click);
            // 
            // ModManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 258);
            this.Controls.Add(this.ButtonApplyAndRun);
            this.Controls.Add(this.RestoreButton);
            this.Controls.Add(this.ButtonApplyMods);
            this.Controls.Add(this.PanelInfo);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ModList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ModManagerForm";
            this.Text = "Mod Manager";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.PanelInfo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ModList;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem modsToolStripMenuItem;
        private System.Windows.Forms.Panel PanelInfo;
        private System.Windows.Forms.ToolStripMenuItem makeYourOwnModToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ToolStripProgressBar StatusProgreessBar;
        private System.Windows.Forms.ToolStripMenuItem restoreToOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StatusGameInfo;
        private System.Windows.Forms.Button ButtonApplyMods;
        private System.Windows.Forms.OpenFileDialog SelectOrigCFF;
        private System.Windows.Forms.FolderBrowserDialog SelectGameDirectory;
        private System.Windows.Forms.OpenFileDialog SelectOrigEXE;
        private System.Windows.Forms.Label LabelModInfo;
        private System.Windows.Forms.ToolStripMenuItem reloadModListToolStripMenuItem;
        private System.Windows.Forms.Button RestoreButton;
        private System.Windows.Forms.Button ButtonApplyAndRun;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specifySpellforceexeArgumentsToolStripMenuItem;
    }
}