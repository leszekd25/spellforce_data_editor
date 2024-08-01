namespace SpellforceDataEditor.special_forms
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpelllforceCFFEditor));
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            loadGameDatacffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            undoCtrlZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            redoCtrlYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            operationHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            specialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            findAllReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            clipboardTooldebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            eXPERIMENTALLoadDiffFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            CategorySelect = new System.Windows.Forms.ComboBox();
            ElementSelect = new SFCFF.helper_forms.ListBoxNoFlicker();
            OpenGameData = new System.Windows.Forms.OpenFileDialog();
            ElementDisplayPanel = new System.Windows.Forms.Panel();
            SaveGameData = new System.Windows.Forms.SaveFileDialog();
            label1 = new System.Windows.Forms.Label();
            SearchQuery = new System.Windows.Forms.TextBox();
            radioSearchNumeric = new System.Windows.Forms.RadioButton();
            radioSearchText = new System.Windows.Forms.RadioButton();
            SearchButton = new System.Windows.Forms.Button();
            panelSearch = new System.Windows.Forms.Panel();
            ContinueSearchButton = new System.Windows.Forms.Button();
            SearchColumnID = new System.Windows.Forms.ComboBox();
            checkSearchByColumn = new System.Windows.Forms.CheckBox();
            groupSearch = new System.Windows.Forms.GroupBox();
            radioSearchFlag = new System.Windows.Forms.RadioButton();
            panelElemManipulate = new System.Windows.Forms.Panel();
            ButtonElemAdd = new System.Windows.Forms.Button();
            ButtonElemRemove = new System.Windows.Forms.Button();
            ButtonElemInsert = new System.Windows.Forms.Button();
            ElementSelect_RefreshTimer = new System.Windows.Forms.Timer(components);
            labelDescription = new System.Windows.Forms.TextBox();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ProgressBar_Main = new System.Windows.Forms.ToolStripProgressBar();
            buttonTracerBack = new System.Windows.Forms.Button();
            label_tracedesc = new System.Windows.Forms.Label();
            panelElemCopy = new System.Windows.Forms.Panel();
            ButtonElemClear = new System.Windows.Forms.Button();
            ButtonElemCopy = new System.Windows.Forms.Button();
            menuStrip1.SuspendLayout();
            panelSearch.SuspendLayout();
            groupSearch.SuspendLayout();
            panelElemManipulate.SuspendLayout();
            statusStrip1.SuspendLayout();
            panelElemCopy.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, specialToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(1101, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { loadGameDatacffToolStripMenuItem, saveAsToolStripMenuItem, closeToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadGameDatacffToolStripMenuItem
            // 
            loadGameDatacffToolStripMenuItem.Name = "loadGameDatacffToolStripMenuItem";
            loadGameDatacffToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            loadGameDatacffToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            loadGameDatacffToolStripMenuItem.Text = "Open...";
            loadGameDatacffToolStripMenuItem.Click += loadGameDatacffToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            saveAsToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;
            exitToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { undoCtrlZToolStripMenuItem, redoCtrlYToolStripMenuItem, operationHistoryToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoCtrlZToolStripMenuItem
            // 
            undoCtrlZToolStripMenuItem.Enabled = false;
            undoCtrlZToolStripMenuItem.Name = "undoCtrlZToolStripMenuItem";
            undoCtrlZToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z;
            undoCtrlZToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            undoCtrlZToolStripMenuItem.Text = "Undo";
            undoCtrlZToolStripMenuItem.Click += undoCtrlZToolStripMenuItem_Click;
            // 
            // redoCtrlYToolStripMenuItem
            // 
            redoCtrlYToolStripMenuItem.Enabled = false;
            redoCtrlYToolStripMenuItem.Name = "redoCtrlYToolStripMenuItem";
            redoCtrlYToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y;
            redoCtrlYToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            redoCtrlYToolStripMenuItem.Text = "Redo";
            redoCtrlYToolStripMenuItem.Click += redoCtrlYToolStripMenuItem_Click;
            // 
            // operationHistoryToolStripMenuItem
            // 
            operationHistoryToolStripMenuItem.Name = "operationHistoryToolStripMenuItem";
            operationHistoryToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            operationHistoryToolStripMenuItem.Text = "Operation history...";
            operationHistoryToolStripMenuItem.Click += operationHistoryToolStripMenuItem_Click;
            // 
            // specialToolStripMenuItem
            // 
            specialToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { findAllReferencesToolStripMenuItem, clipboardTooldebugToolStripMenuItem });
            specialToolStripMenuItem.Name = "specialToolStripMenuItem";
            specialToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            specialToolStripMenuItem.Text = "Special";
            // 
            // findAllReferencesToolStripMenuItem
            // 
            findAllReferencesToolStripMenuItem.Name = "findAllReferencesToolStripMenuItem";
            findAllReferencesToolStripMenuItem.ShortcutKeyDisplayString = "";
            findAllReferencesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            findAllReferencesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            findAllReferencesToolStripMenuItem.Text = "Find all references...";
            findAllReferencesToolStripMenuItem.Click += findAllReferencesToolStripMenuItem_Click;
            // 
            // clipboardTooldebugToolStripMenuItem
            // 
            clipboardTooldebugToolStripMenuItem.Name = "clipboardTooldebugToolStripMenuItem";
            clipboardTooldebugToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            clipboardTooldebugToolStripMenuItem.Text = "ClipboardTool (debug)";
            clipboardTooldebugToolStripMenuItem.Visible = false;
            // 
            // eXPERIMENTALLoadDiffFileToolStripMenuItem
            // 
            eXPERIMENTALLoadDiffFileToolStripMenuItem.Name = "eXPERIMENTALLoadDiffFileToolStripMenuItem";
            eXPERIMENTALLoadDiffFileToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // CategorySelect
            // 
            CategorySelect.Enabled = false;
            CategorySelect.FormattingEnabled = true;
            CategorySelect.Location = new System.Drawing.Point(14, 31);
            CategorySelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            CategorySelect.Name = "CategorySelect";
            CategorySelect.Size = new System.Drawing.Size(440, 23);
            CategorySelect.TabIndex = 1;
            CategorySelect.SelectedIndexChanged += CategorySelect_SelectedIndexChanged;
            // 
            // ElementSelect
            // 
            ElementSelect.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            ElementSelect.Enabled = false;
            ElementSelect.HorizontalScrollbar = true;
            ElementSelect.Location = new System.Drawing.Point(14, 62);
            ElementSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ElementSelect.MaximumSize = new System.Drawing.Size(344, 485);
            ElementSelect.Name = "ElementSelect";
            ElementSelect.Size = new System.Drawing.Size(344, 436);
            ElementSelect.TabIndex = 2;
            ElementSelect.DrawItem += ElementSelect_DrawItem;
            ElementSelect.SelectedIndexChanged += ElementSelect_SelectedIndexChanged;
            // 
            // OpenGameData
            // 
            OpenGameData.FileName = "GameData.cff";
            OpenGameData.Filter = "CFF file(*.cff)|*.cff";
            // 
            // ElementDisplayPanel
            // 
            ElementDisplayPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ElementDisplayPanel.Location = new System.Drawing.Point(461, 62);
            ElementDisplayPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ElementDisplayPanel.Name = "ElementDisplayPanel";
            ElementDisplayPanel.Size = new System.Drawing.Size(583, 436);
            ElementDisplayPanel.TabIndex = 4;
            // 
            // SaveGameData
            // 
            SaveGameData.FileName = "GameData_new.cff";
            SaveGameData.Filter = "CFF file(*.cff)|*.cff";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 40);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(38, 15);
            label1.TabIndex = 5;
            label1.Text = "Value:";
            // 
            // SearchQuery
            // 
            SearchQuery.Location = new System.Drawing.Point(56, 37);
            SearchQuery.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchQuery.Name = "SearchQuery";
            SearchQuery.Size = new System.Drawing.Size(286, 23);
            SearchQuery.TabIndex = 6;
            // 
            // radioSearchNumeric
            // 
            radioSearchNumeric.AutoSize = true;
            radioSearchNumeric.Checked = true;
            radioSearchNumeric.Location = new System.Drawing.Point(7, 15);
            radioSearchNumeric.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioSearchNumeric.Name = "radioSearchNumeric";
            radioSearchNumeric.Size = new System.Drawing.Size(71, 19);
            radioSearchNumeric.TabIndex = 7;
            radioSearchNumeric.TabStop = true;
            radioSearchNumeric.Text = "Numeric";
            radioSearchNumeric.UseVisualStyleBackColor = true;
            // 
            // radioSearchText
            // 
            radioSearchText.AutoSize = true;
            radioSearchText.Location = new System.Drawing.Point(7, 42);
            radioSearchText.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioSearchText.Name = "radioSearchText";
            radioSearchText.Size = new System.Drawing.Size(46, 19);
            radioSearchText.TabIndex = 8;
            radioSearchText.Text = "Text";
            radioSearchText.UseVisualStyleBackColor = true;
            // 
            // SearchButton
            // 
            SearchButton.Location = new System.Drawing.Point(4, 3);
            SearchButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchButton.Name = "SearchButton";
            SearchButton.Size = new System.Drawing.Size(166, 27);
            SearchButton.TabIndex = 9;
            SearchButton.Text = "New search";
            SearchButton.UseVisualStyleBackColor = true;
            SearchButton.Click += SearchButton_Click;
            // 
            // panelSearch
            // 
            panelSearch.Controls.Add(ContinueSearchButton);
            panelSearch.Controls.Add(SearchColumnID);
            panelSearch.Controls.Add(checkSearchByColumn);
            panelSearch.Controls.Add(groupSearch);
            panelSearch.Controls.Add(label1);
            panelSearch.Controls.Add(SearchButton);
            panelSearch.Controls.Add(SearchQuery);
            panelSearch.Location = new System.Drawing.Point(14, 504);
            panelSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelSearch.Name = "panelSearch";
            panelSearch.Size = new System.Drawing.Size(346, 157);
            panelSearch.TabIndex = 10;
            panelSearch.Visible = false;
            // 
            // ContinueSearchButton
            // 
            ContinueSearchButton.Enabled = false;
            ContinueSearchButton.Location = new System.Drawing.Point(176, 3);
            ContinueSearchButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ContinueSearchButton.Name = "ContinueSearchButton";
            ContinueSearchButton.Size = new System.Drawing.Size(167, 27);
            ContinueSearchButton.TabIndex = 17;
            ContinueSearchButton.Text = "Continue search";
            ContinueSearchButton.UseVisualStyleBackColor = true;
            ContinueSearchButton.Click += ContinueSearchButton_Click;
            // 
            // SearchColumnID
            // 
            SearchColumnID.Enabled = false;
            SearchColumnID.FormattingEnabled = true;
            SearchColumnID.Location = new System.Drawing.Point(176, 98);
            SearchColumnID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            SearchColumnID.Name = "SearchColumnID";
            SearchColumnID.Size = new System.Drawing.Size(166, 23);
            SearchColumnID.TabIndex = 15;
            // 
            // checkSearchByColumn
            // 
            checkSearchByColumn.AutoSize = true;
            checkSearchByColumn.Location = new System.Drawing.Point(176, 72);
            checkSearchByColumn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            checkSearchByColumn.Name = "checkSearchByColumn";
            checkSearchByColumn.Size = new System.Drawing.Size(121, 19);
            checkSearchByColumn.TabIndex = 12;
            checkSearchByColumn.Text = "Search by column";
            checkSearchByColumn.UseVisualStyleBackColor = true;
            checkSearchByColumn.CheckedChanged += checkSearchByColumn_CheckedChanged;
            // 
            // groupSearch
            // 
            groupSearch.Controls.Add(radioSearchText);
            groupSearch.Controls.Add(radioSearchFlag);
            groupSearch.Controls.Add(radioSearchNumeric);
            groupSearch.Location = new System.Drawing.Point(4, 62);
            groupSearch.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupSearch.Name = "groupSearch";
            groupSearch.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            groupSearch.Size = new System.Drawing.Size(98, 87);
            groupSearch.TabIndex = 11;
            groupSearch.TabStop = false;
            // 
            // radioSearchFlag
            // 
            radioSearchFlag.AutoSize = true;
            radioSearchFlag.Location = new System.Drawing.Point(7, 68);
            radioSearchFlag.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            radioSearchFlag.Name = "radioSearchFlag";
            radioSearchFlag.Size = new System.Drawing.Size(62, 19);
            radioSearchFlag.TabIndex = 9;
            radioSearchFlag.Text = "Bitfield";
            radioSearchFlag.UseVisualStyleBackColor = true;
            // 
            // panelElemManipulate
            // 
            panelElemManipulate.Controls.Add(ButtonElemAdd);
            panelElemManipulate.Controls.Add(ButtonElemRemove);
            panelElemManipulate.Controls.Add(ButtonElemInsert);
            panelElemManipulate.Location = new System.Drawing.Point(365, 62);
            panelElemManipulate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelElemManipulate.Name = "panelElemManipulate";
            panelElemManipulate.Size = new System.Drawing.Size(89, 107);
            panelElemManipulate.TabIndex = 11;
            panelElemManipulate.Visible = false;
            // 
            // ButtonElemAdd
            // 
            ButtonElemAdd.Location = new System.Drawing.Point(5, 5);
            ButtonElemAdd.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonElemAdd.Name = "ButtonElemAdd";
            ButtonElemAdd.Size = new System.Drawing.Size(80, 27);
            ButtonElemAdd.TabIndex = 2;
            ButtonElemAdd.Text = "Add";
            ButtonElemAdd.UseVisualStyleBackColor = true;
            ButtonElemAdd.Click += ButtonElemAdd_Click;
            // 
            // ButtonElemRemove
            // 
            ButtonElemRemove.Location = new System.Drawing.Point(4, 72);
            ButtonElemRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonElemRemove.Name = "ButtonElemRemove";
            ButtonElemRemove.Size = new System.Drawing.Size(80, 27);
            ButtonElemRemove.TabIndex = 1;
            ButtonElemRemove.Text = "Remove";
            ButtonElemRemove.UseVisualStyleBackColor = true;
            ButtonElemRemove.Click += ButtonElemRemove_Click;
            // 
            // ButtonElemInsert
            // 
            ButtonElemInsert.Location = new System.Drawing.Point(5, 38);
            ButtonElemInsert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonElemInsert.Name = "ButtonElemInsert";
            ButtonElemInsert.Size = new System.Drawing.Size(80, 27);
            ButtonElemInsert.TabIndex = 0;
            ButtonElemInsert.Text = "Insert";
            ButtonElemInsert.UseVisualStyleBackColor = true;
            ButtonElemInsert.Click += ButtonElemInsert_Click;
            // 
            // ElementSelect_RefreshTimer
            // 
            ElementSelect_RefreshTimer.Interval = 1000;
            ElementSelect_RefreshTimer.Tick += ElementSelect_RefreshTimer_Tick;
            // 
            // labelDescription
            // 
            labelDescription.BackColor = System.Drawing.SystemColors.Control;
            labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            labelDescription.Cursor = System.Windows.Forms.Cursors.No;
            labelDescription.Location = new System.Drawing.Point(461, 504);
            labelDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            labelDescription.Multiline = true;
            labelDescription.Name = "labelDescription";
            labelDescription.ReadOnly = true;
            labelDescription.Size = new System.Drawing.Size(583, 157);
            labelDescription.TabIndex = 12;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { labelStatus, ProgressBar_Main });
            statusStrip1.Location = new System.Drawing.Point(0, 667);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            statusStrip1.Size = new System.Drawing.Size(1101, 22);
            statusStrip1.TabIndex = 13;
            statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // ProgressBar_Main
            // 
            ProgressBar_Main.Maximum = 150;
            ProgressBar_Main.Name = "ProgressBar_Main";
            ProgressBar_Main.Size = new System.Drawing.Size(175, 18);
            ProgressBar_Main.Step = 1;
            ProgressBar_Main.Visible = false;
            // 
            // buttonTracerBack
            // 
            buttonTracerBack.BackColor = System.Drawing.Color.DarkOrange;
            buttonTracerBack.Location = new System.Drawing.Point(1048, 31);
            buttonTracerBack.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonTracerBack.Name = "buttonTracerBack";
            buttonTracerBack.Size = new System.Drawing.Size(49, 27);
            buttonTracerBack.TabIndex = 14;
            buttonTracerBack.Text = "Back";
            buttonTracerBack.UseVisualStyleBackColor = false;
            buttonTracerBack.Visible = false;
            buttonTracerBack.Click += buttonTracerBack_Click;
            // 
            // label_tracedesc
            // 
            label_tracedesc.AutoSize = true;
            label_tracedesc.Location = new System.Drawing.Point(464, 37);
            label_tracedesc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label_tracedesc.Name = "label_tracedesc";
            label_tracedesc.Size = new System.Drawing.Size(0, 15);
            label_tracedesc.TabIndex = 0;
            // 
            // panelElemCopy
            // 
            panelElemCopy.Controls.Add(ButtonElemClear);
            panelElemCopy.Controls.Add(ButtonElemCopy);
            panelElemCopy.Location = new System.Drawing.Point(365, 177);
            panelElemCopy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelElemCopy.Name = "panelElemCopy";
            panelElemCopy.Size = new System.Drawing.Size(89, 72);
            panelElemCopy.TabIndex = 12;
            panelElemCopy.Visible = false;
            // 
            // ButtonElemClear
            // 
            ButtonElemClear.Location = new System.Drawing.Point(4, 38);
            ButtonElemClear.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonElemClear.Name = "ButtonElemClear";
            ButtonElemClear.Size = new System.Drawing.Size(80, 27);
            ButtonElemClear.TabIndex = 1;
            ButtonElemClear.Text = "Clear";
            ButtonElemClear.UseVisualStyleBackColor = true;
            ButtonElemClear.Click += ButtonElemClear_Click;
            // 
            // ButtonElemCopy
            // 
            ButtonElemCopy.Location = new System.Drawing.Point(5, 5);
            ButtonElemCopy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonElemCopy.Name = "ButtonElemCopy";
            ButtonElemCopy.Size = new System.Drawing.Size(80, 27);
            ButtonElemCopy.TabIndex = 0;
            ButtonElemCopy.Text = "Copy";
            ButtonElemCopy.UseVisualStyleBackColor = true;
            ButtonElemCopy.Click += ButtonElemCopy_Click;
            // 
            // SpelllforceCFFEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1101, 689);
            Controls.Add(panelElemCopy);
            Controls.Add(buttonTracerBack);
            Controls.Add(labelDescription);
            Controls.Add(statusStrip1);
            Controls.Add(panelElemManipulate);
            Controls.Add(panelSearch);
            Controls.Add(ElementDisplayPanel);
            Controls.Add(ElementSelect);
            Controls.Add(CategorySelect);
            Controls.Add(menuStrip1);
            Controls.Add(label_tracedesc);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SpelllforceCFFEditor";
            Text = "GameData Editor";
            FormClosing += AskBeforeExit;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panelSearch.ResumeLayout(false);
            panelSearch.PerformLayout();
            groupSearch.ResumeLayout(false);
            groupSearch.PerformLayout();
            panelElemManipulate.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panelElemCopy.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadGameDatacffToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ComboBox CategorySelect;
        private System.Windows.Forms.OpenFileDialog OpenGameData;
        private System.Windows.Forms.Panel ElementDisplayPanel;
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
        private System.Windows.Forms.Timer ElementSelect_RefreshTimer;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.TextBox labelDescription;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar_Main;
        private System.Windows.Forms.ToolStripMenuItem eXPERIMENTALLoadDiffFileToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioSearchFlag;
        private System.Windows.Forms.Button buttonTracerBack;
        private System.Windows.Forms.Label label_tracedesc;
        private System.Windows.Forms.Button ContinueSearchButton;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoCtrlZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoCtrlYToolStripMenuItem;
        private System.Windows.Forms.Panel panelElemCopy;
        private System.Windows.Forms.Button ButtonElemClear;
        private System.Windows.Forms.Button ButtonElemCopy;
        private System.Windows.Forms.ToolStripMenuItem specialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAllReferencesToolStripMenuItem;
        private System.Windows.Forms.Button ButtonElemAdd;
        private SFCFF.helper_forms.ListBoxNoFlicker ElementSelect;
        private System.Windows.Forms.ToolStripMenuItem clipboardTooldebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operationHistoryToolStripMenuItem;
    }
}