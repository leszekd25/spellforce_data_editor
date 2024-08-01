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
            components = new System.ComponentModel.Container();
            label1 = new System.Windows.Forms.Label();
            bGDEditor = new System.Windows.Forms.Button();
            bAssets = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            labelVersion = new System.Windows.Forms.Label();
            linkEditor = new System.Windows.Forms.LinkLabel();
            bMap = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            ButtonSpecifyGameDirectory = new System.Windows.Forms.Button();
            ButtonAbout = new System.Windows.Forms.Button();
            LabelIsSpecifiedGameDir = new System.Windows.Forms.Label();
            bSQLEdit = new System.Windows.Forms.Button();
            GameDirDialog = new System.Windows.Forms.FolderBrowserDialog();
            bSaveData = new System.Windows.Forms.Button();
            TimerCheckUpdateStatus = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 10);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(0, 15);
            label1.TabIndex = 0;
            // 
            // bGDEditor
            // 
            bGDEditor.Location = new System.Drawing.Point(153, 44);
            bGDEditor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bGDEditor.Name = "bGDEditor";
            bGDEditor.Size = new System.Drawing.Size(134, 27);
            bGDEditor.TabIndex = 1;
            bGDEditor.Text = "GameData Editor";
            bGDEditor.UseVisualStyleBackColor = true;
            bGDEditor.Click += bGDEditor_Click;
            // 
            // bAssets
            // 
            bAssets.ForeColor = System.Drawing.Color.Crimson;
            bAssets.Location = new System.Drawing.Point(294, 44);
            bAssets.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bAssets.Name = "bAssets";
            bAssets.Size = new System.Drawing.Size(134, 27);
            bAssets.TabIndex = 2;
            bAssets.Text = "Asset Viewer";
            bAssets.UseVisualStyleBackColor = true;
            bAssets.Click += bAssets_Click;
            // 
            // label2
            // 
            label2.AutoEllipsis = true;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(14, 10);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.MaximumSize = new System.Drawing.Size(117, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(117, 60);
            label2.TabIndex = 5;
            label2.Text = "Select tools to run! Closing this window will close all running tools.";
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Location = new System.Drawing.Point(14, 150);
            labelVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new System.Drawing.Size(70, 15);
            labelVersion.TabIndex = 6;
            labelVersion.Text = "01.08.2024.1";
            // 
            // linkEditor
            // 
            linkEditor.AutoSize = true;
            linkEditor.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            linkEditor.LinkColor = System.Drawing.Color.Blue;
            linkEditor.Location = new System.Drawing.Point(149, 150);
            linkEditor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            linkEditor.Name = "linkEditor";
            linkEditor.Size = new System.Drawing.Size(121, 15);
            linkEditor.TabIndex = 7;
            linkEditor.TabStop = true;
            linkEditor.Text = "New version available";
            linkEditor.VisitedLinkColor = System.Drawing.Color.Blue;
            linkEditor.LinkClicked += linkEditor_LinkClicked;
            // 
            // bMap
            // 
            bMap.ForeColor = System.Drawing.Color.Crimson;
            bMap.Location = new System.Drawing.Point(153, 77);
            bMap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bMap.Name = "bMap";
            bMap.Size = new System.Drawing.Size(134, 27);
            bMap.TabIndex = 8;
            bMap.Text = "Map Editor";
            bMap.UseVisualStyleBackColor = true;
            bMap.Click += bMap_Click;
            // 
            // label3
            // 
            label3.ForeColor = System.Drawing.Color.Crimson;
            label3.Location = new System.Drawing.Point(14, 77);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(126, 60);
            label3.TabIndex = 9;
            label3.Text = "Crimson features require specified game directory";
            // 
            // ButtonSpecifyGameDirectory
            // 
            ButtonSpecifyGameDirectory.ForeColor = System.Drawing.Color.Crimson;
            ButtonSpecifyGameDirectory.Location = new System.Drawing.Point(153, 10);
            ButtonSpecifyGameDirectory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonSpecifyGameDirectory.Name = "ButtonSpecifyGameDirectory";
            ButtonSpecifyGameDirectory.Size = new System.Drawing.Size(134, 27);
            ButtonSpecifyGameDirectory.TabIndex = 10;
            ButtonSpecifyGameDirectory.Text = "Specify game dir...";
            ButtonSpecifyGameDirectory.UseVisualStyleBackColor = true;
            ButtonSpecifyGameDirectory.Click += ButtonSpecifyGameDirectory_Click;
            // 
            // ButtonAbout
            // 
            ButtonAbout.Location = new System.Drawing.Point(294, 144);
            ButtonAbout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonAbout.Name = "ButtonAbout";
            ButtonAbout.Size = new System.Drawing.Size(134, 27);
            ButtonAbout.TabIndex = 11;
            ButtonAbout.Text = "About";
            ButtonAbout.UseVisualStyleBackColor = true;
            ButtonAbout.Click += ButtonAbout_Click;
            // 
            // LabelIsSpecifiedGameDir
            // 
            LabelIsSpecifiedGameDir.AutoSize = true;
            LabelIsSpecifiedGameDir.Location = new System.Drawing.Point(294, 10);
            LabelIsSpecifiedGameDir.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelIsSpecifiedGameDir.Name = "LabelIsSpecifiedGameDir";
            LabelIsSpecifiedGameDir.Size = new System.Drawing.Size(0, 15);
            LabelIsSpecifiedGameDir.TabIndex = 12;
            // 
            // bSQLEdit
            // 
            bSQLEdit.BackColor = System.Drawing.SystemColors.ControlLight;
            bSQLEdit.ForeColor = System.Drawing.Color.Crimson;
            bSQLEdit.Location = new System.Drawing.Point(294, 77);
            bSQLEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bSQLEdit.Name = "bSQLEdit";
            bSQLEdit.Size = new System.Drawing.Size(134, 27);
            bSQLEdit.TabIndex = 13;
            bSQLEdit.Text = "SQL Modifier";
            bSQLEdit.UseVisualStyleBackColor = false;
            bSQLEdit.Click += bSQLEdit_Click;
            // 
            // bSaveData
            // 
            bSaveData.Location = new System.Drawing.Point(294, 111);
            bSaveData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bSaveData.Name = "bSaveData";
            bSaveData.Size = new System.Drawing.Size(134, 27);
            bSaveData.TabIndex = 14;
            bSaveData.Text = "Save Data Editor";
            bSaveData.UseVisualStyleBackColor = true;
            bSaveData.Visible = false;
            bSaveData.Click += bSaveData_Click;
            // 
            // TimerCheckUpdateStatus
            // 
            TimerCheckUpdateStatus.Interval = 1000;
            TimerCheckUpdateStatus.Tick += TimerCheckUpdateStatus_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(440, 182);
            Controls.Add(bSaveData);
            Controls.Add(bSQLEdit);
            Controls.Add(LabelIsSpecifiedGameDir);
            Controls.Add(ButtonAbout);
            Controls.Add(ButtonSpecifyGameDirectory);
            Controls.Add(label3);
            Controls.Add(bMap);
            Controls.Add(linkEditor);
            Controls.Add(labelVersion);
            Controls.Add(label2);
            Controls.Add(bAssets);
            Controls.Add(bGDEditor);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "SpellForce Editor";
            FormClosing += MainForm_FormClosing;
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bGDEditor;
        private System.Windows.Forms.Button bAssets;
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
        private System.Windows.Forms.Button bSaveData;
        private System.Windows.Forms.Timer TimerCheckUpdateStatus;
    }
}