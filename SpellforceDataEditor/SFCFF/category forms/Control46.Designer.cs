namespace SpellforceDataEditor.SFCFF.category_forms
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 48);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Block value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 21;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(105, 71);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(126, 20);
            this.textBox2.TabIndex = 23;
            this.textBox2.Validated += new System.EventHandler(this.textBox2_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 74);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Cultivation flags";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Control46
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
    }
}
