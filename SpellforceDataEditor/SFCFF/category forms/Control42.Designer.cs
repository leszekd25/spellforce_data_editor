namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control42
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
            sb_sd5 = new System.Windows.Forms.TextBox();
            lb_sd5 = new System.Windows.Forms.Label();
            tb_sd4 = new System.Windows.Forms.TextBox();
            lb_sd4 = new System.Windows.Forms.Label();
            tb_sd3 = new System.Windows.Forms.TextBox();
            lb_sd3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // sb_sd5
            // 
            sb_sd5.BackColor = System.Drawing.Color.DarkOrange;
            sb_sd5.Location = new System.Drawing.Point(122, 82);
            sb_sd5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            sb_sd5.Name = "sb_sd5";
            sb_sd5.ShortcutsEnabled = false;
            sb_sd5.Size = new System.Drawing.Size(146, 23);
            sb_sd5.TabIndex = 53;
            sb_sd5.Leave += sb_sd5_TextChanged;
            sb_sd5.MouseDown += sb_sd5_MouseDown;
            // 
            // lb_sd5
            // 
            lb_sd5.AutoSize = true;
            lb_sd5.Location = new System.Drawing.Point(10, 85);
            lb_sd5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_sd5.Name = "lb_sd5";
            lb_sd5.Size = new System.Drawing.Size(97, 15);
            lb_sd5.TabIndex = 52;
            lb_sd5.Text = "Advanced text ID";
            // 
            // tb_sd4
            // 
            tb_sd4.BackColor = System.Drawing.Color.DarkOrange;
            tb_sd4.Location = new System.Drawing.Point(122, 52);
            tb_sd4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_sd4.Name = "tb_sd4";
            tb_sd4.ShortcutsEnabled = false;
            tb_sd4.Size = new System.Drawing.Size(146, 23);
            tb_sd4.TabIndex = 51;
            tb_sd4.Leave += tb_sd4_TextChanged;
            tb_sd4.MouseDown += tb_sd4_MouseDown;
            // 
            // lb_sd4
            // 
            lb_sd4.AutoSize = true;
            lb_sd4.Location = new System.Drawing.Point(66, 55);
            lb_sd4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_sd4.Name = "lb_sd4";
            lb_sd4.Size = new System.Drawing.Size(42, 15);
            lb_sd4.TabIndex = 50;
            lb_sd4.Text = "Text ID";
            // 
            // tb_sd3
            // 
            tb_sd3.Location = new System.Drawing.Point(122, 22);
            tb_sd3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_sd3.Name = "tb_sd3";
            tb_sd3.Size = new System.Drawing.Size(146, 23);
            tb_sd3.TabIndex = 49;
            tb_sd3.Leave += tb_sd3_TextChanged;
            // 
            // lb_sd3
            // 
            lb_sd3.AutoSize = true;
            lb_sd3.Location = new System.Drawing.Point(29, 25);
            lb_sd3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_sd3.Name = "lb_sd3";
            lb_sd3.Size = new System.Drawing.Size(81, 15);
            lb_sd3.TabIndex = 48;
            lb_sd3.Text = "Description ID";
            // 
            // Control42
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(sb_sd5);
            Controls.Add(lb_sd5);
            Controls.Add(tb_sd4);
            Controls.Add(lb_sd4);
            Controls.Add(tb_sd3);
            Controls.Add(lb_sd3);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control42";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox sb_sd5;
        private System.Windows.Forms.Label lb_sd5;
        private System.Windows.Forms.TextBox tb_sd4;
        private System.Windows.Forms.Label lb_sd4;
        private System.Windows.Forms.TextBox tb_sd3;
        private System.Windows.Forms.Label lb_sd3;
    }
}
