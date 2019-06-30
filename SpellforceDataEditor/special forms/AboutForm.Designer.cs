namespace SpellforceDataEditor.special_forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.TextBoxAbout = new System.Windows.Forms.RichTextBox();
            this.TextBoxContactInfo = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(83, 119);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // TextBoxAbout
            // 
            this.TextBoxAbout.BackColor = System.Drawing.SystemColors.Control;
            this.TextBoxAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxAbout.Cursor = System.Windows.Forms.Cursors.Default;
            this.TextBoxAbout.Location = new System.Drawing.Point(101, 12);
            this.TextBoxAbout.Name = "TextBoxAbout";
            this.TextBoxAbout.ReadOnly = true;
            this.TextBoxAbout.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.TextBoxAbout.Size = new System.Drawing.Size(272, 119);
            this.TextBoxAbout.TabIndex = 1;
            this.TextBoxAbout.Text = "";
            // 
            // TextBoxContactInfo
            // 
            this.TextBoxContactInfo.BackColor = System.Drawing.SystemColors.Control;
            this.TextBoxContactInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBoxContactInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.TextBoxContactInfo.Location = new System.Drawing.Point(12, 155);
            this.TextBoxContactInfo.Name = "TextBoxContactInfo";
            this.TextBoxContactInfo.ReadOnly = true;
            this.TextBoxContactInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.TextBoxContactInfo.Size = new System.Drawing.Size(361, 118);
            this.TextBoxContactInfo.TabIndex = 2;
            this.TextBoxContactInfo.Text = "";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 285);
            this.Controls.Add(this.TextBoxContactInfo);
            this.Controls.Add(this.TextBoxAbout);
            this.Controls.Add(this.pictureBox1);
            this.Name = "AboutForm";
            this.Text = "About the program";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AboutForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox TextBoxAbout;
        private System.Windows.Forms.RichTextBox TextBoxContactInfo;
    }
}