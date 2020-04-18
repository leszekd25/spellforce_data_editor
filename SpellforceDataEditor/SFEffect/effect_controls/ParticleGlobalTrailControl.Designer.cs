namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleGlobalTrailControl
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
            this.TextBoxBuffer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(68, 13);
            this.LabelName.Text = "GlobalTrail";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 255);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Buffer";
            // 
            // TextBoxBuffer
            // 
            this.TextBoxBuffer.Location = new System.Drawing.Point(93, 251);
            this.TextBoxBuffer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxBuffer.Name = "TextBoxBuffer";
            this.TextBoxBuffer.Size = new System.Drawing.Size(160, 22);
            this.TextBoxBuffer.TabIndex = 21;
            this.TextBoxBuffer.Validated += new System.EventHandler(this.TextBoxBuffer_Validated);
            // 
            // ParticleGlobalTrailControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxBuffer);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleGlobalTrailControl";
            this.Size = new System.Drawing.Size(531, 283);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.TextBoxBuffer, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBoxBuffer;
    }
}
