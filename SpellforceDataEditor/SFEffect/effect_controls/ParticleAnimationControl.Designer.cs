namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleAnimationControl
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
            this.TextBoxAnimation = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(62, 13);
            this.LabelName.Text = "Animation";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 255);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Animation";
            // 
            // TextBoxAnimation
            // 
            this.TextBoxAnimation.Location = new System.Drawing.Point(93, 251);
            this.TextBoxAnimation.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxAnimation.Name = "TextBoxAnimation";
            this.TextBoxAnimation.Size = new System.Drawing.Size(432, 22);
            this.TextBoxAnimation.TabIndex = 21;
            this.TextBoxAnimation.Validated += new System.EventHandler(this.TextBoxAnimation_Validated);
            // 
            // ParticleAnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxAnimation);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleAnimationControl";
            this.Size = new System.Drawing.Size(531, 282);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.TextBoxAnimation, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TextBoxAnimation;
    }
}
