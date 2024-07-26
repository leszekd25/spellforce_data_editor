namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control29
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
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBox3 = new System.Windows.Forms.TextBox();
            textBox1 = new System.Windows.Forms.TextBox();
            ButtonGoto30 = new System.Windows.Forms.Button();
            ButtonGoto31 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(69, 52);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(43, 15);
            label3.TabIndex = 55;
            label3.Text = "Unit ID";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(38, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 15);
            label1.TabIndex = 54;
            label1.Text = "Merchant ID";
            // 
            // textBox3
            // 
            textBox3.BackColor = System.Drawing.Color.DarkOrange;
            textBox3.Location = new System.Drawing.Point(122, 52);
            textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.ShortcutsEnabled = false;
            textBox3.Size = new System.Drawing.Size(146, 23);
            textBox3.TabIndex = 53;
            textBox3.Leave += textBox3_TextChanged;
            textBox3.MouseDown += textBox3_MouseDown;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 52;
            textBox1.Leave += textBox1_TextChanged;
            // 
            // ButtonGoto30
            // 
            ButtonGoto30.Location = new System.Drawing.Point(4, 82);
            ButtonGoto30.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonGoto30.Name = "ButtonGoto30";
            ButtonGoto30.Size = new System.Drawing.Size(266, 27);
            ButtonGoto30.TabIndex = 56;
            ButtonGoto30.Text = "button1";
            ButtonGoto30.UseVisualStyleBackColor = true;
            ButtonGoto30.Click += ButtonGoto30_Click;
            // 
            // ButtonGoto31
            // 
            ButtonGoto31.Location = new System.Drawing.Point(4, 115);
            ButtonGoto31.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonGoto31.Name = "ButtonGoto31";
            ButtonGoto31.Size = new System.Drawing.Size(266, 27);
            ButtonGoto31.TabIndex = 57;
            ButtonGoto31.Text = "button2";
            ButtonGoto31.UseVisualStyleBackColor = true;
            ButtonGoto31.Click += ButtonGoto31_Click;
            // 
            // Control29
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(ButtonGoto31);
            Controls.Add(ButtonGoto30);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(textBox3);
            Controls.Add(textBox1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control29";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button ButtonGoto30;
        private System.Windows.Forms.Button ButtonGoto31;
    }
}
