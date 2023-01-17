namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapLakeInspector
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.LakeTypeIce = new System.Windows.Forms.RadioButton();
            this.LakeTypeSwamp = new System.Windows.Forms.RadioButton();
            this.LakeTypeLava = new System.Windows.Forms.RadioButton();
            this.LakeTypeWater = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectedLakeDepth = new System.Windows.Forms.TextBox();
            this.SelectedLakeLevel = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Lake type";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LakeTypeIce);
            this.panel1.Controls.Add(this.LakeTypeSwamp);
            this.panel1.Controls.Add(this.LakeTypeLava);
            this.panel1.Controls.Add(this.LakeTypeWater);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(177, 92);
            this.panel1.TabIndex = 2;
            // 
            // LakeTypeIce
            // 
            this.LakeTypeIce.AutoSize = true;
            this.LakeTypeIce.Location = new System.Drawing.Point(104, 72);
            this.LakeTypeIce.Name = "LakeTypeIce";
            this.LakeTypeIce.Size = new System.Drawing.Size(40, 17);
            this.LakeTypeIce.TabIndex = 5;
            this.LakeTypeIce.TabStop = true;
            this.LakeTypeIce.Text = "Ice";
            this.LakeTypeIce.UseVisualStyleBackColor = true;
            this.LakeTypeIce.Click += new System.EventHandler(this.LakeTypeIce_Click);
            // 
            // LakeTypeSwamp
            // 
            this.LakeTypeSwamp.AutoSize = true;
            this.LakeTypeSwamp.Location = new System.Drawing.Point(104, 26);
            this.LakeTypeSwamp.Name = "LakeTypeSwamp";
            this.LakeTypeSwamp.Size = new System.Drawing.Size(60, 17);
            this.LakeTypeSwamp.TabIndex = 4;
            this.LakeTypeSwamp.TabStop = true;
            this.LakeTypeSwamp.Text = "Swamp";
            this.LakeTypeSwamp.UseVisualStyleBackColor = true;
            this.LakeTypeSwamp.Click += new System.EventHandler(this.LakeTypeSwamp_Click);
            // 
            // LakeTypeLava
            // 
            this.LakeTypeLava.AutoSize = true;
            this.LakeTypeLava.Location = new System.Drawing.Point(104, 49);
            this.LakeTypeLava.Name = "LakeTypeLava";
            this.LakeTypeLava.Size = new System.Drawing.Size(49, 17);
            this.LakeTypeLava.TabIndex = 3;
            this.LakeTypeLava.TabStop = true;
            this.LakeTypeLava.Text = "Lava";
            this.LakeTypeLava.UseVisualStyleBackColor = true;
            this.LakeTypeLava.Click += new System.EventHandler(this.LakeTypeLava_Click);
            // 
            // LakeTypeWater
            // 
            this.LakeTypeWater.AutoSize = true;
            this.LakeTypeWater.Location = new System.Drawing.Point(104, 3);
            this.LakeTypeWater.Name = "LakeTypeWater";
            this.LakeTypeWater.Size = new System.Drawing.Size(54, 17);
            this.LakeTypeWater.TabIndex = 2;
            this.LakeTypeWater.TabStop = true;
            this.LakeTypeWater.Text = "Water";
            this.LakeTypeWater.UseVisualStyleBackColor = true;
            this.LakeTypeWater.Click += new System.EventHandler(this.LakeTypeWater_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lake depth";
            // 
            // SelectedLakeDepth
            // 
            this.SelectedLakeDepth.Location = new System.Drawing.Point(107, 97);
            this.SelectedLakeDepth.Name = "SelectedLakeDepth";
            this.SelectedLakeDepth.Size = new System.Drawing.Size(73, 20);
            this.SelectedLakeDepth.TabIndex = 4;
            this.SelectedLakeDepth.Validated += new System.EventHandler(this.SelectedLakeDepth_Leave);
            // 
            // SelectedLakeLevel
            // 
            this.SelectedLakeLevel.Location = new System.Drawing.Point(107, 123);
            this.SelectedLakeLevel.Name = "SelectedLakeLevel";
            this.SelectedLakeLevel.Size = new System.Drawing.Size(73, 20);
            this.SelectedLakeLevel.TabIndex = 6;
            this.SelectedLakeLevel.Validated += new System.EventHandler(this.SelectedLakeLevel_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lake level";
            // 
            // MapLakeInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.SelectedLakeLevel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SelectedLakeDepth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Enabled = false;
            this.Name = "MapLakeInspector";
            this.Size = new System.Drawing.Size(183, 162);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton LakeTypeIce;
        private System.Windows.Forms.RadioButton LakeTypeSwamp;
        private System.Windows.Forms.RadioButton LakeTypeLava;
        private System.Windows.Forms.RadioButton LakeTypeWater;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SelectedLakeDepth;
        private System.Windows.Forms.TextBox SelectedLakeLevel;
        private System.Windows.Forms.Label label3;
    }
}
