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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadGameDatacffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CategorySelect = new System.Windows.Forms.ComboBox();
            this.ElementSelect = new System.Windows.Forms.ListBox();
            this.OpenGameData = new System.Windows.Forms.OpenFileDialog();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.SaveGameData = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.SearchQuery = new System.Windows.Forms.TextBox();
            this.radioSearchNumeric = new System.Windows.Forms.RadioButton();
            this.radioSearchText = new System.Windows.Forms.RadioButton();
            this.SearchButton = new System.Windows.Forms.Button();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.checkSearchDescription = new System.Windows.Forms.CheckBox();
            this.SearchColumnID = new System.Windows.Forms.ComboBox();
            this.checkSearchByColumn = new System.Windows.Forms.CheckBox();
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.panelElemManipulate = new System.Windows.Forms.Panel();
            this.ButtonElemRemove = new System.Windows.Forms.Button();
            this.ButtonElemInsert = new System.Windows.Forms.Button();
            this.ElementSelect_RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.labelDescription = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar_Main = new System.Windows.Forms.ToolStripProgressBar();
            this.OpenDataDiff = new System.Windows.Forms.OpenFileDialog();
            this.radioSearchFlag = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.panelElemManipulate.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
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
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem,
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
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // eXPERIMENTALLoadDiffFileToolStripMenuItem
            // 
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem.Name = "eXPERIMENTALLoadDiffFileToolStripMenuItem";
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem.Text = "(EXPERIMENTAL) Load diff file...";
            this.eXPERIMENTALLoadDiffFileToolStripMenuItem.Click += new System.EventHandler(this.eXPERIMENTALLoadDiffFileToolStripMenuItem_Click);
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
            this.ElementSelect.HorizontalScrollbar = true;
            this.ElementSelect.Location = new System.Drawing.Point(12, 54);
            this.ElementSelect.MaximumSize = new System.Drawing.Size(295, 400);
            this.ElementSelect.Name = "ElementSelect";
            this.ElementSelect.Size = new System.Drawing.Size(295, 394);
            this.ElementSelect.TabIndex = 2;
            this.ElementSelect.SelectedIndexChanged += new System.EventHandler(this.ElementSelect_SelectedIndexChanged);
            // 
            // OpenGameData
            // 
            this.OpenGameData.FileName = "GameData.cff";
            this.OpenGameData.Filter = "CFF file(*.cff)|*.cff";
            // 
            // SearchPanel
            // 
            this.SearchPanel.Location = new System.Drawing.Point(395, 27);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(500, 421);
            this.SearchPanel.TabIndex = 4;
            // 
            // SaveGameData
            // 
            this.SaveGameData.FileName = "GameData_new.cff";
            this.SaveGameData.Filter = "CFF file(*.cff)|*.cff";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Search for value";
            // 
            // SearchQuery
            // 
            this.SearchQuery.Location = new System.Drawing.Point(95, 9);
            this.SearchQuery.Name = "SearchQuery";
            this.SearchQuery.Size = new System.Drawing.Size(199, 20);
            this.SearchQuery.TabIndex = 6;
            // 
            // radioSearchNumeric
            // 
            this.radioSearchNumeric.AutoSize = true;
            this.radioSearchNumeric.Checked = true;
            this.radioSearchNumeric.Location = new System.Drawing.Point(6, 10);
            this.radioSearchNumeric.Name = "radioSearchNumeric";
            this.radioSearchNumeric.Size = new System.Drawing.Size(64, 17);
            this.radioSearchNumeric.TabIndex = 7;
            this.radioSearchNumeric.Text = "Numeric";
            this.radioSearchNumeric.UseVisualStyleBackColor = true;
            // 
            // radioSearchText
            // 
            this.radioSearchText.AutoSize = true;
            this.radioSearchText.Location = new System.Drawing.Point(76, 10);
            this.radioSearchText.Name = "radioSearchText";
            this.radioSearchText.Size = new System.Drawing.Size(46, 17);
            this.radioSearchText.TabIndex = 8;
            this.radioSearchText.Text = "Text";
            this.radioSearchText.UseVisualStyleBackColor = true;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(163, 57);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(131, 23);
            this.SearchButton.TabIndex = 9;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.checkSearchDescription);
            this.panelSearch.Controls.Add(this.SearchColumnID);
            this.panelSearch.Controls.Add(this.checkSearchByColumn);
            this.panelSearch.Controls.Add(this.groupSearch);
            this.panelSearch.Controls.Add(this.label1);
            this.panelSearch.Controls.Add(this.SearchButton);
            this.panelSearch.Controls.Add(this.SearchQuery);
            this.panelSearch.Location = new System.Drawing.Point(12, 454);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(297, 144);
            this.panelSearch.TabIndex = 10;
            this.panelSearch.Visible = false;
            // 
            // checkSearchDescription
            // 
            this.checkSearchDescription.AutoSize = true;
            this.checkSearchDescription.Location = new System.Drawing.Point(8, 124);
            this.checkSearchDescription.Name = "checkSearchDescription";
            this.checkSearchDescription.Size = new System.Drawing.Size(119, 17);
            this.checkSearchDescription.TabIndex = 16;
            this.checkSearchDescription.Text = "Search descriptions";
            this.checkSearchDescription.UseVisualStyleBackColor = true;
            // 
            // SearchColumnID
            // 
            this.SearchColumnID.Enabled = false;
            this.SearchColumnID.FormattingEnabled = true;
            this.SearchColumnID.Location = new System.Drawing.Point(117, 97);
            this.SearchColumnID.Name = "SearchColumnID";
            this.SearchColumnID.Size = new System.Drawing.Size(177, 21);
            this.SearchColumnID.TabIndex = 15;
            // 
            // checkSearchByColumn
            // 
            this.checkSearchByColumn.AutoSize = true;
            this.checkSearchByColumn.Location = new System.Drawing.Point(8, 99);
            this.checkSearchByColumn.Name = "checkSearchByColumn";
            this.checkSearchByColumn.Size = new System.Drawing.Size(111, 17);
            this.checkSearchByColumn.TabIndex = 12;
            this.checkSearchByColumn.Text = "Search by column";
            this.checkSearchByColumn.UseVisualStyleBackColor = true;
            this.checkSearchByColumn.CheckedChanged += new System.EventHandler(this.checkSearchByColumn_CheckedChanged);
            // 
            // groupSearch
            // 
            this.groupSearch.Controls.Add(this.radioSearchFlag);
            this.groupSearch.Controls.Add(this.radioSearchNumeric);
            this.groupSearch.Controls.Add(this.radioSearchText);
            this.groupSearch.Location = new System.Drawing.Point(2, 35);
            this.groupSearch.Name = "groupSearch";
            this.groupSearch.Size = new System.Drawing.Size(155, 58);
            this.groupSearch.TabIndex = 11;
            this.groupSearch.TabStop = false;
            // 
            // panelElemManipulate
            // 
            this.panelElemManipulate.Controls.Add(this.ButtonElemRemove);
            this.panelElemManipulate.Controls.Add(this.ButtonElemInsert);
            this.panelElemManipulate.Location = new System.Drawing.Point(313, 54);
            this.panelElemManipulate.Name = "panelElemManipulate";
            this.panelElemManipulate.Size = new System.Drawing.Size(76, 115);
            this.panelElemManipulate.TabIndex = 11;
            this.panelElemManipulate.Visible = false;
            // 
            // ButtonElemRemove
            // 
            this.ButtonElemRemove.Location = new System.Drawing.Point(3, 33);
            this.ButtonElemRemove.Name = "ButtonElemRemove";
            this.ButtonElemRemove.Size = new System.Drawing.Size(69, 23);
            this.ButtonElemRemove.TabIndex = 1;
            this.ButtonElemRemove.Text = "Remove";
            this.ButtonElemRemove.UseVisualStyleBackColor = true;
            this.ButtonElemRemove.Click += new System.EventHandler(this.ButtonElemRemove_Click);
            // 
            // ButtonElemInsert
            // 
            this.ButtonElemInsert.Location = new System.Drawing.Point(4, 4);
            this.ButtonElemInsert.Name = "ButtonElemInsert";
            this.ButtonElemInsert.Size = new System.Drawing.Size(69, 23);
            this.ButtonElemInsert.TabIndex = 0;
            this.ButtonElemInsert.Text = "Insert";
            this.ButtonElemInsert.UseVisualStyleBackColor = true;
            this.ButtonElemInsert.Click += new System.EventHandler(this.ButtonElemInsert_Click);
            // 
            // ElementSelect_RefreshTimer
            // 
            this.ElementSelect_RefreshTimer.Interval = 1000;
            this.ElementSelect_RefreshTimer.Tick += new System.EventHandler(this.ElementSelect_RefreshTimer_Tick);
            // 
            // labelDescription
            // 
            this.labelDescription.BackColor = System.Drawing.SystemColors.Control;
            this.labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelDescription.Cursor = System.Windows.Forms.Cursors.No;
            this.labelDescription.Location = new System.Drawing.Point(395, 463);
            this.labelDescription.Multiline = true;
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.ReadOnly = true;
            this.labelDescription.Size = new System.Drawing.Size(500, 126);
            this.labelDescription.TabIndex = 12;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus,
            this.ProgressBar_Main});
            this.statusStrip1.Location = new System.Drawing.Point(0, 601);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(944, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // ProgressBar_Main
            // 
            this.ProgressBar_Main.Maximum = 150;
            this.ProgressBar_Main.Name = "ProgressBar_Main";
            this.ProgressBar_Main.Size = new System.Drawing.Size(150, 16);
            this.ProgressBar_Main.Visible = false;
            // 
            // OpenDataDiff
            // 
            this.OpenDataDiff.FileName = "GameData.dff";
            this.OpenDataDiff.Filter = "DFF file(*.dff)|*.dff";
            // 
            // radioSearchFlag
            // 
            this.radioSearchFlag.AutoSize = true;
            this.radioSearchFlag.Location = new System.Drawing.Point(6, 33);
            this.radioSearchFlag.Name = "radioSearchFlag";
            this.radioSearchFlag.Size = new System.Drawing.Size(45, 17);
            this.radioSearchFlag.TabIndex = 9;
            this.radioSearchFlag.Text = "Flag";
            this.radioSearchFlag.UseVisualStyleBackColor = true;
            // 
            // SpelllforceCFFEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 623);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.panelElemManipulate);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.SearchPanel);
            this.Controls.Add(this.ElementSelect);
            this.Controls.Add(this.CategorySelect);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpelllforceCFFEditor";
            this.Text = "SpellforceDataEditor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.groupSearch.ResumeLayout(false);
            this.groupSearch.PerformLayout();
            this.panelElemManipulate.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.Panel SearchPanel;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog SaveGameData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SearchQuery;
        private System.Windows.Forms.RadioButton radioSearchNumeric;
        private System.Windows.Forms.RadioButton radioSearchText;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.GroupBox groupSearch;
        private System.Windows.Forms.Panel panelElemManipulate;
        private System.Windows.Forms.Button ButtonElemRemove;
        private System.Windows.Forms.Button ButtonElemInsert;
        private System.Windows.Forms.CheckBox checkSearchByColumn;
        private System.Windows.Forms.ComboBox SearchColumnID;
        private System.Windows.Forms.CheckBox checkSearchDescription;
        private System.Windows.Forms.Timer ElementSelect_RefreshTimer;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.TextBox labelDescription;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar_Main;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eXPERIMENTALLoadDiffFileToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog OpenDataDiff;
        private System.Windows.Forms.RadioButton radioSearchFlag;
    }
}