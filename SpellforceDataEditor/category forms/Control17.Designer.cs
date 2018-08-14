namespace SpellforceDataEditor.category_forms
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.RelationGrid = new System.Windows.Forms.DataGridView();
            this.ClanName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Relation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.RelationGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Clan ID";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(105, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Validated += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // RelationGrid
            // 
            this.RelationGrid.AllowUserToAddRows = false;
            this.RelationGrid.AllowUserToDeleteRows = false;
            this.RelationGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RelationGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClanName,
            this.Relation});
            this.RelationGrid.Location = new System.Drawing.Point(55, 45);
            this.RelationGrid.Name = "RelationGrid";
            this.RelationGrid.Size = new System.Drawing.Size(360, 370);
            this.RelationGrid.TabIndex = 22;
            this.RelationGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // ClanName
            // 
            this.ClanName.HeaderText = "Clan name";
            this.ClanName.Name = "ClanName";
            this.ClanName.ReadOnly = true;
            this.ClanName.Width = 200;
            // 
            // Relation
            // 
            this.Relation.HeaderText = "Relation";
            this.Relation.Items.AddRange(new object[] {
            "Neutral",
            "Friendly",
            "Hostile"});
            this.Relation.Name = "Relation";
            this.Relation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Relation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Control17
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RelationGrid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "Control17";
            ((System.ComponentModel.ISupportInitialize)(this.RelationGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView RelationGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClanName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Relation;
    }
}
