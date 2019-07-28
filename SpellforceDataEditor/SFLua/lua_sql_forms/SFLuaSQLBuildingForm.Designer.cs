namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    partial class SFLuaSQLBuildingForm
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
            this.SelectedMesh = new System.Windows.Forms.TextBox();
            this.ButtonRemoveMesh = new System.Windows.Forms.Button();
            this.ButtonAddMesh = new System.Windows.Forms.Button();
            this.Mesh = new System.Windows.Forms.ListBox();
            this.BuildingID = new System.Windows.Forms.TextBox();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.SelectionSize = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSaveChanges = new System.Windows.Forms.Button();
            this.ButtonCancelChanges = new System.Windows.Forms.Button();
            this.ListBuildings = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // SelectedMesh
            // 
            this.SelectedMesh.Location = new System.Drawing.Point(361, 175);
            this.SelectedMesh.Name = "SelectedMesh";
            this.SelectedMesh.Size = new System.Drawing.Size(322, 20);
            this.SelectedMesh.TabIndex = 82;
            this.SelectedMesh.Validated += new System.EventHandler(this.SelectedMesh_Validated);
            // 
            // ButtonRemoveMesh
            // 
            this.ButtonRemoveMesh.Location = new System.Drawing.Point(608, 64);
            this.ButtonRemoveMesh.Name = "ButtonRemoveMesh";
            this.ButtonRemoveMesh.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemoveMesh.TabIndex = 81;
            this.ButtonRemoveMesh.Text = "Remove";
            this.ButtonRemoveMesh.UseVisualStyleBackColor = true;
            this.ButtonRemoveMesh.Click += new System.EventHandler(this.ButtonRemoveMesh_Click);
            // 
            // ButtonAddMesh
            // 
            this.ButtonAddMesh.Location = new System.Drawing.Point(608, 35);
            this.ButtonAddMesh.Name = "ButtonAddMesh";
            this.ButtonAddMesh.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddMesh.TabIndex = 80;
            this.ButtonAddMesh.Text = "Add";
            this.ButtonAddMesh.UseVisualStyleBackColor = true;
            this.ButtonAddMesh.Click += new System.EventHandler(this.ButtonAddMesh_Click);
            // 
            // Mesh
            // 
            this.Mesh.FormattingEnabled = true;
            this.Mesh.Location = new System.Drawing.Point(361, 35);
            this.Mesh.Name = "Mesh";
            this.Mesh.Size = new System.Drawing.Size(241, 134);
            this.Mesh.TabIndex = 77;
            this.Mesh.SelectedIndexChanged += new System.EventHandler(this.Mesh_SelectedIndexChanged);
            // 
            // BuildingID
            // 
            this.BuildingID.Location = new System.Drawing.Point(12, 282);
            this.BuildingID.Name = "BuildingID";
            this.BuildingID.Size = new System.Drawing.Size(58, 20);
            this.BuildingID.TabIndex = 76;
            this.BuildingID.Validated += new System.EventHandler(this.BuildingID_Validated);
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Location = new System.Drawing.Point(157, 282);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemove.TabIndex = 75;
            this.ButtonRemove.Text = "Remove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(76, 282);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 74;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // SelectionSize
            // 
            this.SelectionSize.Location = new System.Drawing.Point(361, 201);
            this.SelectionSize.Name = "SelectionSize";
            this.SelectionSize.Size = new System.Drawing.Size(100, 20);
            this.SelectionSize.TabIndex = 73;
            this.SelectionSize.Validated += new System.EventHandler(this.SelectionSize_Validated);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(238, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 68;
            this.label9.Text = "Selection size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "Mesh files";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 64;
            this.label1.Text = "Buildings";
            // 
            // ButtonSaveChanges
            // 
            this.ButtonSaveChanges.Location = new System.Drawing.Point(608, 318);
            this.ButtonSaveChanges.Name = "ButtonSaveChanges";
            this.ButtonSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveChanges.TabIndex = 63;
            this.ButtonSaveChanges.Text = "Apply";
            this.ButtonSaveChanges.UseVisualStyleBackColor = true;
            this.ButtonSaveChanges.Click += new System.EventHandler(this.ButtonSaveChanges_Click);
            // 
            // ButtonCancelChanges
            // 
            this.ButtonCancelChanges.Location = new System.Drawing.Point(12, 318);
            this.ButtonCancelChanges.Name = "ButtonCancelChanges";
            this.ButtonCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancelChanges.TabIndex = 62;
            this.ButtonCancelChanges.Text = "Cancel";
            this.ButtonCancelChanges.UseVisualStyleBackColor = true;
            this.ButtonCancelChanges.Click += new System.EventHandler(this.ButtonCancelChanges_Click);
            // 
            // ListBuildings
            // 
            this.ListBuildings.FormattingEnabled = true;
            this.ListBuildings.Location = new System.Drawing.Point(12, 38);
            this.ListBuildings.Name = "ListBuildings";
            this.ListBuildings.Size = new System.Drawing.Size(220, 238);
            this.ListBuildings.TabIndex = 61;
            this.ListBuildings.SelectedIndexChanged += new System.EventHandler(this.ListBuildings_SelectedIndexChanged);
            // 
            // SFLuaSQLBuildingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 348);
            this.Controls.Add(this.SelectedMesh);
            this.Controls.Add(this.ButtonRemoveMesh);
            this.Controls.Add(this.ButtonAddMesh);
            this.Controls.Add(this.Mesh);
            this.Controls.Add(this.BuildingID);
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.SelectionSize);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSaveChanges);
            this.Controls.Add(this.ButtonCancelChanges);
            this.Controls.Add(this.ListBuildings);
            this.Name = "SFLuaSQLBuildingForm";
            this.Text = "sql_building";
            this.Load += new System.EventHandler(this.SFLuaSQLBuildingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SelectedMesh;
        private System.Windows.Forms.Button ButtonRemoveMesh;
        private System.Windows.Forms.Button ButtonAddMesh;
        private System.Windows.Forms.ListBox Mesh;
        private System.Windows.Forms.TextBox BuildingID;
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.TextBox SelectionSize;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSaveChanges;
        private System.Windows.Forms.Button ButtonCancelChanges;
        private System.Windows.Forms.ListBox ListBuildings;
    }
}