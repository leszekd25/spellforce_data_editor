namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control25
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
            textBox1 = new System.Windows.Forms.TextBox();
            listBox1 = new System.Windows.Forms.ListBox();
            label3 = new System.Windows.Forms.Label();
            textBox3 = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            textBox4 = new System.Windows.Forms.TextBox();
            textBox5 = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            buttonInsert = new System.Windows.Forms.Button();
            buttonRemove = new System.Windows.Forms.Button();
            ListPolygons = new System.Windows.Forms.ListBox();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(48, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 0;
            label1.Text = "Building ID";
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 1;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(122, 243);
            listBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(344, 154);
            listBox1.TabIndex = 4;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(27, 243);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(87, 15);
            label3.TabIndex = 5;
            label3.Text = "Polygon points";
            // 
            // textBox3
            // 
            textBox3.Location = new System.Drawing.Point(122, 405);
            textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(58, 23);
            textBox3.TabIndex = 7;
            textBox3.Leave += textBox3_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(64, 408);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(50, 15);
            label4.TabIndex = 6;
            label4.Text = "Position";
            // 
            // textBox4
            // 
            textBox4.Location = new System.Drawing.Point(211, 405);
            textBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox4.Name = "textBox4";
            textBox4.Size = new System.Drawing.Size(58, 23);
            textBox4.TabIndex = 8;
            textBox4.Leave += textBox4_TextChanged;
            // 
            // textBox5
            // 
            textBox5.Location = new System.Drawing.Point(122, 213);
            textBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox5.Name = "textBox5";
            textBox5.Size = new System.Drawing.Size(44, 23);
            textBox5.TabIndex = 10;
            textBox5.Leave += textBox5_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(23, 217);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(84, 15);
            label5.TabIndex = 9;
            label5.Text = "Casts shadow?";
            // 
            // buttonInsert
            // 
            buttonInsert.Location = new System.Drawing.Point(474, 243);
            buttonInsert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonInsert.Name = "buttonInsert";
            buttonInsert.Size = new System.Drawing.Size(88, 27);
            buttonInsert.TabIndex = 11;
            buttonInsert.Text = "Insert";
            buttonInsert.UseVisualStyleBackColor = true;
            buttonInsert.Click += buttonInsert_Click;
            // 
            // buttonRemove
            // 
            buttonRemove.Location = new System.Drawing.Point(474, 277);
            buttonRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new System.Drawing.Size(88, 27);
            buttonRemove.TabIndex = 12;
            buttonRemove.Text = "Remove";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Click += buttonRemove_Click;
            // 
            // ListPolygons
            // 
            ListPolygons.FormattingEnabled = true;
            ListPolygons.ItemHeight = 15;
            ListPolygons.Location = new System.Drawing.Point(122, 52);
            ListPolygons.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListPolygons.Name = "ListPolygons";
            ListPolygons.Size = new System.Drawing.Size(344, 154);
            ListPolygons.TabIndex = 13;
            ListPolygons.SelectedIndexChanged += ListPolygons_SelectedIndexChanged;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(474, 85);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 27);
            button1.TabIndex = 15;
            button1.Text = "Remove";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(472, 52);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(88, 27);
            button2.TabIndex = 14;
            button2.Text = "Insert";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Control25
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(ListPolygons);
            Controls.Add(buttonRemove);
            Controls.Add(buttonInsert);
            Controls.Add(textBox5);
            Controls.Add(label5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(listBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control25";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonInsert;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.ListBox ListPolygons;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
