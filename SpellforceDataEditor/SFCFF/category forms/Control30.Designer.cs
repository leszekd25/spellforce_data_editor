namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control30
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            label1 = new System.Windows.Forms.Label();
            textBox1 = new System.Windows.Forms.TextBox();
            MerchantGrid = new System.Windows.Forms.DataGridView();
            ItemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ItemCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            button1 = new System.Windows.Forms.Button();
            textBox2 = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)MerchantGrid).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(38, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 15);
            label1.TabIndex = 49;
            label1.Text = "Merchant ID";
            // 
            // textBox1
            // 
            textBox1.BackColor = System.Drawing.Color.DarkOrange;
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.ShortcutsEnabled = false;
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 46;
            textBox1.Leave += textBox1_TextChanged;
            textBox1.MouseDown += textBox1_MouseDown;
            // 
            // MerchantGrid
            // 
            MerchantGrid.AllowUserToAddRows = false;
            MerchantGrid.AllowUserToDeleteRows = false;
            MerchantGrid.AllowUserToResizeRows = false;
            MerchantGrid.ColumnHeadersHeight = 20;
            MerchantGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            MerchantGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ItemID, ItemCount, ItemName });
            MerchantGrid.Location = new System.Drawing.Point(42, 52);
            MerchantGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MerchantGrid.MultiSelect = false;
            MerchantGrid.Name = "MerchantGrid";
            MerchantGrid.Size = new System.Drawing.Size(527, 298);
            MerchantGrid.TabIndex = 52;
            MerchantGrid.CellValidated += OnCellValueChange;
            MerchantGrid.MouseDown += MerchantGrid_MouseDown;
            // 
            // ItemID
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(255, 128, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            ItemID.DefaultCellStyle = dataGridViewCellStyle1;
            ItemID.HeaderText = "Item ID";
            ItemID.Name = "ItemID";
            ItemID.Width = 60;
            // 
            // ItemCount
            // 
            ItemCount.HeaderText = "Item count";
            ItemCount.Name = "ItemCount";
            ItemCount.Width = 80;
            // 
            // ItemName
            // 
            ItemName.HeaderText = "Item name";
            ItemName.Name = "ItemName";
            ItemName.ReadOnly = true;
            ItemName.Width = 250;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(42, 356);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 27);
            button1.TabIndex = 53;
            button1.Text = "Insert";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = System.Drawing.Color.DarkOrange;
            textBox2.Location = new System.Drawing.Point(192, 358);
            textBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.ShortcutsEnabled = false;
            textBox2.Size = new System.Drawing.Size(116, 23);
            textBox2.TabIndex = 54;
            textBox2.MouseDown += textBox2_MouseDown;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(138, 363);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(45, 15);
            label2.TabIndex = 55;
            label2.Text = "Item ID";
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(42, 390);
            button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(88, 27);
            button2.TabIndex = 56;
            button2.Text = "Remove";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Control30
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(MerchantGrid);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control30";
            ((System.ComponentModel.ISupportInitialize)MerchantGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView MerchantGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
    }
}
