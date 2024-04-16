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
            label1 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            LakeTypeIce = new System.Windows.Forms.RadioButton();
            LakeTypeSwamp = new System.Windows.Forms.RadioButton();
            LakeTypeLava = new System.Windows.Forms.RadioButton();
            LakeTypeWater = new System.Windows.Forms.RadioButton();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 6);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(57, 15);
            label1.TabIndex = 1;
            label1.Text = "Lake type";
            // 
            // panel1
            // 
            panel1.Controls.Add(LakeTypeIce);
            panel1.Controls.Add(LakeTypeSwamp);
            panel1.Controls.Add(LakeTypeLava);
            panel1.Controls.Add(LakeTypeWater);
            panel1.Controls.Add(label1);
            panel1.Location = new System.Drawing.Point(4, 3);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(206, 106);
            panel1.TabIndex = 2;
            // 
            // LakeTypeIce
            // 
            LakeTypeIce.AutoSize = true;
            LakeTypeIce.Location = new System.Drawing.Point(121, 83);
            LakeTypeIce.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LakeTypeIce.Name = "LakeTypeIce";
            LakeTypeIce.Size = new System.Drawing.Size(40, 19);
            LakeTypeIce.TabIndex = 5;
            LakeTypeIce.TabStop = true;
            LakeTypeIce.Text = "Ice";
            LakeTypeIce.UseVisualStyleBackColor = true;
            LakeTypeIce.Click += LakeTypeIce_Click;
            // 
            // LakeTypeSwamp
            // 
            LakeTypeSwamp.AutoSize = true;
            LakeTypeSwamp.Location = new System.Drawing.Point(121, 30);
            LakeTypeSwamp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LakeTypeSwamp.Name = "LakeTypeSwamp";
            LakeTypeSwamp.Size = new System.Drawing.Size(64, 19);
            LakeTypeSwamp.TabIndex = 4;
            LakeTypeSwamp.TabStop = true;
            LakeTypeSwamp.Text = "Swamp";
            LakeTypeSwamp.UseVisualStyleBackColor = true;
            LakeTypeSwamp.Click += LakeTypeSwamp_Click;
            // 
            // LakeTypeLava
            // 
            LakeTypeLava.AutoSize = true;
            LakeTypeLava.Location = new System.Drawing.Point(121, 57);
            LakeTypeLava.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LakeTypeLava.Name = "LakeTypeLava";
            LakeTypeLava.Size = new System.Drawing.Size(49, 19);
            LakeTypeLava.TabIndex = 3;
            LakeTypeLava.TabStop = true;
            LakeTypeLava.Text = "Lava";
            LakeTypeLava.UseVisualStyleBackColor = true;
            LakeTypeLava.Click += LakeTypeLava_Click;
            // 
            // LakeTypeWater
            // 
            LakeTypeWater.AutoSize = true;
            LakeTypeWater.Location = new System.Drawing.Point(121, 3);
            LakeTypeWater.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            LakeTypeWater.Name = "LakeTypeWater";
            LakeTypeWater.Size = new System.Drawing.Size(56, 19);
            LakeTypeWater.TabIndex = 2;
            LakeTypeWater.TabStop = true;
            LakeTypeWater.Text = "Water";
            LakeTypeWater.UseVisualStyleBackColor = true;
            LakeTypeWater.Click += LakeTypeWater_Click;
            // 
            // MapLakeInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(panel1);
            Enabled = false;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapLakeInspector";
            Size = new System.Drawing.Size(214, 187);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton LakeTypeIce;
        private System.Windows.Forms.RadioButton LakeTypeSwamp;
        private System.Windows.Forms.RadioButton LakeTypeLava;
        private System.Windows.Forms.RadioButton LakeTypeWater;
    }
}
