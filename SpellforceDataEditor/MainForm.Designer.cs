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
            this.bScripting = new System.Windows.Forms.Button();
            this.bMods = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.linkEditor = new System.Windows.Forms.LinkLabel();
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
            this.bGDEditor.Location = new System.Drawing.Point(131, 4);
            this.bGDEditor.Name = "bGDEditor";
            this.bGDEditor.Size = new System.Drawing.Size(115, 23);
            this.bGDEditor.TabIndex = 1;
            this.bGDEditor.Text = "GameData Editor";
            this.bGDEditor.UseVisualStyleBackColor = true;
            this.bGDEditor.Click += new System.EventHandler(this.bGDEditor_Click);
            // 
            // bAssets
            // 
            this.bAssets.Location = new System.Drawing.Point(131, 33);
            this.bAssets.Name = "bAssets";
            this.bAssets.Size = new System.Drawing.Size(115, 23);
            this.bAssets.TabIndex = 2;
            this.bAssets.Text = "Asset Viewer";
            this.bAssets.UseVisualStyleBackColor = true;
            this.bAssets.Click += new System.EventHandler(this.bAssets_Click);
            // 
            // bScripting
            // 
            this.bScripting.Location = new System.Drawing.Point(131, 62);
            this.bScripting.Name = "bScripting";
            this.bScripting.Size = new System.Drawing.Size(115, 23);
            this.bScripting.TabIndex = 3;
            this.bScripting.Text = "Script Builder";
            this.bScripting.UseVisualStyleBackColor = true;
            this.bScripting.Click += new System.EventHandler(this.bScripting_Click);
            // 
            // bMods
            // 
            this.bMods.Location = new System.Drawing.Point(131, 91);
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
            this.labelVersion.Location = new System.Drawing.Point(12, 83);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(108, 13);
            this.labelVersion.TabIndex = 6;
            this.labelVersion.Text = "Version 12.01.2019.1";
            // 
            // linkEditor
            // 
            this.linkEditor.AutoSize = true;
            this.linkEditor.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkEditor.LinkColor = System.Drawing.Color.Blue;
            this.linkEditor.Location = new System.Drawing.Point(12, 96);
            this.linkEditor.Name = "linkEditor";
            this.linkEditor.Size = new System.Drawing.Size(111, 13);
            this.linkEditor.TabIndex = 7;
            this.linkEditor.TabStop = true;
            this.linkEditor.Text = "New version available";
            this.linkEditor.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkEditor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEditor_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 117);
            this.Controls.Add(this.linkEditor);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bMods);
            this.Controls.Add(this.bScripting);
            this.Controls.Add(this.bAssets);
            this.Controls.Add(this.bGDEditor);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "All-in-one Tool Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bGDEditor;
        private System.Windows.Forms.Button bAssets;
        private System.Windows.Forms.Button bScripting;
        private System.Windows.Forms.Button bMods;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel linkEditor;
    }
}