namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control49
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
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            tb_effID = new System.Windows.Forms.TextBox();
            lb_effID = new System.Windows.Forms.Label();
            textBox2 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 52);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 17;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(29, 55);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label1.Size = new System.Drawing.Size(81, 15);
            label1.TabIndex = 16;
            label1.Text = "Description ID";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_effID
            // 
            tb_effID.Location = new System.Drawing.Point(122, 22);
            tb_effID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_effID.Name = "tb_effID";
            tb_effID.Size = new System.Drawing.Size(146, 23);
            tb_effID.TabIndex = 15;
            tb_effID.Leave += tb_effID_TextChanged;
            // 
            // lb_effID
            // 
            lb_effID.AutoSize = true;
            lb_effID.Location = new System.Drawing.Point(72, 25);
            lb_effID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_effID.Name = "lb_effID";
            lb_effID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            lb_effID.Size = new System.Drawing.Size(37, 15);
            lb_effID.TabIndex = 14;
            lb_effID.Text = "Set ID";
            lb_effID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(122, 82);
            textBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(146, 23);
            textBox2.TabIndex = 19;
            textBox2.Leave += textBox2_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(62, 85);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label2.Size = new System.Drawing.Size(49, 15);
            label2.TabIndex = 18;
            label2.Text = "Set type";
            label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Control49
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(tb_effID);
            Controls.Add(lb_effID);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control49";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_effID;
        private System.Windows.Forms.Label lb_effID;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
    }
}
