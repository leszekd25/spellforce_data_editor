namespace SpellforceDataEditor.SFCFF.helper_forms
{
    partial class DiffSelectForm
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
            this.ButtonGD1 = new System.Windows.Forms.Button();
            this.ButtonGD2 = new System.Windows.Forms.Button();
            this.GDSelect = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelGD1Name = new System.Windows.Forms.Label();
            this.LabelGD2Name = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonGD1
            // 
            this.ButtonGD1.Location = new System.Drawing.Point(121, 31);
            this.ButtonGD1.Name = "ButtonGD1";
            this.ButtonGD1.Size = new System.Drawing.Size(75, 23);
            this.ButtonGD1.TabIndex = 0;
            this.ButtonGD1.Text = "Select...";
            this.ButtonGD1.UseVisualStyleBackColor = true;
            this.ButtonGD1.Click += new System.EventHandler(this.ButtonGD1_Click);
            // 
            // ButtonGD2
            // 
            this.ButtonGD2.Location = new System.Drawing.Point(121, 60);
            this.ButtonGD2.Name = "ButtonGD2";
            this.ButtonGD2.Size = new System.Drawing.Size(75, 23);
            this.ButtonGD2.TabIndex = 1;
            this.ButtonGD2.Text = "Select...";
            this.ButtonGD2.UseVisualStyleBackColor = true;
            this.ButtonGD2.Click += new System.EventHandler(this.ButtonGD2_Click);
            // 
            // GDSelect
            // 
            this.GDSelect.FileName = "GameData.cff";
            this.GDSelect.Filter = "CFF files|*.cff";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "GameData 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "GameData 2";
            // 
            // LabelGD1Name
            // 
            this.LabelGD1Name.AutoSize = true;
            this.LabelGD1Name.Location = new System.Drawing.Point(202, 36);
            this.LabelGD1Name.Name = "LabelGD1Name";
            this.LabelGD1Name.Size = new System.Drawing.Size(0, 13);
            this.LabelGD1Name.TabIndex = 4;
            // 
            // LabelGD2Name
            // 
            this.LabelGD2Name.AutoSize = true;
            this.LabelGD2Name.Location = new System.Drawing.Point(202, 65);
            this.LabelGD2Name.Name = "LabelGD2Name";
            this.LabelGD2Name.Size = new System.Drawing.Size(0, 13);
            this.LabelGD2Name.TabIndex = 5;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 121);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 6;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Enabled = false;
            this.ButtonOK.Location = new System.Drawing.Point(340, 121);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 7;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(187, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Select gamedata files to make a diff of";
            // 
            // DiffSelectForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(423, 151);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.LabelGD2Name);
            this.Controls.Add(this.LabelGD1Name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ButtonGD2);
            this.Controls.Add(this.ButtonGD1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiffSelectForm";
            this.Text = "Select gamedata files";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonGD1;
        private System.Windows.Forms.Button ButtonGD2;
        private System.Windows.Forms.OpenFileDialog GDSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelGD1Name;
        private System.Windows.Forms.Label LabelGD2Name;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Label label5;
    }
}