namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control6
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
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            textBox3 = new System.Windows.Forms.TextBox();
            ListSpells = new System.Windows.Forms.ListBox();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(40, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 15);
            label1.TabIndex = 0;
            label1.Text = "Unit stats ID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(64, 217);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(46, 15);
            label3.TabIndex = 2;
            label3.Text = "Spell ID";
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 3;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // textBox3
            // 
            textBox3.BackColor = System.Drawing.Color.DarkOrange;
            textBox3.Location = new System.Drawing.Point(122, 213);
            textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.ShortcutsEnabled = false;
            textBox3.Size = new System.Drawing.Size(146, 23);
            textBox3.TabIndex = 5;
            textBox3.Leave += textBox3_TextChanged;
            textBox3.MouseDown += textBox3_MouseDown;
            // 
            // ListSpells
            // 
            ListSpells.FormattingEnabled = true;
            ListSpells.ItemHeight = 15;
            ListSpells.Location = new System.Drawing.Point(122, 52);
            ListSpells.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListSpells.Name = "ListSpells";
            ListSpells.Size = new System.Drawing.Size(312, 154);
            ListSpells.TabIndex = 6;
            ListSpells.SelectedIndexChanged += ListSpells_SelectedIndexChanged;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(443, 52);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 27);
            button1.TabIndex = 7;
            button1.Text = "Insert";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(443, 85);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(88, 27);
            button2.TabIndex = 8;
            button2.Text = "Remove";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Control6
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(ListSpells);
            Controls.Add(textBox3);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control6";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ListBox ListSpells;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
