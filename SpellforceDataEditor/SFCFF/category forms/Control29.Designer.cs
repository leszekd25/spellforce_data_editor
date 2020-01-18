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
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ButtonGoto30 = new System.Windows.Forms.Button();
            this.ButtonGoto31 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 55;
            this.label3.Text = "Unit ID";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "Merchant ID";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.DarkOrange;
            this.textBox3.Location = new System.Drawing.Point(105, 45);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(126, 20);
            this.textBox3.TabIndex = 53;
            this.textBox3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textBox3_MouseDown);
            this.textBox3.Validated += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 52;
            this.textBox1.Validated += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ButtonGoto30
            // 
            this.ButtonGoto30.Location = new System.Drawing.Point(3, 71);
            this.ButtonGoto30.Name = "ButtonGoto30";
            this.ButtonGoto30.Size = new System.Drawing.Size(228, 23);
            this.ButtonGoto30.TabIndex = 56;
            this.ButtonGoto30.Text = "button1";
            this.ButtonGoto30.UseVisualStyleBackColor = true;
            this.ButtonGoto30.Click += new System.EventHandler(this.ButtonGoto30_Click);
            // 
            // ButtonGoto31
            // 
            this.ButtonGoto31.Location = new System.Drawing.Point(3, 100);
            this.ButtonGoto31.Name = "ButtonGoto31";
            this.ButtonGoto31.Size = new System.Drawing.Size(228, 23);
            this.ButtonGoto31.TabIndex = 57;
            this.ButtonGoto31.Text = "button2";
            this.ButtonGoto31.UseVisualStyleBackColor = true;
            this.ButtonGoto31.Click += new System.EventHandler(this.ButtonGoto31_Click);
            // 
            // Control29
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ButtonGoto31);
            this.Controls.Add(this.ButtonGoto30);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox1);
            this.Name = "Control29";
            this.ResumeLayout(false);
            this.PerformLayout();

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
