namespace SpellforceDataEditor.SFCFF.category_forms
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
            tb_sd2 = new System.Windows.Forms.TextBox();
            lb_sd2 = new System.Windows.Forms.Label();
            tb_sd1 = new System.Windows.Forms.TextBox();
            lb_sd1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // tb_sd2
            // 
            tb_sd2.BackColor = System.Drawing.Color.DarkOrange;
            tb_sd2.Location = new System.Drawing.Point(122, 52);
            tb_sd2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_sd2.Name = "tb_sd2";
            tb_sd2.ShortcutsEnabled = false;
            tb_sd2.Size = new System.Drawing.Size(146, 23);
            tb_sd2.TabIndex = 39;
            tb_sd2.Leave += tb_sd2_TextChanged;
            tb_sd2.MouseDown += tb_sd2_MouseDown;
            // 
            // lb_sd2
            // 
            lb_sd2.AutoSize = true;
            lb_sd2.Location = new System.Drawing.Point(66, 55);
            lb_sd2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_sd2.Name = "lb_sd2";
            lb_sd2.Size = new System.Drawing.Size(42, 15);
            lb_sd2.TabIndex = 38;
            lb_sd2.Text = "Text ID";
            // 
            // tb_sd1
            // 
            tb_sd1.Location = new System.Drawing.Point(122, 22);
            tb_sd1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_sd1.Name = "tb_sd1";
            tb_sd1.Size = new System.Drawing.Size(146, 23);
            tb_sd1.TabIndex = 37;
            tb_sd1.Leave += tb_sd1_TextChanged;
            // 
            // lb_sd1
            // 
            lb_sd1.AutoSize = true;
            lb_sd1.Location = new System.Drawing.Point(29, 25);
            lb_sd1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_sd1.Name = "lb_sd1";
            lb_sd1.Size = new System.Drawing.Size(81, 15);
            lb_sd1.TabIndex = 36;
            lb_sd1.Text = "Description ID";
            // 
            // Control41
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tb_sd2);
            Controls.Add(lb_sd2);
            Controls.Add(tb_sd1);
            Controls.Add(lb_sd1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control41";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tb_sd2;
        private System.Windows.Forms.Label lb_sd2;
        private System.Windows.Forms.TextBox tb_sd1;
        private System.Windows.Forms.Label lb_sd1;
    }
}
