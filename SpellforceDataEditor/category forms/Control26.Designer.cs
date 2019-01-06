namespace SpellforceDataEditor.category_forms
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ListResources = new System.Windows.Forms.ListBox();
            this.comboRes = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 45;
            this.label3.Text = "Amount";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 44;
            this.label2.Text = "Resource";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 43;
            this.label1.Text = "Building ID";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(105, 212);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(126, 20);
            this.textBox3.TabIndex = 42;
            this.textBox3.Validated += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.DarkOrange;
            this.textBox1.Location = new System.Drawing.Point(105, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 40;
            this.textBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox1_MouseDown);
            this.textBox1.Validated += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(380, 74);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 48;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(380, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 47;
            this.button1.Text = "Insert";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ListResources
            // 
            this.ListResources.FormattingEnabled = true;
            this.ListResources.Location = new System.Drawing.Point(105, 45);
            this.ListResources.Name = "ListResources";
            this.ListResources.Size = new System.Drawing.Size(269, 134);
            this.ListResources.TabIndex = 46;
            this.ListResources.SelectedIndexChanged += new System.EventHandler(this.ListResources_SelectedIndexChanged);
            // 
            // comboRes
            // 
            this.comboRes.FormattingEnabled = true;
            this.comboRes.Location = new System.Drawing.Point(105, 185);
            this.comboRes.Name = "comboRes";
            this.comboRes.Size = new System.Drawing.Size(126, 21);
            this.comboRes.TabIndex = 49;
            this.comboRes.SelectedIndexChanged += new System.EventHandler(this.comboRes_SelectedIndexChanged);
            // 
            // Control26
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboRes);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ListResources);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox1);
            this.Name = "Control26";
            this.ResumeLayout(false);
            this.PerformLayout();

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
