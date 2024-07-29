namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control31
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
            comboItemType = new System.Windows.Forms.ComboBox();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            ListItemTypes = new System.Windows.Forms.ListBox();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            textBox4 = new System.Windows.Forms.TextBox();
            textBox5 = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // comboItemType
            // 
            comboItemType.FormattingEnabled = true;
            comboItemType.Location = new System.Drawing.Point(122, 213);
            comboItemType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            comboItemType.Name = "comboItemType";
            comboItemType.Size = new System.Drawing.Size(146, 23);
            comboItemType.TabIndex = 58;
            comboItemType.SelectedIndexChanged += comboItemType_SelectedIndexChanged;
            comboItemType.Invalidated += comboItemType_SelectedIndexChanged;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(443, 85);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(88, 27);
            button2.TabIndex = 57;
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
            button1.TabIndex = 56;
            button1.Text = "Insert";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ListItemTypes
            // 
            ListItemTypes.FormattingEnabled = true;
            ListItemTypes.ItemHeight = 15;
            ListItemTypes.Location = new System.Drawing.Point(122, 52);
            ListItemTypes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ListItemTypes.Name = "ListItemTypes";
            ListItemTypes.Size = new System.Drawing.Size(313, 154);
            ListItemTypes.TabIndex = 55;
            ListItemTypes.SelectedIndexChanged += ListItemTypes_SelectedIndexChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(9, 248);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(108, 15);
            label4.TabIndex = 54;
            label4.Text = "Price multiplier (%)";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(57, 217);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(57, 15);
            label5.TabIndex = 53;
            label5.Text = "Item type";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(38, 25);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(72, 15);
            label6.TabIndex = 52;
            label6.Text = "Merchant ID";
            // 
            // textBox4
            // 
            textBox4.Location = new System.Drawing.Point(122, 245);
            textBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox4.Name = "textBox4";
            textBox4.Size = new System.Drawing.Size(146, 23);
            textBox4.TabIndex = 51;
            textBox4.Invalidated += textBox4_TextChanged;
            textBox4.Validated += textBox4_Validated;
            // 
            // textBox5
            // 
            textBox5.BackColor = System.Drawing.Color.DarkOrange;
            textBox5.Location = new System.Drawing.Point(122, 22);
            textBox5.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox5.Name = "textBox5";
            textBox5.ShortcutsEnabled = false;
            textBox5.Size = new System.Drawing.Size(146, 23);
            textBox5.TabIndex = 50;
            textBox5.Invalidated += textBox5_TextChanged;
            textBox5.MouseDown += textBox5_MouseDown;
            // 
            // Control31
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(comboItemType);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(ListItemTypes);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(textBox4);
            Controls.Add(textBox5);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control31";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox comboItemType;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox ListItemTypes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
    }
}
