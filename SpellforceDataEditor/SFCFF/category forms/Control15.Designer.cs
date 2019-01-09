namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control15
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.DomainLanguages = new System.Windows.Forms.DomainUpDown();
            this.ButtonAddLang = new System.Windows.Forms.Button();
            this.ButtonRemoveLang = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Validated += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(362, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(126, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.Validated += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(105, 96);
            this.textBox4.MaxLength = 50;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(383, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Validated += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(105, 122);
            this.textBox5.MaxLength = 512;
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(383, 100);
            this.textBox5.TabIndex = 4;
            this.textBox5.Validated += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Text ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Language ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(299, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Text mode";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Text handle";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Text content";
            // 
            // DomainLanguages
            // 
            this.DomainLanguages.Location = new System.Drawing.Point(362, 19);
            this.DomainLanguages.Name = "DomainLanguages";
            this.DomainLanguages.Size = new System.Drawing.Size(126, 20);
            this.DomainLanguages.TabIndex = 10;
            this.DomainLanguages.Text = "0";
            this.DomainLanguages.SelectedItemChanged += new System.EventHandler(this.DomainLanguages_SelectedItemChanged);
            // 
            // ButtonAddLang
            // 
            this.ButtonAddLang.Location = new System.Drawing.Point(362, 41);
            this.ButtonAddLang.Name = "ButtonAddLang";
            this.ButtonAddLang.Size = new System.Drawing.Size(59, 23);
            this.ButtonAddLang.TabIndex = 11;
            this.ButtonAddLang.Text = "Add";
            this.ButtonAddLang.UseVisualStyleBackColor = true;
            this.ButtonAddLang.Click += new System.EventHandler(this.ButtonAddLang_Click);
            // 
            // ButtonRemoveLang
            // 
            this.ButtonRemoveLang.Location = new System.Drawing.Point(429, 41);
            this.ButtonRemoveLang.Name = "ButtonRemoveLang";
            this.ButtonRemoveLang.Size = new System.Drawing.Size(59, 23);
            this.ButtonRemoveLang.TabIndex = 12;
            this.ButtonRemoveLang.Text = "Remove";
            this.ButtonRemoveLang.UseVisualStyleBackColor = true;
            this.ButtonRemoveLang.Click += new System.EventHandler(this.ButtonRemoveLang_Click);
            // 
            // Control15
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonRemoveLang);
            this.Controls.Add(this.ButtonAddLang);
            this.Controls.Add(this.DomainLanguages);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox1);
            this.Name = "Control15";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DomainUpDown DomainLanguages;
        private System.Windows.Forms.Button ButtonAddLang;
        private System.Windows.Forms.Button ButtonRemoveLang;
    }
}
