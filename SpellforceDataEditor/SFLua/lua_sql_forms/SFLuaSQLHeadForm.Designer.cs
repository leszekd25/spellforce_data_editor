namespace SpellforceDataEditor.SFLua.lua_sql_forms
{
    partial class SFLuaSQLHeadForm
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
            this.ButtonRemove = new System.Windows.Forms.Button();
            this.ButtonAdd = new System.Windows.Forms.Button();
            this.MF = new System.Windows.Forms.TextBox();
            this.MM = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonSaveChanges = new System.Windows.Forms.Button();
            this.ButtonCancelChanges = new System.Windows.Forms.Button();
            this.ListHeads = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ButtonRemove
            // 
            this.ButtonRemove.Location = new System.Drawing.Point(157, 165);
            this.ButtonRemove.Name = "ButtonRemove";
            this.ButtonRemove.Size = new System.Drawing.Size(75, 23);
            this.ButtonRemove.TabIndex = 53;
            this.ButtonRemove.Text = "Remove";
            this.ButtonRemove.UseVisualStyleBackColor = true;
            this.ButtonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // ButtonAdd
            // 
            this.ButtonAdd.Location = new System.Drawing.Point(12, 165);
            this.ButtonAdd.Name = "ButtonAdd";
            this.ButtonAdd.Size = new System.Drawing.Size(75, 23);
            this.ButtonAdd.TabIndex = 52;
            this.ButtonAdd.Text = "Add";
            this.ButtonAdd.UseVisualStyleBackColor = true;
            this.ButtonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // MF
            // 
            this.MF.Location = new System.Drawing.Point(361, 64);
            this.MF.Name = "MF";
            this.MF.Size = new System.Drawing.Size(322, 20);
            this.MF.TabIndex = 43;
            this.MF.Validated += new System.EventHandler(this.MF_Validated);
            // 
            // MM
            // 
            this.MM.Location = new System.Drawing.Point(361, 38);
            this.MM.Name = "MM";
            this.MM.Size = new System.Drawing.Size(322, 20);
            this.MM.TabIndex = 42;
            this.MM.Validated += new System.EventHandler(this.MM_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 33;
            this.label3.Text = "Mesh (female)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Mesh (male)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Items";
            // 
            // ButtonSaveChanges
            // 
            this.ButtonSaveChanges.Location = new System.Drawing.Point(608, 194);
            this.ButtonSaveChanges.Name = "ButtonSaveChanges";
            this.ButtonSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonSaveChanges.TabIndex = 30;
            this.ButtonSaveChanges.Text = "Apply";
            this.ButtonSaveChanges.UseVisualStyleBackColor = true;
            this.ButtonSaveChanges.Click += new System.EventHandler(this.ButtonSaveChanges_Click);
            // 
            // ButtonCancelChanges
            // 
            this.ButtonCancelChanges.Location = new System.Drawing.Point(12, 194);
            this.ButtonCancelChanges.Name = "ButtonCancelChanges";
            this.ButtonCancelChanges.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancelChanges.TabIndex = 29;
            this.ButtonCancelChanges.Text = "Cancel";
            this.ButtonCancelChanges.UseVisualStyleBackColor = true;
            this.ButtonCancelChanges.Click += new System.EventHandler(this.ButtonCancelChanges_Click);
            // 
            // ListHeads
            // 
            this.ListHeads.FormattingEnabled = true;
            this.ListHeads.Location = new System.Drawing.Point(12, 38);
            this.ListHeads.Name = "ListHeads";
            this.ListHeads.Size = new System.Drawing.Size(220, 121);
            this.ListHeads.TabIndex = 28;
            this.ListHeads.SelectedIndexChanged += new System.EventHandler(this.ListHeads_SelectedIndexChanged);
            // 
            // SFLuaSQLHeadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 226);
            this.Controls.Add(this.ButtonRemove);
            this.Controls.Add(this.ButtonAdd);
            this.Controls.Add(this.MF);
            this.Controls.Add(this.MM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonSaveChanges);
            this.Controls.Add(this.ButtonCancelChanges);
            this.Controls.Add(this.ListHeads);
            this.Name = "SFLuaSQLHeadForm";
            this.Text = "SFLuaSQLHeadForm";
            this.Load += new System.EventHandler(this.SFLuaSQLHeadForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonRemove;
        private System.Windows.Forms.Button ButtonAdd;
        private System.Windows.Forms.TextBox MF;
        private System.Windows.Forms.TextBox MM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonSaveChanges;
        private System.Windows.Forms.Button ButtonCancelChanges;
        private System.Windows.Forms.ListBox ListHeads;
    }
}