namespace SpellforceDataEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.bGDEditor = new System.Windows.Forms.Button();
            this.bAssets = new System.Windows.Forms.Button();
            this.bMods = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkEditor = new System.Windows.Forms.LinkLabel();
            this.bMap = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ButtonSpecifyGameDirectory = new System.Windows.Forms.Button();
            this.ButtonAbout = new System.Windows.Forms.Button();
            this.LabelIsSpecifiedGameDir = new System.Windows.Forms.Label();
            this.bSQLEdit = new System.Windows.Forms.Button();
            this.GameDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // bGDEditor
            // 
            this.bGDEditor.Location = new System.Drawing.Point(131, 38);
            this.bGDEditor.Name = "bGDEditor";
            this.bGDEditor.Size = new System.Drawing.Size(115, 23);
            this.bGDEditor.TabIndex = 1;
            this.bGDEditor.Text = "GameData Editor";
            this.bGDEditor.UseVisualStyleBackColor = true;
            this.bGDEditor.Click += new System.EventHandler(this.bGDEditor_Click);
            // 
            // bAssets
            // 
            this.bAssets.ForeColor = System.Drawing.Color.Crimson;
            this.bAssets.Location = new System.Drawing.Point(252, 38);
            this.bAssets.Name = "bAssets";
            this.bAssets.Size = new System.Drawing.Size(115, 23);
            this.bAssets.TabIndex = 2;
            this.bAssets.Text = "Asset Viewer";
            this.bAssets.UseVisualStyleBackColor = true;
            this.bAssets.Click += new System.EventHandler(this.bAssets_Click);
            // 
            // bMods
            // 
            this.bMods.ForeColor = System.Drawing.Color.Crimson;
            this.bMods.Location = new System.Drawing.Point(252, 67);
            this.bMods.Name = "bMods";
            this.bMods.Size = new System.Drawing.Size(115, 23);
            this.bMods.TabIndex = 4;
            this.bMods.Text = "Mod Manager";
            this.bMods.UseVisualStyleBackColor = true;
            this.bMods.Click += new System.EventHandler(this.bMods_Click);
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.MaximumSize = new System.Drawing.Size(100, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 52);
            this.label2.TabIndex = 5;
            this.label2.Text = "Select tools to run! Closing this window will close all running tools.";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(12, 130);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(108, 13);
            this.labelVersion.TabIndex = 6;
            this.labelVersion.Text = "Version 30.09.2019.1";
            // 
            // linkEditor
            // 
            this.linkEditor.AutoSize = true;
            this.linkEditor.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkEditor.LinkColor = System.Drawing.Color.Blue;
            this.linkEditor.Location = new System.Drawing.Point(128, 130);
            this.linkEditor.Name = "linkEditor";
            this.linkEditor.Size = new System.Drawing.Size(111, 13);
            this.linkEditor.TabIndex = 7;
            this.linkEditor.TabStop = true;
            this.linkEditor.Text = "New version available";
            this.linkEditor.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkEditor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEditor_LinkClicked);
            // 
            // bMap
            // 
            this.bMap.ForeColor = System.Drawing.Color.Crimson;
            this.bMap.Location = new System.Drawing.Point(131, 67);
            this.bMap.Name = "bMap";
            this.bMap.Size = new System.Drawing.Size(115, 23);
            this.bMap.TabIndex = 8;
            this.bMap.Text = "Map Editor";
            this.bMap.UseVisualStyleBackColor = true;
            this.bMap.Click += new System.EventHandler(this.bMap_Click);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Crimson;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 52);
            this.label3.TabIndex = 9;
            this.label3.Text = "Crimson features require specified game directory";
            // 
            // ButtonSpecifyGameDirectory
            // 
            this.ButtonSpecifyGameDirectory.ForeColor = System.Drawing.Color.Crimson;
            this.ButtonSpecifyGameDirectory.Location = new System.Drawing.Point(131, 9);
            this.ButtonSpecifyGameDirectory.Name = "ButtonSpecifyGameDirectory";
            this.ButtonSpecifyGameDirectory.Size = new System.Drawing.Size(115, 23);
            this.ButtonSpecifyGameDirectory.TabIndex = 10;
            this.ButtonSpecifyGameDirectory.Text = "Specify game dir...";
            this.ButtonSpecifyGameDirectory.UseVisualStyleBackColor = true;
            this.ButtonSpecifyGameDirectory.Click += new System.EventHandler(this.ButtonSpecifyGameDirectory_Click);
            // 
            // ButtonAbout
            // 
            this.ButtonAbout.Location = new System.Drawing.Point(252, 125);
            this.ButtonAbout.Name = "ButtonAbout";
            this.ButtonAbout.Size = new System.Drawing.Size(115, 23);
            this.ButtonAbout.TabIndex = 11;
            this.ButtonAbout.Text = "About";
            this.ButtonAbout.UseVisualStyleBackColor = true;
            this.ButtonAbout.Click += new System.EventHandler(this.ButtonAbout_Click);
            // 
            // LabelIsSpecifiedGameDir
            // 
            this.LabelIsSpecifiedGameDir.AutoSize = true;
            this.LabelIsSpecifiedGameDir.Location = new System.Drawing.Point(252, 9);
            this.LabelIsSpecifiedGameDir.Name = "LabelIsSpecifiedGameDir";
            this.LabelIsSpecifiedGameDir.Size = new System.Drawing.Size(0, 13);
            this.LabelIsSpecifiedGameDir.TabIndex = 12;
            // 
            // bSQLEdit
            // 
            this.bSQLEdit.BackColor = System.Drawing.SystemColors.ControlLight;
            this.bSQLEdit.ForeColor = System.Drawing.Color.Crimson;
            this.bSQLEdit.Location = new System.Drawing.Point(131, 96);
            this.bSQLEdit.Name = "bSQLEdit";
            this.bSQLEdit.Size = new System.Drawing.Size(115, 23);
            this.bSQLEdit.TabIndex = 13;
            this.bSQLEdit.Text = "SQL Modifier";
            this.bSQLEdit.UseVisualStyleBackColor = false;
            this.bSQLEdit.Click += new System.EventHandler(this.bSQLEdit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 158);
            this.Controls.Add(this.bSQLEdit);
            this.Controls.Add(this.LabelIsSpecifiedGameDir);
            this.Controls.Add(this.ButtonAbout);
            this.Controls.Add(this.ButtonSpecifyGameDirectory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bMap);
            this.Controls.Add(this.linkEditor);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bMods);
            this.Controls.Add(this.bAssets);
            this.Controls.Add(this.bGDEditor);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "SpellForce Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bGDEditor;
        private System.Windows.Forms.Button bAssets;
        private System.Windows.Forms.Button bMods;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkEditor;
        private System.Windows.Forms.Button bMap;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ButtonSpecifyGameDirectory;
        private System.Windows.Forms.Button ButtonAbout;
        private System.Windows.Forms.Label LabelIsSpecifiedGameDir;
        private System.Windows.Forms.Button bSQLEdit;
        private System.Windows.Forms.FolderBrowserDialog GameDirDialog;
    }
}