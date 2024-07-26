namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapDecorationInspector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            label1 = new System.Windows.Forms.Label();
            DecGroupName = new System.Windows.Forms.Label();
            DecGroupData = new System.Windows.Forms.DataGridView();
            ObjID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Weight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)DecGroupData).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 0);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(149, 15);
            label1.TabIndex = 0;
            label1.Text = "Selected decoration group:";
            // 
            // DecGroupName
            // 
            DecGroupName.AutoSize = true;
            DecGroupName.Location = new System.Drawing.Point(168, 0);
            DecGroupName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            DecGroupName.Name = "DecGroupName";
            DecGroupName.Size = new System.Drawing.Size(0, 15);
            DecGroupName.TabIndex = 1;
            // 
            // DecGroupData
            // 
            DecGroupData.AllowUserToAddRows = false;
            DecGroupData.AllowUserToDeleteRows = false;
            DecGroupData.AllowUserToResizeColumns = false;
            DecGroupData.AllowUserToResizeRows = false;
            DecGroupData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DecGroupData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ObjID, Weight });
            DecGroupData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            DecGroupData.Location = new System.Drawing.Point(7, 47);
            DecGroupData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            DecGroupData.MultiSelect = false;
            DecGroupData.Name = "DecGroupData";
            DecGroupData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            DecGroupData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            DecGroupData.Size = new System.Drawing.Size(196, 325);
            DecGroupData.TabIndex = 2;
            DecGroupData.Visible = false;
            DecGroupData.CellEndEdit += DecGroupData_CellEndEdit;
            DecGroupData.MouseDown += DecGroupData_MouseDown;
            // 
            // ObjID
            // 
            ObjID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Aquamarine;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            ObjID.DefaultCellStyle = dataGridViewCellStyle1;
            ObjID.HeaderText = "Object ID";
            ObjID.Name = "ObjID";
            ObjID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            ObjID.Width = 62;
            // 
            // Weight
            // 
            Weight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            Weight.HeaderText = "Weight";
            Weight.Name = "Weight";
            Weight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Weight.Width = 51;
            // 
            // MapDecorationInspector
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(DecGroupData);
            Controls.Add(DecGroupName);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MapDecorationInspector";
            Size = new System.Drawing.Size(211, 429);
            ((System.ComponentModel.ISupportInitialize)DecGroupData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DecGroupName;
        private System.Windows.Forms.DataGridView DecGroupData;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weight;
    }
}
