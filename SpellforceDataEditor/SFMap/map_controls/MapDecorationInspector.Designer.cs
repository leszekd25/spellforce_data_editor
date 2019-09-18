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
            this.label1 = new System.Windows.Forms.Label();
            this.DecGroupName = new System.Windows.Forms.Label();
            this.DecGroupData = new System.Windows.Forms.DataGridView();
            this.ObjID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Weight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DecGroupData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selected decoration group:";
            // 
            // DecGroupName
            // 
            this.DecGroupName.AutoSize = true;
            this.DecGroupName.Location = new System.Drawing.Point(144, 0);
            this.DecGroupName.Name = "DecGroupName";
            this.DecGroupName.Size = new System.Drawing.Size(0, 13);
            this.DecGroupName.TabIndex = 1;
            // 
            // DecGroupData
            // 
            this.DecGroupData.AllowUserToAddRows = false;
            this.DecGroupData.AllowUserToDeleteRows = false;
            this.DecGroupData.AllowUserToResizeColumns = false;
            this.DecGroupData.AllowUserToResizeRows = false;
            this.DecGroupData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DecGroupData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ObjID,
            this.Weight});
            this.DecGroupData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.DecGroupData.Location = new System.Drawing.Point(6, 41);
            this.DecGroupData.MultiSelect = false;
            this.DecGroupData.Name = "DecGroupData";
            this.DecGroupData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DecGroupData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DecGroupData.Size = new System.Drawing.Size(168, 282);
            this.DecGroupData.TabIndex = 2;
            this.DecGroupData.Visible = false;
            this.DecGroupData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DecGroupData_CellEndEdit);
            // 
            // ObjID
            // 
            this.ObjID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.ObjID.HeaderText = "Object ID";
            this.ObjID.Name = "ObjID";
            this.ObjID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ObjID.Width = 58;
            // 
            // Weight
            // 
            this.Weight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Weight.HeaderText = "Weight";
            this.Weight.Name = "Weight";
            this.Weight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Weight.Width = 47;
            // 
            // MapDecorationInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.DecGroupData);
            this.Controls.Add(this.DecGroupName);
            this.Controls.Add(this.label1);
            this.Name = "MapDecorationInspector";
            this.Size = new System.Drawing.Size(181, 372);
            ((System.ComponentModel.ISupportInitialize)(this.DecGroupData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label DecGroupName;
        private System.Windows.Forms.DataGridView DecGroupData;
        private System.Windows.Forms.DataGridViewTextBoxColumn ObjID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weight;
    }
}
