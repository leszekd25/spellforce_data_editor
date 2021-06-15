namespace SpellforceDataEditor.SFCFF.helper_forms
{
    partial class SaveGamedataForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.RadioDependencyGD = new System.Windows.Forms.RadioButton();
            this.RadioFullGD = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonMainGD = new System.Windows.Forms.Button();
            this.LabelGDMain = new System.Windows.Forms.Label();
            this.SaveGD = new System.Windows.Forms.SaveFileDialog();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RadioDependencyGD);
            this.panel1.Controls.Add(this.RadioFullGD);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(401, 38);
            this.panel1.TabIndex = 0;
            // 
            // RadioDependencyGD
            // 
            this.RadioDependencyGD.AutoSize = true;
            this.RadioDependencyGD.Location = new System.Drawing.Point(122, 10);
            this.RadioDependencyGD.Name = "RadioDependencyGD";
            this.RadioDependencyGD.Size = new System.Drawing.Size(86, 17);
            this.RadioDependencyGD.TabIndex = 2;
            this.RadioDependencyGD.TabStop = true;
            this.RadioDependencyGD.Text = "Dependency";
            this.RadioDependencyGD.UseVisualStyleBackColor = true;
            this.RadioDependencyGD.CheckedChanged += new System.EventHandler(this.RadioDependencyGD_CheckedChanged);
            // 
            // RadioFullGD
            // 
            this.RadioFullGD.AutoSize = true;
            this.RadioFullGD.Location = new System.Drawing.Point(72, 10);
            this.RadioFullGD.Name = "RadioFullGD";
            this.RadioFullGD.Size = new System.Drawing.Size(41, 17);
            this.RadioFullGD.TabIndex = 1;
            this.RadioFullGD.TabStop = true;
            this.RadioFullGD.Text = "Full";
            this.RadioFullGD.UseVisualStyleBackColor = true;
            this.RadioFullGD.CheckedChanged += new System.EventHandler(this.RadioFullGD_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Load mode:";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 117);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 1;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Enabled = false;
            this.ButtonOK.Location = new System.Drawing.Point(338, 117);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 2;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // ButtonMainGD
            // 
            this.ButtonMainGD.Location = new System.Drawing.Point(12, 56);
            this.ButtonMainGD.Name = "ButtonMainGD";
            this.ButtonMainGD.Size = new System.Drawing.Size(113, 23);
            this.ButtonMainGD.TabIndex = 3;
            this.ButtonMainGD.Text = "Select gamedata";
            this.ButtonMainGD.UseVisualStyleBackColor = true;
            this.ButtonMainGD.Visible = false;
            this.ButtonMainGD.Click += new System.EventHandler(this.ButtonMainGD_Click);
            // 
            // LabelGDMain
            // 
            this.LabelGDMain.AutoSize = true;
            this.LabelGDMain.Location = new System.Drawing.Point(131, 61);
            this.LabelGDMain.Name = "LabelGDMain";
            this.LabelGDMain.Size = new System.Drawing.Size(0, 13);
            this.LabelGDMain.TabIndex = 4;
            this.LabelGDMain.Visible = false;
            // 
            // SaveGD
            // 
            this.SaveGD.FileName = "GameData_new.cff";
            this.SaveGD.Filter = "CFF files|*.cff";
            // 
            // SaveGamedataForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(423, 152);
            this.Controls.Add(this.LabelGDMain);
            this.Controls.Add(this.ButtonMainGD);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.panel1);
            this.Name = "SaveGamedataForm";
            this.Text = "Save gamedata";
            this.Load += new System.EventHandler(this.LoadGamedataForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton RadioDependencyGD;
        private System.Windows.Forms.RadioButton RadioFullGD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonMainGD;
        private System.Windows.Forms.Label LabelGDMain;
        private System.Windows.Forms.SaveFileDialog SaveGD;
    }
}