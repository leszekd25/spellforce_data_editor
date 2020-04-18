namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleSoundControl
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
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TextBoxLength = new System.Windows.Forms.TextBox();
            this.TextBoxSoundName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(43, 13);
            this.LabelName.Text = "Sound";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 256);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Sound name";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 288);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 16);
            this.label8.TabIndex = 21;
            this.label8.Text = "Length";
            // 
            // TextBoxLength
            // 
            this.TextBoxLength.Location = new System.Drawing.Point(93, 284);
            this.TextBoxLength.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxLength.Name = "TextBoxLength";
            this.TextBoxLength.Size = new System.Drawing.Size(160, 22);
            this.TextBoxLength.TabIndex = 22;
            this.TextBoxLength.Validated += new System.EventHandler(this.TextBoxLength_Validated);
            // 
            // TextBoxSoundName
            // 
            this.TextBoxSoundName.Location = new System.Drawing.Point(93, 252);
            this.TextBoxSoundName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxSoundName.Name = "TextBoxSoundName";
            this.TextBoxSoundName.Size = new System.Drawing.Size(432, 22);
            this.TextBoxSoundName.TabIndex = 23;
            this.TextBoxSoundName.Validated += new System.EventHandler(this.TextBoxSoundName_Validated);
            // 
            // ParticleSoundControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxSoundName);
            this.Controls.Add(this.TextBoxLength);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleSoundControl";
            this.Size = new System.Drawing.Size(531, 319);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.TextBoxLength, 0);
            this.Controls.SetChildIndex(this.TextBoxSoundName, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TextBoxLength;
        private System.Windows.Forms.TextBox TextBoxSoundName;
    }
}
