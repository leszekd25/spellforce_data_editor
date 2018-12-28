namespace SpellforceDataEditor.category_forms
{
    partial class Control46
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_effID = new System.Windows.Forms.TextBox();
            this.lb_effID = new System.Windows.Forms.Label();
            this.flagDepthWrite = new System.Windows.Forms.CheckBox();
            this.flagDepthReadOn = new System.Windows.Forms.CheckBox();
            this.flagCulling = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tb_effID
            // 
            this.tb_effID.Location = new System.Drawing.Point(105, 19);
            this.tb_effID.Name = "tb_effID";
            this.tb_effID.Size = new System.Drawing.Size(126, 20);
            this.tb_effID.TabIndex = 15;
            this.tb_effID.Validated += new System.EventHandler(this.tb_effID_TextChanged);
            // 
            // lb_effID
            // 
            this.lb_effID.AutoSize = true;
            this.lb_effID.Location = new System.Drawing.Point(45, 22);
            this.lb_effID.Name = "lb_effID";
            this.lb_effID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lb_effID.Size = new System.Drawing.Size(54, 13);
            this.lb_effID.TabIndex = 14;
            this.lb_effID.Text = "Terrain ID";
            this.lb_effID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // flagDepthWrite
            // 
            this.flagDepthWrite.AutoSize = true;
            this.flagDepthWrite.Location = new System.Drawing.Point(25, 71);
            this.flagDepthWrite.Name = "flagDepthWrite";
            this.flagDepthWrite.Size = new System.Drawing.Size(123, 17);
            this.flagDepthWrite.TabIndex = 18;
            this.flagDepthWrite.Text = "Write to depth buffer";
            this.flagDepthWrite.UseVisualStyleBackColor = true;
            this.flagDepthWrite.CheckedChanged += new System.EventHandler(this.flagDepthWrite_CheckedChanged);
            // 
            // flagDepthReadOn
            // 
            this.flagDepthReadOn.AutoSize = true;
            this.flagDepthReadOn.Location = new System.Drawing.Point(25, 94);
            this.flagDepthReadOn.Name = "flagDepthReadOn";
            this.flagDepthReadOn.Size = new System.Drawing.Size(183, 17);
            this.flagDepthReadOn.TabIndex = 19;
            this.flagDepthReadOn.Text = "Read from depth buffer on render";
            this.flagDepthReadOn.UseVisualStyleBackColor = true;
            this.flagDepthReadOn.CheckedChanged += new System.EventHandler(this.flagDepthReadOn_CheckedChanged);
            // 
            // flagCulling
            // 
            this.flagCulling.AutoSize = true;
            this.flagCulling.Location = new System.Drawing.Point(25, 117);
            this.flagCulling.Name = "flagCulling";
            this.flagCulling.Size = new System.Drawing.Size(96, 17);
            this.flagCulling.TabIndex = 20;
            this.flagCulling.Text = "Surface culling";
            this.flagCulling.UseVisualStyleBackColor = true;
            this.flagCulling.CheckedChanged += new System.EventHandler(this.flagCulling_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 48);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Unknown (?)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 21;
            // 
            // Control46
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.flagCulling);
            this.Controls.Add(this.flagDepthReadOn);
            this.Controls.Add(this.flagDepthWrite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_effID);
            this.Controls.Add(this.lb_effID);
            this.Name = "Control46";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tb_effID;
        private System.Windows.Forms.Label lb_effID;
        private System.Windows.Forms.CheckBox flagDepthWrite;
        private System.Windows.Forms.CheckBox flagDepthReadOn;
        private System.Windows.Forms.CheckBox flagCulling;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}
