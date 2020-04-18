namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleColorControl
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
            this.ButtonMaxColor = new System.Windows.Forms.Button();
            this.ButtonMinColor = new System.Windows.Forms.Button();
            this.TrackBarMinAlpha = new System.Windows.Forms.TrackBar();
            this.ColorPick = new System.Windows.Forms.ColorDialog();
            this.TrackBarMaxAlpha = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarMinAlpha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarMaxAlpha)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(36, 13);
            this.LabelName.Text = "Color";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 258);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Min";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 294);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 16);
            this.label8.TabIndex = 21;
            this.label8.Text = "Min";
            // 
            // ButtonMaxColor
            // 
            this.ButtonMaxColor.Location = new System.Drawing.Point(93, 288);
            this.ButtonMaxColor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ButtonMaxColor.Name = "ButtonMaxColor";
            this.ButtonMaxColor.Size = new System.Drawing.Size(161, 28);
            this.ButtonMaxColor.TabIndex = 22;
            this.ButtonMaxColor.UseVisualStyleBackColor = true;
            this.ButtonMaxColor.Click += new System.EventHandler(this.ButtonMaxColor_Click);
            // 
            // ButtonMinColor
            // 
            this.ButtonMinColor.Location = new System.Drawing.Point(93, 252);
            this.ButtonMinColor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ButtonMinColor.Name = "ButtonMinColor";
            this.ButtonMinColor.Size = new System.Drawing.Size(161, 28);
            this.ButtonMinColor.TabIndex = 23;
            this.ButtonMinColor.UseVisualStyleBackColor = true;
            this.ButtonMinColor.Click += new System.EventHandler(this.ButtonMinColor_Click);
            // 
            // TrackBarMinAlpha
            // 
            this.TrackBarMinAlpha.AutoSize = false;
            this.TrackBarMinAlpha.Location = new System.Drawing.Point(293, 252);
            this.TrackBarMinAlpha.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TrackBarMinAlpha.Maximum = 255;
            this.TrackBarMinAlpha.Name = "TrackBarMinAlpha";
            this.TrackBarMinAlpha.Size = new System.Drawing.Size(233, 30);
            this.TrackBarMinAlpha.TabIndex = 25;
            this.TrackBarMinAlpha.TickFrequency = 15;
            this.TrackBarMinAlpha.Value = 255;
            this.TrackBarMinAlpha.ValueChanged += new System.EventHandler(this.TrackBarMinAlpha_ValueChanged);
            // 
            // TrackBarMaxAlpha
            // 
            this.TrackBarMaxAlpha.AutoSize = false;
            this.TrackBarMaxAlpha.Location = new System.Drawing.Point(293, 289);
            this.TrackBarMaxAlpha.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TrackBarMaxAlpha.Maximum = 255;
            this.TrackBarMaxAlpha.Name = "TrackBarMaxAlpha";
            this.TrackBarMaxAlpha.Size = new System.Drawing.Size(233, 30);
            this.TrackBarMaxAlpha.TabIndex = 26;
            this.TrackBarMaxAlpha.TickFrequency = 15;
            this.TrackBarMaxAlpha.Value = 255;
            this.TrackBarMaxAlpha.ValueChanged += new System.EventHandler(this.TrackBarMaxAlpha_ValueChanged);
            // 
            // ParticleColorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TrackBarMaxAlpha);
            this.Controls.Add(this.TrackBarMinAlpha);
            this.Controls.Add(this.ButtonMinColor);
            this.Controls.Add(this.ButtonMaxColor);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleColorControl";
            this.Size = new System.Drawing.Size(531, 325);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.ButtonMaxColor, 0);
            this.Controls.SetChildIndex(this.ButtonMinColor, 0);
            this.Controls.SetChildIndex(this.TrackBarMinAlpha, 0);
            this.Controls.SetChildIndex(this.TrackBarMaxAlpha, 0);
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarMinAlpha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBarMaxAlpha)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button ButtonMaxColor;
        private System.Windows.Forms.Button ButtonMinColor;
        private System.Windows.Forms.TrackBar TrackBarMinAlpha;
        private System.Windows.Forms.ColorDialog ColorPick;
        private System.Windows.Forms.TrackBar TrackBarMaxAlpha;
    }
}
