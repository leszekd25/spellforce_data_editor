namespace SpellforceDataEditor.SFEffect.effect_controls
{
    partial class ParticleRotationControl
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
            this.textBox11 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.Label();
            this.ComboAxis = new System.Windows.Forms.ComboBox();
            this.TextBoxMin = new System.Windows.Forms.TextBox();
            this.TextBoxAngle = new System.Windows.Forms.TextBox();
            this.TextBoxMax = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelName
            // 
            this.LabelName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.LabelName.Size = new System.Drawing.Size(55, 13);
            this.LabelName.Text = "Rotation";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 256);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 16);
            this.label7.TabIndex = 19;
            this.label7.Text = "Axis";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(289, 256);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 16);
            this.label8.TabIndex = 20;
            this.label8.Text = "Angle";
            // 
            // textBox11
            // 
            this.textBox11.AutoSize = true;
            this.textBox11.Location = new System.Drawing.Point(289, 289);
            this.textBox11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(33, 16);
            this.textBox11.TabIndex = 21;
            this.textBox11.Text = "Max";
            // 
            // textBox10
            // 
            this.textBox10.AutoSize = true;
            this.textBox10.Location = new System.Drawing.Point(4, 289);
            this.textBox10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(29, 16);
            this.textBox10.TabIndex = 22;
            this.textBox10.Text = "Min";
            // 
            // ComboAxis
            // 
            this.ComboAxis.FormattingEnabled = true;
            this.ComboAxis.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z"});
            this.ComboAxis.Location = new System.Drawing.Point(93, 252);
            this.ComboAxis.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboAxis.Name = "ComboAxis";
            this.ComboAxis.Size = new System.Drawing.Size(160, 24);
            this.ComboAxis.TabIndex = 23;
            this.ComboAxis.SelectedIndexChanged += new System.EventHandler(this.ComboAxis_SelectedIndexChanged);
            // 
            // TextBoxMin
            // 
            this.TextBoxMin.Location = new System.Drawing.Point(93, 286);
            this.TextBoxMin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMin.Name = "TextBoxMin";
            this.TextBoxMin.Size = new System.Drawing.Size(160, 22);
            this.TextBoxMin.TabIndex = 24;
            this.TextBoxMin.Validated += new System.EventHandler(this.TextBoxMin_Validated);
            // 
            // TextBoxAngle
            // 
            this.TextBoxAngle.Location = new System.Drawing.Point(377, 252);
            this.TextBoxAngle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxAngle.Name = "TextBoxAngle";
            this.TextBoxAngle.Size = new System.Drawing.Size(151, 22);
            this.TextBoxAngle.TabIndex = 25;
            this.TextBoxAngle.Validated += new System.EventHandler(this.TextBoxAngle_Validated);
            // 
            // TextBoxMax
            // 
            this.TextBoxMax.Location = new System.Drawing.Point(377, 286);
            this.TextBoxMax.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxMax.Name = "TextBoxMax";
            this.TextBoxMax.Size = new System.Drawing.Size(151, 22);
            this.TextBoxMax.TabIndex = 26;
            this.TextBoxMax.Validated += new System.EventHandler(this.TextBoxMax_Validated);
            // 
            // ParticleRotationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Controls.Add(this.TextBoxMax);
            this.Controls.Add(this.TextBoxAngle);
            this.Controls.Add(this.TextBoxMin);
            this.Controls.Add(this.ComboAxis);
            this.Controls.Add(this.textBox10);
            this.Controls.Add(this.textBox11);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "ParticleRotationControl";
            this.Size = new System.Drawing.Size(533, 316);
            this.Controls.SetChildIndex(this.LabelName, 0);
            this.Controls.SetChildIndex(this.label7, 0);
            this.Controls.SetChildIndex(this.label8, 0);
            this.Controls.SetChildIndex(this.textBox11, 0);
            this.Controls.SetChildIndex(this.textBox10, 0);
            this.Controls.SetChildIndex(this.ComboAxis, 0);
            this.Controls.SetChildIndex(this.TextBoxMin, 0);
            this.Controls.SetChildIndex(this.TextBoxAngle, 0);
            this.Controls.SetChildIndex(this.TextBoxMax, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label textBox11;
        private System.Windows.Forms.Label textBox10;
        private System.Windows.Forms.ComboBox ComboAxis;
        private System.Windows.Forms.TextBox TextBoxMin;
        private System.Windows.Forms.TextBox TextBoxAngle;
        private System.Windows.Forms.TextBox TextBoxMax;
    }
}
