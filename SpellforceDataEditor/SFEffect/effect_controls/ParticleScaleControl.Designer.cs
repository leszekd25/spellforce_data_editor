namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleScaleControl
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
            this.TextBoxMaxZ = new System.Windows.Forms.TextBox();
            this.TextBoxMaxY = new System.Windows.Forms.TextBox();
            this.TextBoxMaxX = new System.Windows.Forms.TextBox();
            this.TextBoxMinZ = new System.Windows.Forms.TextBox();
            this.TextBoxMinY = new System.Windows.Forms.TextBox();
            this.TextBoxMinX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(39, 13);
            this.LabelName.Text = "Scale";
            // 
            // TextBoxMaxZ
            // 
            this.TextBoxMaxZ.Location = new System.Drawing.Point(269, 282);
            this.TextBoxMaxZ.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMaxZ.Name = "TextBoxMaxZ";
            this.TextBoxMaxZ.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMaxZ.TabIndex = 34;
            this.TextBoxMaxZ.Validated += new System.EventHandler(this.TextBoxMaxZ_Validated);
            // 
            // TextBoxMaxY
            // 
            this.TextBoxMaxY.Location = new System.Drawing.Point(181, 282);
            this.TextBoxMaxY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMaxY.Name = "TextBoxMaxY";
            this.TextBoxMaxY.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMaxY.TabIndex = 33;
            this.TextBoxMaxY.Validated += new System.EventHandler(this.TextBoxMaxY_Validated);
            // 
            // TextBoxMaxX
            // 
            this.TextBoxMaxX.Location = new System.Drawing.Point(93, 282);
            this.TextBoxMaxX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMaxX.Name = "TextBoxMaxX";
            this.TextBoxMaxX.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMaxX.TabIndex = 32;
            this.TextBoxMaxX.Validated += new System.EventHandler(this.TextBoxMaxX_Validated);
            // 
            // TextBoxMinZ
            // 
            this.TextBoxMinZ.Location = new System.Drawing.Point(269, 252);
            this.TextBoxMinZ.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMinZ.Name = "TextBoxMinZ";
            this.TextBoxMinZ.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMinZ.TabIndex = 31;
            this.TextBoxMinZ.Validated += new System.EventHandler(this.TextBoxMinZ_Validated);
            // 
            // TextBoxMinY
            // 
            this.TextBoxMinY.Location = new System.Drawing.Point(181, 252);
            this.TextBoxMinY.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMinY.Name = "TextBoxMinY";
            this.TextBoxMinY.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMinY.TabIndex = 30;
            this.TextBoxMinY.Validated += new System.EventHandler(this.TextBoxMinY_Validated);
            // 
            // TextBoxMinX
            // 
            this.TextBoxMinX.Location = new System.Drawing.Point(93, 252);
            this.TextBoxMinX.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMinX.Name = "TextBoxMinX";
            this.TextBoxMinX.Size = new System.Drawing.Size(79, 22);
            this.TextBoxMinX.TabIndex = 29;
            this.TextBoxMinX.Validated += new System.EventHandler(this.TextBoxMinX_Validated);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 286);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 16);
            this.label8.TabIndex = 28;
            this.label8.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 256);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 16);
            this.label7.TabIndex = 27;
            this.label7.Text = "Min";
            // 
            // ParticleScaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxMaxZ);
            this.Controls.Add(this.TextBoxMaxY);
            this.Controls.Add(this.TextBoxMaxX);
            this.Controls.Add(this.TextBoxMinZ);
            this.Controls.Add(this.TextBoxMinY);
            this.Controls.Add(this.TextBoxMinX);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleScaleControl";
            this.Size = new System.Drawing.Size(533, 314);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.TextBoxMinX, 0);
            this.Controls.SetChildIndex(this.TextBoxMinY, 0);
            this.Controls.SetChildIndex(this.TextBoxMinZ, 0);
            this.Controls.SetChildIndex(this.TextBoxMaxX, 0);
            this.Controls.SetChildIndex(this.TextBoxMaxY, 0);
            this.Controls.SetChildIndex(this.TextBoxMaxZ, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextBoxMaxZ;
        private System.Windows.Forms.TextBox TextBoxMaxY;
        private System.Windows.Forms.TextBox TextBoxMaxX;
        private System.Windows.Forms.TextBox TextBoxMinZ;
        private System.Windows.Forms.TextBox TextBoxMinY;
        private System.Windows.Forms.TextBox TextBoxMinX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
    }
}
