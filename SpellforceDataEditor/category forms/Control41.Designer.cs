namespace SpellforceDataEditor.category_forms
{
    partial class Control41
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
            this.tb_sd2 = new System.Windows.Forms.TextBox();
            this.lb_sd2 = new System.Windows.Forms.Label();
            this.tb_sd1 = new System.Windows.Forms.TextBox();
            this.lb_sd1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tb_sd2
            // 
            this.tb_sd2.Location = new System.Drawing.Point(105, 45);
            this.tb_sd2.Name = "tb_sd2";
            this.tb_sd2.Size = new System.Drawing.Size(126, 20);
            this.tb_sd2.TabIndex = 39;
            this.tb_sd2.TextChanged += new System.EventHandler(this.tb_sd2_TextChanged);
            // 
            // lb_sd2
            // 
            this.lb_sd2.AutoSize = true;
            this.lb_sd2.Location = new System.Drawing.Point(57, 48);
            this.lb_sd2.Name = "lb_sd2";
            this.lb_sd2.Size = new System.Drawing.Size(42, 13);
            this.lb_sd2.TabIndex = 38;
            this.lb_sd2.Text = "Text ID";
            // 
            // tb_sd1
            // 
            this.tb_sd1.Location = new System.Drawing.Point(105, 19);
            this.tb_sd1.Name = "tb_sd1";
            this.tb_sd1.Size = new System.Drawing.Size(126, 20);
            this.tb_sd1.TabIndex = 37;
            this.tb_sd1.TextChanged += new System.EventHandler(this.tb_sd1_TextChanged);
            // 
            // lb_sd1
            // 
            this.lb_sd1.AutoSize = true;
            this.lb_sd1.Location = new System.Drawing.Point(25, 22);
            this.lb_sd1.Name = "lb_sd1";
            this.lb_sd1.Size = new System.Drawing.Size(74, 13);
            this.lb_sd1.TabIndex = 36;
            this.lb_sd1.Text = "Description ID";
            // 
            // Control41
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tb_sd2);
            this.Controls.Add(this.lb_sd2);
            this.Controls.Add(this.tb_sd1);
            this.Controls.Add(this.lb_sd1);
            this.Name = "Control41";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_sd2;
        private System.Windows.Forms.Label lb_sd2;
        private System.Windows.Forms.TextBox tb_sd1;
        private System.Windows.Forms.Label lb_sd1;
    }
}
