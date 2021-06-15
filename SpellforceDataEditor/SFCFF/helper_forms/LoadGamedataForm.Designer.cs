namespace SpellforceDataEditor.SFCFF.helper_forms
{
    partial class LoadGamedataForm
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
            this.RadioMergeGD = new System.Windows.Forms.RadioButton();
            this.RadioDiffGD = new System.Windows.Forms.RadioButton();
            this.RadioDependencyGD = new System.Windows.Forms.RadioButton();
            this.RadioFullGD = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.OpenGD = new System.Windows.Forms.OpenFileDialog();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonMainGD = new System.Windows.Forms.Button();
            this.LabelGDMain = new System.Windows.Forms.Label();
            this.ListboxDependencyGD = new System.Windows.Forms.ListBox();
            this.ButtonAddDependencyGD = new System.Windows.Forms.Button();
            this.ButtonRemoveDependencyGD = new System.Windows.Forms.Button();
            this.ButtonMoveUpDependencyGD = new System.Windows.Forms.Button();
            this.ButtonMoveDownDependencyGD = new System.Windows.Forms.Button();
            this.ButtonDiffGD = new System.Windows.Forms.Button();
            this.LabelGDDiff = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.RadioMergeGD);
            this.panel1.Controls.Add(this.RadioDiffGD);
            this.panel1.Controls.Add(this.RadioDependencyGD);
            this.panel1.Controls.Add(this.RadioFullGD);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(401, 38);
            this.panel1.TabIndex = 0;
            // 
            // RadioMergeGD
            // 
            this.RadioMergeGD.AutoSize = true;
            this.RadioMergeGD.Location = new System.Drawing.Point(294, 10);
            this.RadioMergeGD.Name = "RadioMergeGD";
            this.RadioMergeGD.Size = new System.Drawing.Size(55, 17);
            this.RadioMergeGD.TabIndex = 4;
            this.RadioMergeGD.TabStop = true;
            this.RadioMergeGD.Text = "Merge";
            this.RadioMergeGD.UseVisualStyleBackColor = true;
            this.RadioMergeGD.CheckedChanged += new System.EventHandler(this.RadioMergeGD_CheckedChanged);
            // 
            // RadioDiffGD
            // 
            this.RadioDiffGD.AutoSize = true;
            this.RadioDiffGD.Location = new System.Drawing.Point(214, 10);
            this.RadioDiffGD.Name = "RadioDiffGD";
            this.RadioDiffGD.Size = new System.Drawing.Size(74, 17);
            this.RadioDiffGD.TabIndex = 3;
            this.RadioDiffGD.TabStop = true;
            this.RadioDiffGD.Text = "Difference";
            this.RadioDiffGD.UseVisualStyleBackColor = true;
            this.RadioDiffGD.CheckedChanged += new System.EventHandler(this.RadioDiffGD_CheckedChanged);
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
            // OpenGD
            // 
            this.OpenGD.Filter = "CFF files|*.cff|All files|*.*";
            this.OpenGD.Multiselect = true;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 329);
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
            this.ButtonOK.Location = new System.Drawing.Point(338, 329);
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
            // ListboxDependencyGD
            // 
            this.ListboxDependencyGD.FormattingEnabled = true;
            this.ListboxDependencyGD.Location = new System.Drawing.Point(12, 114);
            this.ListboxDependencyGD.Name = "ListboxDependencyGD";
            this.ListboxDependencyGD.Size = new System.Drawing.Size(320, 173);
            this.ListboxDependencyGD.TabIndex = 5;
            this.ListboxDependencyGD.Visible = false;
            // 
            // ButtonAddDependencyGD
            // 
            this.ButtonAddDependencyGD.Location = new System.Drawing.Point(12, 293);
            this.ButtonAddDependencyGD.Name = "ButtonAddDependencyGD";
            this.ButtonAddDependencyGD.Size = new System.Drawing.Size(113, 23);
            this.ButtonAddDependencyGD.TabIndex = 6;
            this.ButtonAddDependencyGD.Text = "Add dependency";
            this.ButtonAddDependencyGD.UseVisualStyleBackColor = true;
            this.ButtonAddDependencyGD.Visible = false;
            this.ButtonAddDependencyGD.Click += new System.EventHandler(this.ButtonAddDependencyGD_Click);
            // 
            // ButtonRemoveDependencyGD
            // 
            this.ButtonRemoveDependencyGD.Location = new System.Drawing.Point(212, 293);
            this.ButtonRemoveDependencyGD.Name = "ButtonRemoveDependencyGD";
            this.ButtonRemoveDependencyGD.Size = new System.Drawing.Size(120, 23);
            this.ButtonRemoveDependencyGD.TabIndex = 7;
            this.ButtonRemoveDependencyGD.Text = "Remove dependency";
            this.ButtonRemoveDependencyGD.UseVisualStyleBackColor = true;
            this.ButtonRemoveDependencyGD.Visible = false;
            this.ButtonRemoveDependencyGD.Click += new System.EventHandler(this.ButtonRemoveDependencyGD_Click);
            // 
            // ButtonMoveUpDependencyGD
            // 
            this.ButtonMoveUpDependencyGD.Location = new System.Drawing.Point(338, 114);
            this.ButtonMoveUpDependencyGD.Name = "ButtonMoveUpDependencyGD";
            this.ButtonMoveUpDependencyGD.Size = new System.Drawing.Size(75, 23);
            this.ButtonMoveUpDependencyGD.TabIndex = 8;
            this.ButtonMoveUpDependencyGD.Text = "Move up";
            this.ButtonMoveUpDependencyGD.UseVisualStyleBackColor = true;
            this.ButtonMoveUpDependencyGD.Visible = false;
            this.ButtonMoveUpDependencyGD.Click += new System.EventHandler(this.ButtonMoveUpDependencyGD_Click);
            // 
            // ButtonMoveDownDependencyGD
            // 
            this.ButtonMoveDownDependencyGD.Location = new System.Drawing.Point(338, 143);
            this.ButtonMoveDownDependencyGD.Name = "ButtonMoveDownDependencyGD";
            this.ButtonMoveDownDependencyGD.Size = new System.Drawing.Size(75, 23);
            this.ButtonMoveDownDependencyGD.TabIndex = 9;
            this.ButtonMoveDownDependencyGD.Text = "Move down";
            this.ButtonMoveDownDependencyGD.UseVisualStyleBackColor = true;
            this.ButtonMoveDownDependencyGD.Visible = false;
            this.ButtonMoveDownDependencyGD.Click += new System.EventHandler(this.ButtonMoveDownDependencyGD_Click);
            // 
            // ButtonDiffGD
            // 
            this.ButtonDiffGD.Location = new System.Drawing.Point(12, 85);
            this.ButtonDiffGD.Name = "ButtonDiffGD";
            this.ButtonDiffGD.Size = new System.Drawing.Size(113, 23);
            this.ButtonDiffGD.TabIndex = 10;
            this.ButtonDiffGD.Text = "Select diff gamedata";
            this.ButtonDiffGD.UseVisualStyleBackColor = true;
            this.ButtonDiffGD.Visible = false;
            this.ButtonDiffGD.Click += new System.EventHandler(this.ButtonDiffGD_Click);
            // 
            // LabelGDDiff
            // 
            this.LabelGDDiff.AutoSize = true;
            this.LabelGDDiff.Location = new System.Drawing.Point(131, 90);
            this.LabelGDDiff.Name = "LabelGDDiff";
            this.LabelGDDiff.Size = new System.Drawing.Size(0, 13);
            this.LabelGDDiff.TabIndex = 11;
            this.LabelGDDiff.Visible = false;
            // 
            // LoadGamedataForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(423, 361);
            this.Controls.Add(this.LabelGDDiff);
            this.Controls.Add(this.ButtonDiffGD);
            this.Controls.Add(this.ButtonMoveDownDependencyGD);
            this.Controls.Add(this.ButtonMoveUpDependencyGD);
            this.Controls.Add(this.ButtonRemoveDependencyGD);
            this.Controls.Add(this.ButtonAddDependencyGD);
            this.Controls.Add(this.ListboxDependencyGD);
            this.Controls.Add(this.LabelGDMain);
            this.Controls.Add(this.ButtonMainGD);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.panel1);
            this.Name = "LoadGamedataForm";
            this.Text = "Load gamedata";
            this.Load += new System.EventHandler(this.LoadGamedataForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton RadioMergeGD;
        private System.Windows.Forms.RadioButton RadioDiffGD;
        private System.Windows.Forms.RadioButton RadioDependencyGD;
        private System.Windows.Forms.RadioButton RadioFullGD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog OpenGD;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonMainGD;
        private System.Windows.Forms.Label LabelGDMain;
        private System.Windows.Forms.ListBox ListboxDependencyGD;
        private System.Windows.Forms.Button ButtonAddDependencyGD;
        private System.Windows.Forms.Button ButtonRemoveDependencyGD;
        private System.Windows.Forms.Button ButtonMoveUpDependencyGD;
        private System.Windows.Forms.Button ButtonMoveDownDependencyGD;
        private System.Windows.Forms.Button ButtonDiffGD;
        private System.Windows.Forms.Label LabelGDDiff;
    }
}