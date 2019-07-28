namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    partial class SFLuaSQLObjectForm
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
            this.ObjectID = new System.Windows.Forms.TextBox();
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.SelectionSize = new System.Windows.Forms.TextBox();
            this.Scale = new System.Windows.Forms.TextBox();
            this.ObjName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSaveChanges = new System.Windows.Forms.Button();
            this.ButtonCancelChanges = new System.Windows.Forms.Button();
            this.ListObjects = new System.Windows.Forms.ListBox();
            this.Mesh = new System.Windows.Forms.ListBox();
            this.CastsShadow = new System.Windows.Forms.ComboBox();
            this.IsBillboarded = new System.Windows.Forms.ComboBox();
            this.ButtonAddMesh = new System.Windows.Forms.Button();
            this.ButtonRemoveMesh = new System.Windows.Forms.Button();
            this.SelectedMesh = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ObjectID
            // 
            this.ObjectID.Location = new System.Drawing.Point(12, 336);
            this.ObjectID.Name = "ObjectID";
            this.ObjectID.Size = new System.Drawing.Size(58, 20);
            this.ObjectID.TabIndex = 54;
            this.ObjectID.Validated += new System.EventHandler(this.ObjectID_Validated);
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Location = new System.Drawing.Point(157, 336);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemove.TabIndex = 53;
            this.ButtonRemove.Text = "Remove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(76, 336);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 52;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // SelectionSize
            // 
            this.SelectionSize.Location = new System.Drawing.Point(361, 307);
            this.SelectionSize.Name = "SelectionSize";
            this.SelectionSize.Size = new System.Drawing.Size(100, 20);
            this.SelectionSize.TabIndex = 51;
            this.SelectionSize.Validated += new System.EventHandler(this.SelectionSize_Validated);
            // 
            // Scale
            // 
            this.Scale.Location = new System.Drawing.Point(361, 281);
            this.Scale.Name = "Scale";
            this.Scale.Size = new System.Drawing.Size(100, 20);
            this.Scale.TabIndex = 49;
            this.Scale.Validated += new System.EventHandler(this.Scale_Validated);
            // 
            // ObjName
            // 
            this.ObjName.Location = new System.Drawing.Point(361, 38);
            this.ObjName.Name = "ObjName";
            this.ObjName.Size = new System.Drawing.Size(322, 20);
            this.ObjName.TabIndex = 42;
            this.ObjName.Validated += new System.EventHandler(this.ObjName_Validated);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(238, 284);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Scale (%)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(238, 259);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 13);
            this.label10.TabIndex = 40;
            this.label10.Text = "Is billboarded?";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(238, 310);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Selection size";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(238, 233);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 37;
            this.label7.Text = "Casts shadow?";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Mesh files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Objects";
            // 
            // ButtonSaveChanges
            // 
            this.ButtonSaveChanges.Location = new System.Drawing.Point(608, 372);
            this.ButtonSaveChanges.Name = "ButtonSaveChanges";
            this.ButtonSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveChanges.TabIndex = 30;
            this.ButtonSaveChanges.Text = "Apply";
            this.ButtonSaveChanges.UseVisualStyleBackColor = true;
            this.ButtonSaveChanges.Click += new System.EventHandler(this.ButtonSaveChanges_Click);
            // 
            // ButtonCancelChanges
            // 
            this.ButtonCancelChanges.Location = new System.Drawing.Point(12, 372);
            this.ButtonCancelChanges.Name = "ButtonCancelChanges";
            this.ButtonCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancelChanges.TabIndex = 29;
            this.ButtonCancelChanges.Text = "Cancel";
            this.ButtonCancelChanges.UseVisualStyleBackColor = true;
            this.ButtonCancelChanges.Click += new System.EventHandler(this.ButtonCancelChanges_Click);
            // 
            // ListObjects
            // 
            this.ListObjects.FormattingEnabled = true;
            this.ListObjects.Location = new System.Drawing.Point(12, 38);
            this.ListObjects.Name = "ListObjects";
            this.ListObjects.Size = new System.Drawing.Size(220, 290);
            this.ListObjects.TabIndex = 28;
            this.ListObjects.SelectedIndexChanged += new System.EventHandler(this.ListObjects_SelectedIndexChanged);
            // 
            // Mesh
            // 
            this.Mesh.FormattingEnabled = true;
            this.Mesh.Location = new System.Drawing.Point(361, 64);
            this.Mesh.Name = "Mesh";
            this.Mesh.Size = new System.Drawing.Size(241, 134);
            this.Mesh.TabIndex = 55;
            this.Mesh.SelectedIndexChanged += new System.EventHandler(this.Mesh_SelectedIndexChanged);
            // 
            // CastsShadow
            // 
            this.CastsShadow.FormattingEnabled = true;
            this.CastsShadow.Items.AddRange(new object[] {
            "False",
            "True"});
            this.CastsShadow.Location = new System.Drawing.Point(361, 230);
            this.CastsShadow.Name = "CastsShadow";
            this.CastsShadow.Size = new System.Drawing.Size(100, 21);
            this.CastsShadow.TabIndex = 56;
            this.CastsShadow.SelectedIndexChanged += new System.EventHandler(this.CastsShadow_SelectedIndexChanged);
            // 
            // IsBillboarded
            // 
            this.IsBillboarded.FormattingEnabled = true;
            this.IsBillboarded.Items.AddRange(new object[] {
            "False",
            "True"});
            this.IsBillboarded.Location = new System.Drawing.Point(361, 256);
            this.IsBillboarded.Name = "IsBillboarded";
            this.IsBillboarded.Size = new System.Drawing.Size(100, 21);
            this.IsBillboarded.TabIndex = 57;
            this.IsBillboarded.SelectedIndexChanged += new System.EventHandler(this.IsBillboarded_SelectedIndexChanged);
            // 
            // ButtonAddMesh
            // 
            this.ButtonAddMesh.Location = new System.Drawing.Point(608, 64);
            this.ButtonAddMesh.Name = "ButtonAddMesh";
            this.ButtonAddMesh.Size = new System.Drawing.Size(75, 23);
            this.ButtonAddMesh.TabIndex = 58;
            this.ButtonAddMesh.Text = "Add";
            this.ButtonAddMesh.UseVisualStyleBackColor = true;
            this.ButtonAddMesh.Click += new System.EventHandler(this.ButtonAddMesh_Click);
            // 
            // ButtonRemoveMesh
            // 
            this.ButtonRemoveMesh.Location = new System.Drawing.Point(608, 93);
            this.ButtonRemoveMesh.Name = "ButtonRemoveMesh";
            this.ButtonRemoveMesh.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemoveMesh.TabIndex = 59;
            this.ButtonRemoveMesh.Text = "Remove";
            this.ButtonRemoveMesh.UseVisualStyleBackColor = true;
            this.ButtonRemoveMesh.Click += new System.EventHandler(this.ButtonRemoveMesh_Click);
            // 
            // SelectedMesh
            // 
            this.SelectedMesh.Location = new System.Drawing.Point(361, 204);
            this.SelectedMesh.Name = "SelectedMesh";
            this.SelectedMesh.Size = new System.Drawing.Size(322, 20);
            this.SelectedMesh.TabIndex = 60;
            this.SelectedMesh.Validated += new System.EventHandler(this.SelectedMesh_Validated);
            // 
            // SFLuaSQLObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 407);
            this.Controls.Add(this.SelectedMesh);
            this.Controls.Add(this.ButtonRemoveMesh);
            this.Controls.Add(this.ButtonAddMesh);
            this.Controls.Add(this.IsBillboarded);
            this.Controls.Add(this.CastsShadow);
            this.Controls.Add(this.Mesh);
            this.Controls.Add(this.ObjectID);
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.SelectionSize);
            this.Controls.Add(this.Scale);
            this.Controls.Add(this.ObjName);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSaveChanges);
            this.Controls.Add(this.ButtonCancelChanges);
            this.Controls.Add(this.ListObjects);
            this.Name = "SFLuaSQLObjectForm";
            this.Text = "sql_object";
            this.Load += new System.EventHandler(this.SFLuaSQLObjectForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ObjectID;
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.TextBox SelectionSize;
        private System.Windows.Forms.TextBox Scale;
        private System.Windows.Forms.TextBox ObjName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSaveChanges;
        private System.Windows.Forms.Button ButtonCancelChanges;
        private System.Windows.Forms.ListBox ListObjects;
        private System.Windows.Forms.ListBox Mesh;
        private System.Windows.Forms.ComboBox CastsShadow;
        private System.Windows.Forms.ComboBox IsBillboarded;
        private System.Windows.Forms.Button ButtonAddMesh;
        private System.Windows.Forms.Button ButtonRemoveMesh;
        private System.Windows.Forms.TextBox SelectedMesh;
    }
}