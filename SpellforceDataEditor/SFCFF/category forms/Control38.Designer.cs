namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control38
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
            tb_effID = new System.Windows.Forms.TextBox();
            lb_effID = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBox2 = new System.Windows.Forms.TextBox();
            textBox3 = new System.Windows.Forms.TextBox();
            textBox4 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // tb_effID
            // 
            tb_effID.Location = new System.Drawing.Point(122, 22);
            tb_effID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_effID.Name = "tb_effID";
            tb_effID.Size = new System.Drawing.Size(146, 23);
            tb_effID.TabIndex = 3;
            tb_effID.Leave += tb_effID_TextChanged;
            // 
            // lb_effID
            // 
            lb_effID.AutoSize = true;
            lb_effID.Location = new System.Drawing.Point(66, 25);
            lb_effID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_effID.Name = "lb_effID";
            lb_effID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            lb_effID.Size = new System.Drawing.Size(45, 15);
            lb_effID.TabIndex = 2;
            lb_effID.Text = "Map ID";
            lb_effID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(35, 55);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label1.Size = new System.Drawing.Size(74, 15);
            label1.TabIndex = 4;
            label1.Text = "Is persistent?";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox2
            // 
            textBox2.BackColor = System.Drawing.Color.DarkOrange;
            textBox2.Location = new System.Drawing.Point(122, 112);
            textBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.ShortcutsEnabled = false;
            textBox2.Size = new System.Drawing.Size(146, 23);
            textBox2.TabIndex = 6;
            textBox2.Leave += textBox2_TextChanged;
            textBox2.MouseDown += textBox2_MouseDown;
            // 
            // textBox3
            // 
            textBox3.Location = new System.Drawing.Point(122, 52);
            textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(44, 23);
            textBox3.TabIndex = 7;
            textBox3.Leave += textBox3_TextChanged;
            // 
            // textBox4
            // 
            textBox4.Location = new System.Drawing.Point(122, 82);
            textBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox4.MaxLength = 64;
            textBox4.Name = "textBox4";
            textBox4.Size = new System.Drawing.Size(446, 23);
            textBox4.TabIndex = 9;
            textBox4.Leave += textBox4_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(42, 85);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label2.Size = new System.Drawing.Size(70, 15);
            label2.TabIndex = 8;
            label2.Text = "Map handle";
            label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(58, 115);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label3.Size = new System.Drawing.Size(53, 15);
            label3.TabIndex = 11;
            label3.Text = "Name ID";
            label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Control38
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(label3);
            Controls.Add(textBox4);
            Controls.Add(label2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(label1);
            Controls.Add(tb_effID);
            Controls.Add(lb_effID);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control38";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox tb_effID;
        private System.Windows.Forms.Label lb_effID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
