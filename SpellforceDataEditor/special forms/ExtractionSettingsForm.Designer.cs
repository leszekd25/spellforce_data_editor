namespace SpellforceDataEditor.special_forms
{
    partial class ExtractionSettingsForm
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
            this.ButtonSelectExtractionDirectory = new System.Windows.Forms.Button();
            this.TextBoxExtractionDirectory = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PanelExtractionMode = new System.Windows.Forms.Panel();
            this.AllInOne = new System.Windows.Forms.RadioButton();
            this.Subdirectories = new System.Windows.Forms.RadioButton();
            this.DescriptionExtractionMode = new System.Windows.Forms.Label();
            this.SelectExtractionDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.PanelExtractionMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Extraction directory";
            // 
            // ButtonSelectExtractionDirectory
            // 
            this.ButtonSelectExtractionDirectory.Location = new System.Drawing.Point(449, 4);
            this.ButtonSelectExtractionDirectory.Name = "ButtonSelectExtractionDirectory";
            this.ButtonSelectExtractionDirectory.Size = new System.Drawing.Size(108, 23);
            this.ButtonSelectExtractionDirectory.TabIndex = 1;
            this.ButtonSelectExtractionDirectory.Text = "Select directory...";
            this.ButtonSelectExtractionDirectory.UseVisualStyleBackColor = true;
            this.ButtonSelectExtractionDirectory.Click += new System.EventHandler(this.ButtonSelectExtractionDirectory_Click);
            // 
            // TextBoxExtractionDirectory
            // 
            this.TextBoxExtractionDirectory.Location = new System.Drawing.Point(115, 6);
            this.TextBoxExtractionDirectory.Name = "TextBoxExtractionDirectory";
            this.TextBoxExtractionDirectory.ReadOnly = true;
            this.TextBoxExtractionDirectory.Size = new System.Drawing.Size(328, 20);
            this.TextBoxExtractionDirectory.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Extraction mode";
            // 
            // PanelExtractionMode
            // 
            this.PanelExtractionMode.Controls.Add(this.DescriptionExtractionMode);
            this.PanelExtractionMode.Controls.Add(this.Subdirectories);
            this.PanelExtractionMode.Controls.Add(this.AllInOne);
            this.PanelExtractionMode.Location = new System.Drawing.Point(115, 32);
            this.PanelExtractionMode.Name = "PanelExtractionMode";
            this.PanelExtractionMode.Size = new System.Drawing.Size(442, 100);
            this.PanelExtractionMode.TabIndex = 4;
            // 
            // AllInOne
            // 
            this.AllInOne.AutoSize = true;
            this.AllInOne.Location = new System.Drawing.Point(3, 3);
            this.AllInOne.Name = "AllInOne";
            this.AllInOne.Size = new System.Drawing.Size(111, 17);
            this.AllInOne.TabIndex = 0;
            this.AllInOne.TabStop = true;
            this.AllInOne.Text = "All in one directory";
            this.AllInOne.UseVisualStyleBackColor = true;
            this.AllInOne.CheckedChanged += new System.EventHandler(this.AllInOne_CheckedChanged);
            // 
            // Subdirectories
            // 
            this.Subdirectories.AutoSize = true;
            this.Subdirectories.Location = new System.Drawing.Point(3, 26);
            this.Subdirectories.Name = "Subdirectories";
            this.Subdirectories.Size = new System.Drawing.Size(92, 17);
            this.Subdirectories.TabIndex = 1;
            this.Subdirectories.TabStop = true;
            this.Subdirectories.Text = "Subdirectories";
            this.Subdirectories.UseVisualStyleBackColor = true;
            this.Subdirectories.CheckedChanged += new System.EventHandler(this.Subdirectories_CheckedChanged);
            // 
            // DescriptionExtractionMode
            // 
            this.DescriptionExtractionMode.AutoEllipsis = true;
            this.DescriptionExtractionMode.Location = new System.Drawing.Point(131, 5);
            this.DescriptionExtractionMode.Name = "DescriptionExtractionMode";
            this.DescriptionExtractionMode.Size = new System.Drawing.Size(308, 83);
            this.DescriptionExtractionMode.TabIndex = 2;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonOK.Location = new System.Drawing.Point(449, 138);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(108, 23);
            this.ButtonOK.TabIndex = 5;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ExtractionSettingsForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonOK;
            this.ClientSize = new System.Drawing.Size(569, 169);
            this.ControlBox = false;
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.PanelExtractionMode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxExtractionDirectory);
            this.Controls.Add(this.ButtonSelectExtractionDirectory);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExtractionSettingsForm";
            this.Text = "Extraction settings";
            this.Load += new System.EventHandler(this.ExtractionSettingsForm_Load);
            this.PanelExtractionMode.ResumeLayout(false);
            this.PanelExtractionMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSelectExtractionDirectory;
        private System.Windows.Forms.TextBox TextBoxExtractionDirectory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PanelExtractionMode;
        private System.Windows.Forms.Label DescriptionExtractionMode;
        private System.Windows.Forms.RadioButton Subdirectories;
        private System.Windows.Forms.RadioButton AllInOne;
        private System.Windows.Forms.FolderBrowserDialog SelectExtractionDirectory;
        private System.Windows.Forms.Button ButtonOK;
    }
}