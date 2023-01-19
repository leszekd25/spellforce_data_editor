
namespace SpellforceDataEditor.special_forms
{
    partial class TextureRepairForm
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
            this.ButtonTexDirectorySource = new System.Windows.Forms.Button();
            this.ButtonTexDirectoryDestination = new System.Windows.Forms.Button();
            this.TextBoxTexDirectorySource = new System.Windows.Forms.TextBox();
            this.TextBoxTexDirectoryDestination = new System.Windows.Forms.TextBox();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.ButtonRepairStart = new System.Windows.Forms.Button();
            this.LabelDescription = new System.Windows.Forms.Label();
            this.SelectFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // ButtonTexDirectorySource
            // 
            this.ButtonTexDirectorySource.Location = new System.Drawing.Point(12, 12);
            this.ButtonTexDirectorySource.Name = "ButtonTexDirectorySource";
            this.ButtonTexDirectorySource.Size = new System.Drawing.Size(165, 23);
            this.ButtonTexDirectorySource.TabIndex = 0;
            this.ButtonTexDirectorySource.Text = "Source directory";
            this.ButtonTexDirectorySource.UseVisualStyleBackColor = true;
            this.ButtonTexDirectorySource.Click += new System.EventHandler(this.ButtonTexDirectorySource_Click);
            // 
            // ButtonTexDirectoryDestination
            // 
            this.ButtonTexDirectoryDestination.Location = new System.Drawing.Point(12, 41);
            this.ButtonTexDirectoryDestination.Name = "ButtonTexDirectoryDestination";
            this.ButtonTexDirectoryDestination.Size = new System.Drawing.Size(165, 23);
            this.ButtonTexDirectoryDestination.TabIndex = 1;
            this.ButtonTexDirectoryDestination.Text = "Destination directory";
            this.ButtonTexDirectoryDestination.UseVisualStyleBackColor = true;
            this.ButtonTexDirectoryDestination.Click += new System.EventHandler(this.ButtonTexDirectoryDestination_Click);
            // 
            // TextBoxTexDirectorySource
            // 
            this.TextBoxTexDirectorySource.Enabled = false;
            this.TextBoxTexDirectorySource.Location = new System.Drawing.Point(183, 15);
            this.TextBoxTexDirectorySource.Name = "TextBoxTexDirectorySource";
            this.TextBoxTexDirectorySource.Size = new System.Drawing.Size(279, 20);
            this.TextBoxTexDirectorySource.TabIndex = 2;
            // 
            // TextBoxTexDirectoryDestination
            // 
            this.TextBoxTexDirectoryDestination.Enabled = false;
            this.TextBoxTexDirectoryDestination.Location = new System.Drawing.Point(183, 44);
            this.TextBoxTexDirectoryDestination.Name = "TextBoxTexDirectoryDestination";
            this.TextBoxTexDirectoryDestination.Size = new System.Drawing.Size(279, 20);
            this.TextBoxTexDirectoryDestination.TabIndex = 3;
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(12, 139);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(316, 25);
            this.Progress.TabIndex = 4;
            // 
            // ButtonRepairStart
            // 
            this.ButtonRepairStart.Enabled = false;
            this.ButtonRepairStart.Location = new System.Drawing.Point(334, 141);
            this.ButtonRepairStart.Name = "ButtonRepairStart";
            this.ButtonRepairStart.Size = new System.Drawing.Size(128, 23);
            this.ButtonRepairStart.TabIndex = 5;
            this.ButtonRepairStart.Text = "Repair";
            this.ButtonRepairStart.UseVisualStyleBackColor = true;
            this.ButtonRepairStart.Click += new System.EventHandler(this.ButtonRepairStart_Click);
            // 
            // LabelDescription
            // 
            this.LabelDescription.Location = new System.Drawing.Point(12, 67);
            this.LabelDescription.Name = "LabelDescription";
            this.LabelDescription.Size = new System.Drawing.Size(316, 71);
            this.LabelDescription.TabIndex = 6;
            // 
            // TextureRepairForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 170);
            this.Controls.Add(this.LabelDescription);
            this.Controls.Add(this.ButtonRepairStart);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.TextBoxTexDirectoryDestination);
            this.Controls.Add(this.TextBoxTexDirectorySource);
            this.Controls.Add(this.ButtonTexDirectoryDestination);
            this.Controls.Add(this.ButtonTexDirectorySource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureRepairForm";
            this.ShowIcon = false;
            this.Text = "Texture repair";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonTexDirectorySource;
        private System.Windows.Forms.Button ButtonTexDirectoryDestination;
        private System.Windows.Forms.TextBox TextBoxTexDirectorySource;
        private System.Windows.Forms.TextBox TextBoxTexDirectoryDestination;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.Button ButtonRepairStart;
        private System.Windows.Forms.Label LabelDescription;
        private System.Windows.Forms.FolderBrowserDialog SelectFolderDialog;
    }
}