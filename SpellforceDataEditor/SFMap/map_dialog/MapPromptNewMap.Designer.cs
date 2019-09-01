namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapPromptNewMap
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
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.CheckGenerateTerrain = new System.Windows.Forms.CheckBox();
            this.PanelTerrainGenerator = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.BaseTerrainHeight = new System.Windows.Forms.TextBox();
            this.ErosionBlurStrength = new System.Windows.Forms.TextBox();
            this.ErosionBlurSize = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.ErosionVarianceY = new System.Windows.Forms.TextBox();
            this.ErosionStrengthY = new System.Windows.Forms.TextBox();
            this.ErosionOffsetY = new System.Windows.Forms.TextBox();
            this.ErosionCellSizeY = new System.Windows.Forms.TextBox();
            this.ErosionVarianceX = new System.Windows.Forms.TextBox();
            this.ErosionStrengthX = new System.Windows.Forms.TextBox();
            this.ErosionOffsetX = new System.Windows.Forms.TextBox();
            this.ErosionCellSizeX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PanelTerrainGenerator.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Size";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 253);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(359, 253);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 5;
            this.ButtonOK.Text = "Create";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "256",
            "512",
            "1024"});
            this.comboBox1.Location = new System.Drawing.Point(45, 7);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(75, 21);
            this.comboBox1.TabIndex = 6;
            this.comboBox1.Text = "256";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // CheckGenerateTerrain
            // 
            this.CheckGenerateTerrain.AutoSize = true;
            this.CheckGenerateTerrain.Location = new System.Drawing.Point(18, 34);
            this.CheckGenerateTerrain.Name = "CheckGenerateTerrain";
            this.CheckGenerateTerrain.Size = new System.Drawing.Size(143, 17);
            this.CheckGenerateTerrain.TabIndex = 7;
            this.CheckGenerateTerrain.Text = "Generate base for terrain";
            this.CheckGenerateTerrain.UseVisualStyleBackColor = true;
            this.CheckGenerateTerrain.CheckedChanged += new System.EventHandler(this.CheckGenerateTerrain_CheckedChanged);
            // 
            // PanelTerrainGenerator
            // 
            this.PanelTerrainGenerator.Controls.Add(this.label6);
            this.PanelTerrainGenerator.Controls.Add(this.label7);
            this.PanelTerrainGenerator.Controls.Add(this.label8);
            this.PanelTerrainGenerator.Controls.Add(this.label9);
            this.PanelTerrainGenerator.Controls.Add(this.BaseTerrainHeight);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionBlurStrength);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionBlurSize);
            this.PanelTerrainGenerator.Controls.Add(this.label10);
            this.PanelTerrainGenerator.Controls.Add(this.label11);
            this.PanelTerrainGenerator.Controls.Add(this.label12);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionVarianceY);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionStrengthY);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionOffsetY);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionCellSizeY);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionVarianceX);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionStrengthX);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionOffsetX);
            this.PanelTerrainGenerator.Controls.Add(this.ErosionCellSizeX);
            this.PanelTerrainGenerator.Controls.Add(this.label5);
            this.PanelTerrainGenerator.Controls.Add(this.label4);
            this.PanelTerrainGenerator.Controls.Add(this.label3);
            this.PanelTerrainGenerator.Controls.Add(this.label2);
            this.PanelTerrainGenerator.Enabled = false;
            this.PanelTerrainGenerator.Location = new System.Drawing.Point(12, 57);
            this.PanelTerrainGenerator.Name = "PanelTerrainGenerator";
            this.PanelTerrainGenerator.Size = new System.Drawing.Size(422, 190);
            this.PanelTerrainGenerator.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(211, 84);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "Erosion variance Y";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(211, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "Erosion strength Y";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(211, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Erosion offset Y";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(211, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 13);
            this.label9.TabIndex = 31;
            this.label9.Text = "Erosion cell size Y";
            // 
            // BaseTerrainHeight
            // 
            this.BaseTerrainHeight.Location = new System.Drawing.Point(105, 167);
            this.BaseTerrainHeight.Name = "BaseTerrainHeight";
            this.BaseTerrainHeight.Size = new System.Drawing.Size(100, 20);
            this.BaseTerrainHeight.TabIndex = 30;
            this.BaseTerrainHeight.Text = "2000";
            // 
            // ErosionBlurStrength
            // 
            this.ErosionBlurStrength.Location = new System.Drawing.Point(105, 133);
            this.ErosionBlurStrength.Name = "ErosionBlurStrength";
            this.ErosionBlurStrength.Size = new System.Drawing.Size(100, 20);
            this.ErosionBlurStrength.TabIndex = 29;
            this.ErosionBlurStrength.Text = "1.0";
            // 
            // ErosionBlurSize
            // 
            this.ErosionBlurSize.Location = new System.Drawing.Point(105, 107);
            this.ErosionBlurSize.Name = "ErosionBlurSize";
            this.ErosionBlurSize.Size = new System.Drawing.Size(100, 20);
            this.ErosionBlurSize.TabIndex = 28;
            this.ErosionBlurSize.Text = "1";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 170);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 13);
            this.label10.TabIndex = 27;
            this.label10.Text = "Base terrain height";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Erosion blur strength";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 110);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(83, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Erosion blur size";
            // 
            // ErosionVarianceY
            // 
            this.ErosionVarianceY.Location = new System.Drawing.Point(319, 81);
            this.ErosionVarianceY.Name = "ErosionVarianceY";
            this.ErosionVarianceY.Size = new System.Drawing.Size(100, 20);
            this.ErosionVarianceY.TabIndex = 24;
            this.ErosionVarianceY.Text = "1.0";
            // 
            // ErosionStrengthY
            // 
            this.ErosionStrengthY.Location = new System.Drawing.Point(319, 55);
            this.ErosionStrengthY.Name = "ErosionStrengthY";
            this.ErosionStrengthY.Size = new System.Drawing.Size(100, 20);
            this.ErosionStrengthY.TabIndex = 23;
            this.ErosionStrengthY.Text = "3.0";
            // 
            // ErosionOffsetY
            // 
            this.ErosionOffsetY.Location = new System.Drawing.Point(319, 29);
            this.ErosionOffsetY.Name = "ErosionOffsetY";
            this.ErosionOffsetY.Size = new System.Drawing.Size(100, 20);
            this.ErosionOffsetY.TabIndex = 22;
            this.ErosionOffsetY.Text = "1";
            // 
            // ErosionCellSizeY
            // 
            this.ErosionCellSizeY.Location = new System.Drawing.Point(319, 3);
            this.ErosionCellSizeY.Name = "ErosionCellSizeY";
            this.ErosionCellSizeY.Size = new System.Drawing.Size(100, 20);
            this.ErosionCellSizeY.TabIndex = 21;
            this.ErosionCellSizeY.Text = "16";
            // 
            // ErosionVarianceX
            // 
            this.ErosionVarianceX.Location = new System.Drawing.Point(105, 81);
            this.ErosionVarianceX.Name = "ErosionVarianceX";
            this.ErosionVarianceX.Size = new System.Drawing.Size(100, 20);
            this.ErosionVarianceX.TabIndex = 20;
            this.ErosionVarianceX.Text = "1.0";
            // 
            // ErosionStrengthX
            // 
            this.ErosionStrengthX.Location = new System.Drawing.Point(105, 55);
            this.ErosionStrengthX.Name = "ErosionStrengthX";
            this.ErosionStrengthX.Size = new System.Drawing.Size(100, 20);
            this.ErosionStrengthX.TabIndex = 19;
            this.ErosionStrengthX.Text = "3.0";
            // 
            // ErosionOffsetX
            // 
            this.ErosionOffsetX.Location = new System.Drawing.Point(105, 29);
            this.ErosionOffsetX.Name = "ErosionOffsetX";
            this.ErosionOffsetX.Size = new System.Drawing.Size(100, 20);
            this.ErosionOffsetX.TabIndex = 18;
            this.ErosionOffsetX.Text = "1";
            // 
            // ErosionCellSizeX
            // 
            this.ErosionCellSizeX.Location = new System.Drawing.Point(105, 3);
            this.ErosionCellSizeX.Name = "ErosionCellSizeX";
            this.ErosionCellSizeX.Size = new System.Drawing.Size(100, 20);
            this.ErosionCellSizeX.TabIndex = 17;
            this.ErosionCellSizeX.Text = "16";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Erosion variance X";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Erosion strength X";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Erosion offset X";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Erosion cell size X";
            // 
            // MapPromptNewMap
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(446, 288);
            this.Controls.Add(this.PanelTerrainGenerator);
            this.Controls.Add(this.CheckGenerateTerrain);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapPromptNewMap";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "New map";
            this.PanelTerrainGenerator.ResumeLayout(false);
            this.PanelTerrainGenerator.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox CheckGenerateTerrain;
        private System.Windows.Forms.Panel PanelTerrainGenerator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox BaseTerrainHeight;
        private System.Windows.Forms.TextBox ErosionBlurStrength;
        private System.Windows.Forms.TextBox ErosionBlurSize;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox ErosionVarianceY;
        private System.Windows.Forms.TextBox ErosionStrengthY;
        private System.Windows.Forms.TextBox ErosionOffsetY;
        private System.Windows.Forms.TextBox ErosionCellSizeY;
        private System.Windows.Forms.TextBox ErosionVarianceX;
        private System.Windows.Forms.TextBox ErosionStrengthX;
        private System.Windows.Forms.TextBox ErosionOffsetX;
        private System.Windows.Forms.TextBox ErosionCellSizeX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}