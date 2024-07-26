namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control45
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
            textBox1.TabIndex = 13;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(66, 55);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            label1.Size = new System.Drawing.Size(42, 15);
            label1.TabIndex = 12;
            label1.Text = "Text ID";
            label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_effID
            // 
            tb_effID.Location = new System.Drawing.Point(122, 22);
            tb_effID.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_effID.Name = "tb_effID";
            tb_effID.Size = new System.Drawing.Size(146, 23);
            tb_effID.TabIndex = 11;
            tb_effID.Leave += tb_effID_TextChanged;
            // 
            // lb_effID
            // 
            lb_effID.AutoSize = true;
            lb_effID.Location = new System.Drawing.Point(14, 25);
            lb_effID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lb_effID.Name = "lb_effID";
            lb_effID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            lb_effID.Size = new System.Drawing.Size(97, 15);
            lb_effID.TabIndex = 10;
            lb_effID.Text = "Weapon material";
            lb_effID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Control45
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(tb_effID);
            Controls.Add(lb_effID);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control45";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_effID;
        private System.Windows.Forms.Label lb_effID;
    }
}
