namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleTrailControl
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
            this.TextBoxMax = new System.Windows.Forms.TextBox();
            this.TextBoxMin = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(32, 13);
            this.LabelName.Text = "Trail";
            // 
            // TextBoxMax
            // 
            this.TextBoxMax.Location = new System.Drawing.Point(377, 252);
            this.TextBoxMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMax.Name = "TextBoxMax";
            this.TextBoxMax.Size = new System.Drawing.Size(148, 22);
            this.TextBoxMax.TabIndex = 36;
            this.TextBoxMax.Validated += new System.EventHandler(this.TextBoxMax_Validated);
            // 
            // TextBoxMin
            // 
            this.TextBoxMin.Location = new System.Drawing.Point(93, 252);
            this.TextBoxMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMin.Name = "TextBoxMin";
            this.TextBoxMin.Size = new System.Drawing.Size(160, 22);
            this.TextBoxMin.TabIndex = 35;
            this.TextBoxMin.Validated += new System.EventHandler(this.TextBoxMin_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(288, 256);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 16);
            this.label8.TabIndex = 34;
            this.label8.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 256);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 16);
            this.label7.TabIndex = 33;
            this.label7.Text = "Min";
            // 
            // ParticleTrailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxMax);
            this.Controls.Add(this.TextBoxMin);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleTrailControl";
            this.Size = new System.Drawing.Size(531, 283);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.TextBoxMin, 0);
            this.Controls.SetChildIndex(this.TextBoxMax, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxMax;
        private System.Windows.Forms.TextBox TextBoxMin;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
    }
}
