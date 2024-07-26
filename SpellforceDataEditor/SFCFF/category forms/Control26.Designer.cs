namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control26
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
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            textBox3 = new System.Windows.Forms.TextBox();
            textBox1 = new System.Windows.Forms.TextBox();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            ListResources = new System.Windows.Forms.ListBox();
            comboRes = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(65, 248);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(51, 15);
            label3.TabIndex = 45;
            label3.Text = "Amount";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(54, 217);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 15);
            label2.TabIndex = 44;
            label2.Text = "Resource";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(48, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(65, 15);
            label1.TabIndex = 43;
            label1.Text = "Building ID";
            // 
            // textBox3
            // 
            textBox3.Location = new System.Drawing.Point(122, 245);
            textBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(146, 23);
            textBox3.TabIndex = 42;
            textBox3.Leave += textBox3_TextChanged;
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 40;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(443, 85);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(88, 27);
            button2.TabIndex = 48;
            button2.Text = "Remove";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(443, 52);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 27);
            button1.TabIndex = 47;
            button1.Text = "Insert";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ListResources
            // 
            ListResources.FormattingEnabled = true;
            ListResources.ItemHeight = 15;
            ListResources.Location = new System.Drawing.Point(122, 52);
            ListResources.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListResources.Name = "ListResources";
            ListResources.Size = new System.Drawing.Size(313, 154);
            ListResources.TabIndex = 46;
            ListResources.SelectedIndexChanged += ListResources_SelectedIndexChanged;
            // 
            // comboRes
            // 
            comboRes.FormattingEnabled = true;
            comboRes.Location = new System.Drawing.Point(122, 213);
            comboRes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboRes.Name = "comboRes";
            comboRes.Size = new System.Drawing.Size(146, 23);
            comboRes.TabIndex = 49;
            comboRes.SelectedIndexChanged += comboRes_SelectedIndexChanged;
            // 
            // Control26
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(comboRes);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(ListResources);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox3);
            Controls.Add(textBox1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control26";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox ListResources;
        private System.Windows.Forms.ComboBox comboRes;
    }
}
