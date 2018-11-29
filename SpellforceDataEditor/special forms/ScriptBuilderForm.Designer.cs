namespace SpellforceDataEditor.special_forms
{
    partial class ScriptBuilderForm
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
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addPlatformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.platformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renamePlatformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewActiveScriptCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeActiveScriptTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportActiveScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAllActiveScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specifyExportDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabDragCode = new System.Windows.Forms.TabControl();
            this.TabEvents = new System.Windows.Forms.TabPage();
            this.TabConditions = new System.Windows.Forms.TabPage();
            this.TabActions = new System.Windows.Forms.TabPage();
            this.TabParameters = new System.Windows.Forms.TabPage();
            this.TabVariables = new System.Windows.Forms.TabPage();
            this.TabCinematics = new System.Windows.Forms.TabPage();
            this.TabDialogs = new System.Windows.Forms.TabPage();
            this.ListCodeEntries = new System.Windows.Forms.ListBox();
            this.LabelProject = new System.Windows.Forms.Label();
            this.TabScripts = new System.Windows.Forms.TabControl();
            this.ListPlatforms = new System.Windows.Forms.ListBox();
            this.ListScripts = new System.Windows.Forms.ListBox();
            this.SaveProjectDialog = new System.Windows.Forms.SaveFileDialog();
            this.OpenProjectDialog = new System.Windows.Forms.OpenFileDialog();
            this.PanelScriptInfo = new System.Windows.Forms.Panel();
            this.ButtonDialogEdit = new System.Windows.Forms.Button();
            this.LabelScriptType = new System.Windows.Forms.Label();
            this.LabelScriptName = new System.Windows.Forms.Label();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.TabRtsSpawnSystems = new System.Windows.Forms.TabPage();
            this.menuStrip1.SuspendLayout();
            this.TabDragCode.SuspendLayout();
            this.PanelScriptInfo.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.projToolStripMenuItem,
            this.platformToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(923, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.loadProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.closeProjectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newProjectToolStripMenuItem.Text = "New project...";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // loadProjectToolStripMenuItem
            // 
            this.loadProjectToolStripMenuItem.Name = "loadProjectToolStripMenuItem";
            this.loadProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadProjectToolStripMenuItem.Text = "Load project...";
            this.loadProjectToolStripMenuItem.Click += new System.EventHandler(this.loadProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveProjectToolStripMenuItem.Text = "Save project...";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // closeProjectToolStripMenuItem
            // 
            this.closeProjectToolStripMenuItem.Name = "closeProjectToolStripMenuItem";
            this.closeProjectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeProjectToolStripMenuItem.Text = "Close project...";
            this.closeProjectToolStripMenuItem.Click += new System.EventHandler(this.closeProjectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // projToolStripMenuItem
            // 
            this.projToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPlatformToolStripMenuItem,
            this.renameProjectToolStripMenuItem});
            this.projToolStripMenuItem.Name = "projToolStripMenuItem";
            this.projToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.projToolStripMenuItem.Text = "Project";
            // 
            // addPlatformToolStripMenuItem
            // 
            this.addPlatformToolStripMenuItem.Name = "addPlatformToolStripMenuItem";
            this.addPlatformToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.addPlatformToolStripMenuItem.Text = "Add Platform";
            this.addPlatformToolStripMenuItem.Click += new System.EventHandler(this.addPlatformToolStripMenuItem_Click);
            // 
            // renameProjectToolStripMenuItem
            // 
            this.renameProjectToolStripMenuItem.Name = "renameProjectToolStripMenuItem";
            this.renameProjectToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.renameProjectToolStripMenuItem.Text = "Rename Project";
            this.renameProjectToolStripMenuItem.Click += new System.EventHandler(this.renameProjectToolStripMenuItem_Click);
            // 
            // platformToolStripMenuItem
            // 
            this.platformToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addScriptToolStripMenuItem,
            this.renamePlatformToolStripMenuItem});
            this.platformToolStripMenuItem.Name = "platformToolStripMenuItem";
            this.platformToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.platformToolStripMenuItem.Text = "Platform";
            // 
            // addScriptToolStripMenuItem
            // 
            this.addScriptToolStripMenuItem.Name = "addScriptToolStripMenuItem";
            this.addScriptToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.addScriptToolStripMenuItem.Text = "Add Script...";
            this.addScriptToolStripMenuItem.Click += new System.EventHandler(this.addScriptToolStripMenuItem_Click);
            // 
            // renamePlatformToolStripMenuItem
            // 
            this.renamePlatformToolStripMenuItem.Name = "renamePlatformToolStripMenuItem";
            this.renamePlatformToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.renamePlatformToolStripMenuItem.Text = "Rename Platform";
            this.renamePlatformToolStripMenuItem.Click += new System.EventHandler(this.renamePlatformToolStripMenuItem_Click);
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previewActiveScriptCodeToolStripMenuItem,
            this.renameScriptToolStripMenuItem,
            this.changeActiveScriptTypeToolStripMenuItem});
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.scriptToolStripMenuItem.Text = "Script";
            // 
            // previewActiveScriptCodeToolStripMenuItem
            // 
            this.previewActiveScriptCodeToolStripMenuItem.Name = "previewActiveScriptCodeToolStripMenuItem";
            this.previewActiveScriptCodeToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.previewActiveScriptCodeToolStripMenuItem.Text = "Preview Active Script code";
            this.previewActiveScriptCodeToolStripMenuItem.Click += new System.EventHandler(this.previewActiveScriptCodeToolStripMenuItem_Click);
            // 
            // renameScriptToolStripMenuItem
            // 
            this.renameScriptToolStripMenuItem.Name = "renameScriptToolStripMenuItem";
            this.renameScriptToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.renameScriptToolStripMenuItem.Text = "Rename Active Script";
            this.renameScriptToolStripMenuItem.Click += new System.EventHandler(this.renameScriptToolStripMenuItem_Click);
            // 
            // changeActiveScriptTypeToolStripMenuItem
            // 
            this.changeActiveScriptTypeToolStripMenuItem.Name = "changeActiveScriptTypeToolStripMenuItem";
            this.changeActiveScriptTypeToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.changeActiveScriptTypeToolStripMenuItem.Text = "Change ActiveScriptType";
            this.changeActiveScriptTypeToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.changeActiveScriptTypeToolStripMenuItem_DropDownItemClicked);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportActiveScriptToolStripMenuItem,
            this.exportAllActiveScriptsToolStripMenuItem,
            this.specifyExportDirectoryToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // exportActiveScriptToolStripMenuItem
            // 
            this.exportActiveScriptToolStripMenuItem.Name = "exportActiveScriptToolStripMenuItem";
            this.exportActiveScriptToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.exportActiveScriptToolStripMenuItem.Text = "Export active script";
            // 
            // exportAllActiveScriptsToolStripMenuItem
            // 
            this.exportAllActiveScriptsToolStripMenuItem.Name = "exportAllActiveScriptsToolStripMenuItem";
            this.exportAllActiveScriptsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.exportAllActiveScriptsToolStripMenuItem.Text = "Export all open scripts";
            // 
            // specifyExportDirectoryToolStripMenuItem
            // 
            this.specifyExportDirectoryToolStripMenuItem.Name = "specifyExportDirectoryToolStripMenuItem";
            this.specifyExportDirectoryToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.specifyExportDirectoryToolStripMenuItem.Text = "Specify export directory...";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // TabDragCode
            // 
            this.TabDragCode.Controls.Add(this.TabEvents);
            this.TabDragCode.Controls.Add(this.TabConditions);
            this.TabDragCode.Controls.Add(this.TabActions);
            this.TabDragCode.Controls.Add(this.TabDialogs);
            this.TabDragCode.Controls.Add(this.TabVariables);
            this.TabDragCode.Controls.Add(this.TabRtsSpawnSystems);
            this.TabDragCode.Controls.Add(this.TabParameters);
            this.TabDragCode.Controls.Add(this.TabCinematics);
            this.TabDragCode.Location = new System.Drawing.Point(0, 28);
            this.TabDragCode.Multiline = true;
            this.TabDragCode.Name = "TabDragCode";
            this.TabDragCode.SelectedIndex = 0;
            this.TabDragCode.Size = new System.Drawing.Size(264, 194);
            this.TabDragCode.TabIndex = 3;
            this.TabDragCode.SelectedIndexChanged += new System.EventHandler(this.TabDragCode_SelectedIndexChanged);
            // 
            // TabEvents
            // 
            this.TabEvents.Location = new System.Drawing.Point(4, 40);
            this.TabEvents.Name = "TabEvents";
            this.TabEvents.Padding = new System.Windows.Forms.Padding(3);
            this.TabEvents.Size = new System.Drawing.Size(256, 150);
            this.TabEvents.TabIndex = 0;
            this.TabEvents.Text = "Events";
            this.TabEvents.UseVisualStyleBackColor = true;
            // 
            // TabConditions
            // 
            this.TabConditions.Location = new System.Drawing.Point(4, 22);
            this.TabConditions.Name = "TabConditions";
            this.TabConditions.Padding = new System.Windows.Forms.Padding(3);
            this.TabConditions.Size = new System.Drawing.Size(256, 168);
            this.TabConditions.TabIndex = 1;
            this.TabConditions.Text = "Conditions";
            this.TabConditions.UseVisualStyleBackColor = true;
            // 
            // TabActions
            // 
            this.TabActions.Location = new System.Drawing.Point(4, 22);
            this.TabActions.Name = "TabActions";
            this.TabActions.Size = new System.Drawing.Size(256, 168);
            this.TabActions.TabIndex = 2;
            this.TabActions.Text = "Actions";
            this.TabActions.UseVisualStyleBackColor = true;
            // 
            // TabParameters
            // 
            this.TabParameters.Location = new System.Drawing.Point(4, 40);
            this.TabParameters.Name = "TabParameters";
            this.TabParameters.Padding = new System.Windows.Forms.Padding(3);
            this.TabParameters.Size = new System.Drawing.Size(256, 150);
            this.TabParameters.TabIndex = 3;
            this.TabParameters.Text = "Parameters";
            this.TabParameters.UseVisualStyleBackColor = true;
            // 
            // TabVariables
            // 
            this.TabVariables.Location = new System.Drawing.Point(4, 40);
            this.TabVariables.Name = "TabVariables";
            this.TabVariables.Padding = new System.Windows.Forms.Padding(3);
            this.TabVariables.Size = new System.Drawing.Size(256, 150);
            this.TabVariables.TabIndex = 4;
            this.TabVariables.Text = "Variables";
            this.TabVariables.UseVisualStyleBackColor = true;
            // 
            // TabCinematics
            // 
            this.TabCinematics.Location = new System.Drawing.Point(4, 40);
            this.TabCinematics.Name = "TabCinematics";
            this.TabCinematics.Padding = new System.Windows.Forms.Padding(3);
            this.TabCinematics.Size = new System.Drawing.Size(256, 150);
            this.TabCinematics.TabIndex = 5;
            this.TabCinematics.Text = "Cinematics";
            this.TabCinematics.UseVisualStyleBackColor = true;
            // 
            // TabDialogs
            // 
            this.TabDialogs.Location = new System.Drawing.Point(4, 40);
            this.TabDialogs.Name = "TabDialogs";
            this.TabDialogs.Size = new System.Drawing.Size(256, 150);
            this.TabDialogs.TabIndex = 0;
            this.TabDialogs.Text = "Dialogs";
            this.TabDialogs.UseVisualStyleBackColor = true;
            // 
            // ListCodeEntries
            // 
            this.ListCodeEntries.FormattingEnabled = true;
            this.ListCodeEntries.Location = new System.Drawing.Point(4, 228);
            this.ListCodeEntries.Name = "ListCodeEntries";
            this.ListCodeEntries.Size = new System.Drawing.Size(256, 212);
            this.ListCodeEntries.TabIndex = 4;
            this.ListCodeEntries.SelectedIndexChanged += new System.EventHandler(this.ListCodeEntries_SelectedIndexChanged);
            // 
            // LabelProject
            // 
            this.LabelProject.AutoSize = true;
            this.LabelProject.Location = new System.Drawing.Point(781, 33);
            this.LabelProject.Name = "LabelProject";
            this.LabelProject.Size = new System.Drawing.Size(46, 13);
            this.LabelProject.TabIndex = 6;
            this.LabelProject.Text = "Project: ";
            // 
            // TabScripts
            // 
            this.TabScripts.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabScripts.Location = new System.Drawing.Point(266, 28);
            this.TabScripts.Name = "TabScripts";
            this.TabScripts.SelectedIndex = 0;
            this.TabScripts.Size = new System.Drawing.Size(509, 412);
            this.TabScripts.TabIndex = 7;
            this.TabScripts.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabScripts_DrawItem);
            this.TabScripts.SelectedIndexChanged += new System.EventHandler(this.TabScripts_SelectedIndexChanged);
            this.TabScripts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TabScripts_MouseDown);
            // 
            // ListPlatforms
            // 
            this.ListPlatforms.FormattingEnabled = true;
            this.ListPlatforms.Location = new System.Drawing.Point(784, 49);
            this.ListPlatforms.Name = "ListPlatforms";
            this.ListPlatforms.Size = new System.Drawing.Size(132, 134);
            this.ListPlatforms.TabIndex = 8;
            this.ListPlatforms.SelectedIndexChanged += new System.EventHandler(this.ListPlatforms_SelectedIndexChanged);
            // 
            // ListScripts
            // 
            this.ListScripts.FormattingEnabled = true;
            this.ListScripts.Location = new System.Drawing.Point(784, 189);
            this.ListScripts.Name = "ListScripts";
            this.ListScripts.Size = new System.Drawing.Size(132, 186);
            this.ListScripts.TabIndex = 9;
            this.ListScripts.SelectedIndexChanged += new System.EventHandler(this.ListScripts_SelectedIndexChanged);
            // 
            // SaveProjectDialog
            // 
            this.SaveProjectDialog.DefaultExt = "luap";
            this.SaveProjectDialog.Filter = "LuaProject files(*.luap)|*.luap";
            // 
            // OpenProjectDialog
            // 
            this.OpenProjectDialog.DefaultExt = "luap";
            this.OpenProjectDialog.FileName = "openFileDialog1";
            this.OpenProjectDialog.Filter = "LuaProject files(*.luap)|*.luap";
            // 
            // PanelScriptInfo
            // 
            this.PanelScriptInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelScriptInfo.Controls.Add(this.ButtonDialogEdit);
            this.PanelScriptInfo.Controls.Add(this.LabelScriptType);
            this.PanelScriptInfo.Controls.Add(this.LabelScriptName);
            this.PanelScriptInfo.Location = new System.Drawing.Point(784, 381);
            this.PanelScriptInfo.Name = "PanelScriptInfo";
            this.PanelScriptInfo.Size = new System.Drawing.Size(132, 59);
            this.PanelScriptInfo.TabIndex = 10;
            // 
            // ButtonDialogEdit
            // 
            this.ButtonDialogEdit.Location = new System.Drawing.Point(2, 32);
            this.ButtonDialogEdit.Name = "ButtonDialogEdit";
            this.ButtonDialogEdit.Size = new System.Drawing.Size(75, 23);
            this.ButtonDialogEdit.TabIndex = 2;
            this.ButtonDialogEdit.Text = "Edit dialog";
            this.ButtonDialogEdit.UseVisualStyleBackColor = true;
            this.ButtonDialogEdit.Click += new System.EventHandler(this.ButtonDialogEdit_Click);
            // 
            // LabelScriptType
            // 
            this.LabelScriptType.AutoSize = true;
            this.LabelScriptType.Location = new System.Drawing.Point(3, 17);
            this.LabelScriptType.Name = "LabelScriptType";
            this.LabelScriptType.Size = new System.Drawing.Size(35, 13);
            this.LabelScriptType.TabIndex = 1;
            this.LabelScriptType.Text = "label1";
            // 
            // LabelScriptName
            // 
            this.LabelScriptName.AutoSize = true;
            this.LabelScriptName.Location = new System.Drawing.Point(3, 0);
            this.LabelScriptName.Name = "LabelScriptName";
            this.LabelScriptName.Size = new System.Drawing.Size(35, 13);
            this.LabelScriptName.TabIndex = 0;
            this.LabelScriptName.Text = "label1";
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.StatusStrip.Location = new System.Drawing.Point(0, 445);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(923, 22);
            this.StatusStrip.TabIndex = 11;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(0, 17);
            // 
            // TabRtsSpawnSystems
            // 
            this.TabRtsSpawnSystems.Location = new System.Drawing.Point(4, 40);
            this.TabRtsSpawnSystems.Name = "TabRtsSpawnSystems";
            this.TabRtsSpawnSystems.Padding = new System.Windows.Forms.Padding(3);
            this.TabRtsSpawnSystems.Size = new System.Drawing.Size(256, 150);
            this.TabRtsSpawnSystems.TabIndex = 6;
            this.TabRtsSpawnSystems.Text = "RtsSpawnSystems";
            this.TabRtsSpawnSystems.UseVisualStyleBackColor = true;
            // 
            // ScriptBuilderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 467);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.PanelScriptInfo);
            this.Controls.Add(this.ListScripts);
            this.Controls.Add(this.ListPlatforms);
            this.Controls.Add(this.TabScripts);
            this.Controls.Add(this.LabelProject);
            this.Controls.Add(this.ListCodeEntries);
            this.Controls.Add(this.TabDragCode);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScriptBuilderForm";
            this.Text = " ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptBuilderForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.TabDragCode.ResumeLayout(false);
            this.PanelScriptInfo.ResumeLayout(false);
            this.PanelScriptInfo.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TabControl TabDragCode;
        private System.Windows.Forms.TabPage TabEvents;
        private System.Windows.Forms.TabPage TabConditions;
        private System.Windows.Forms.TabPage TabActions;
        private System.Windows.Forms.ListBox ListCodeEntries;
        private System.Windows.Forms.TabPage TabParameters;
        private System.Windows.Forms.Label LabelProject;
        private System.Windows.Forms.TabControl TabScripts;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportActiveScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPlatformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem platformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addScriptToolStripMenuItem;
        private System.Windows.Forms.ListBox ListPlatforms;
        private System.Windows.Forms.ListBox ListScripts;
        private System.Windows.Forms.ToolStripMenuItem exportAllActiveScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specifyExportDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog SaveProjectDialog;
        private System.Windows.Forms.OpenFileDialog OpenProjectDialog;
        private System.Windows.Forms.ToolStripMenuItem renameProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renamePlatformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewActiveScriptCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameScriptToolStripMenuItem;
        private System.Windows.Forms.TabPage TabVariables;
        private System.Windows.Forms.ToolStripMenuItem changeActiveScriptTypeToolStripMenuItem;
        private System.Windows.Forms.Panel PanelScriptInfo;
        private System.Windows.Forms.Label LabelScriptType;
        private System.Windows.Forms.Label LabelScriptName;
        private System.Windows.Forms.TabPage TabCinematics;
        private System.Windows.Forms.Button ButtonDialogEdit;
        private System.Windows.Forms.TabPage TabDialogs;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.TabPage TabRtsSpawnSystems;
    }
}