namespace SpellforceDataEditor.SFCFF.category_forms
{
    partial class Control17
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
            RelationGrid = new System.Windows.Forms.DataGridView();
            ClanName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Relation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)RelationGrid).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(66, 25);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(45, 15);
            label1.TabIndex = 19;
            label1.Text = "Clan ID";
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(122, 22);
            textBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(146, 23);
            textBox1.TabIndex = 16;
            textBox1.Leave += textBox1_TextChanged;
            // 
            // RelationGrid
            // 
            RelationGrid.AllowUserToAddRows = false;
            RelationGrid.AllowUserToDeleteRows = false;
            RelationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            RelationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ClanName, Relation });
            RelationGrid.Location = new System.Drawing.Point(64, 52);
            RelationGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RelationGrid.Name = "RelationGrid";
            RelationGrid.Size = new System.Drawing.Size(420, 368);
            RelationGrid.TabIndex = 22;
            RelationGrid.CellValidated += dataGridView1_CellValueChanged;
            // 
            // ClanName
            // 
            ClanName.HeaderText = "Clan name";
            ClanName.Name = "ClanName";
            ClanName.ReadOnly = true;
            ClanName.Width = 200;
            // 
            // Relation
            // 
            Relation.HeaderText = "Relation";
            Relation.Items.AddRange(new object[] { "Neutral", "Friendly", "Hostile" });
            Relation.Name = "Relation";
            Relation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            Relation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Control17
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(RelationGrid);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            Name = "Control17";
            ((System.ComponentModel.ISupportInitialize)RelationGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView RelationGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClanName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Relation;
    }
}
