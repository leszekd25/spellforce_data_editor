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
            panel1 = new System.Windows.Forms.Panel();
            RadioMergeGD = new System.Windows.Forms.RadioButton();
            RadioDiffGD = new System.Windows.Forms.RadioButton();
            RadioFullGD = new System.Windows.Forms.RadioButton();
            label1 = new System.Windows.Forms.Label();
            OpenGD = new System.Windows.Forms.OpenFileDialog();
            ButtonCancel = new System.Windows.Forms.Button();
            ButtonOK = new System.Windows.Forms.Button();
            ButtonMainGD = new System.Windows.Forms.Button();
            LabelGDMain = new System.Windows.Forms.Label();
            ButtonOtherGD = new System.Windows.Forms.Button();
            LabelGDOther = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(RadioMergeGD);
            panel1.Controls.Add(RadioDiffGD);
            panel1.Controls.Add(RadioFullGD);
            panel1.Controls.Add(label1);
            panel1.Location = new System.Drawing.Point(14, 14);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(468, 44);
            panel1.TabIndex = 0;
            // 
            // RadioMergeGD
            // 
            RadioMergeGD.AutoSize = true;
            RadioMergeGD.Location = new System.Drawing.Point(158, 12);
            RadioMergeGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RadioMergeGD.Name = "RadioMergeGD";
            RadioMergeGD.Size = new System.Drawing.Size(59, 19);
            RadioMergeGD.TabIndex = 4;
            RadioMergeGD.TabStop = true;
            RadioMergeGD.Text = "Merge";
            RadioMergeGD.UseVisualStyleBackColor = true;
            RadioMergeGD.CheckedChanged += RadioMergeGD_CheckedChanged;
            // 
            // RadioDiffGD
            // 
            RadioDiffGD.AutoSize = true;
            RadioDiffGD.Location = new System.Drawing.Point(244, 12);
            RadioDiffGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RadioDiffGD.Name = "RadioDiffGD";
            RadioDiffGD.Size = new System.Drawing.Size(79, 19);
            RadioDiffGD.TabIndex = 3;
            RadioDiffGD.TabStop = true;
            RadioDiffGD.Text = "Difference";
            RadioDiffGD.UseVisualStyleBackColor = true;
            RadioDiffGD.CheckedChanged += RadioDiffGD_CheckedChanged;
            // 
            // RadioFullGD
            // 
            RadioFullGD.AutoSize = true;
            RadioFullGD.Location = new System.Drawing.Point(84, 12);
            RadioFullGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RadioFullGD.Name = "RadioFullGD";
            RadioFullGD.Size = new System.Drawing.Size(44, 19);
            RadioFullGD.TabIndex = 1;
            RadioFullGD.TabStop = true;
            RadioFullGD.Text = "Full";
            RadioFullGD.UseVisualStyleBackColor = true;
            RadioFullGD.CheckedChanged += RadioFullGD_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(70, 15);
            label1.TabIndex = 0;
            label1.Text = "Load mode:";
            // 
            // OpenGD
            // 
            OpenGD.Filter = "CFF files|*.cff|All files|*.*";
            // 
            // ButtonCancel
            // 
            ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonCancel.Location = new System.Drawing.Point(14, 131);
            ButtonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new System.Drawing.Size(88, 27);
            ButtonCancel.TabIndex = 1;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            ButtonOK.Enabled = false;
            ButtonOK.Location = new System.Drawing.Point(394, 131);
            ButtonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new System.Drawing.Size(88, 27);
            ButtonOK.TabIndex = 2;
            ButtonOK.Text = "OK";
            ButtonOK.UseVisualStyleBackColor = true;
            // 
            // ButtonMainGD
            // 
            ButtonMainGD.Location = new System.Drawing.Point(14, 65);
            ButtonMainGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonMainGD.Name = "ButtonMainGD";
            ButtonMainGD.Size = new System.Drawing.Size(150, 27);
            ButtonMainGD.TabIndex = 3;
            ButtonMainGD.Text = "Select gamedata";
            ButtonMainGD.UseVisualStyleBackColor = true;
            ButtonMainGD.Visible = false;
            ButtonMainGD.Click += ButtonMainGD_Click;
            // 
            // LabelGDMain
            // 
            LabelGDMain.AutoSize = true;
            LabelGDMain.Location = new System.Drawing.Point(172, 70);
            LabelGDMain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelGDMain.Name = "LabelGDMain";
            LabelGDMain.Size = new System.Drawing.Size(0, 15);
            LabelGDMain.TabIndex = 4;
            LabelGDMain.Visible = false;
            // 
            // ButtonOtherGD
            // 
            ButtonOtherGD.Location = new System.Drawing.Point(14, 98);
            ButtonOtherGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonOtherGD.Name = "ButtonOtherGD";
            ButtonOtherGD.Size = new System.Drawing.Size(150, 27);
            ButtonOtherGD.TabIndex = 10;
            ButtonOtherGD.Text = "Select other gamedata";
            ButtonOtherGD.UseVisualStyleBackColor = true;
            ButtonOtherGD.Visible = false;
            ButtonOtherGD.Click += ButtonOtherGD_Click;
            // 
            // LabelGDOther
            // 
            LabelGDOther.AutoSize = true;
            LabelGDOther.Location = new System.Drawing.Point(172, 104);
            LabelGDOther.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelGDOther.Name = "LabelGDOther";
            LabelGDOther.Size = new System.Drawing.Size(0, 15);
            LabelGDOther.TabIndex = 11;
            LabelGDOther.Visible = false;
            // 
            // LoadGamedataForm
            // 
            AcceptButton = ButtonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = ButtonCancel;
            ClientSize = new System.Drawing.Size(493, 168);
            Controls.Add(LabelGDOther);
            Controls.Add(ButtonOtherGD);
            Controls.Add(LabelGDMain);
            Controls.Add(ButtonMainGD);
            Controls.Add(ButtonOK);
            Controls.Add(ButtonCancel);
            Controls.Add(panel1);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "LoadGamedataForm";
            Text = "Load gamedata";
            Load += LoadGamedataForm_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton RadioMergeGD;
        private System.Windows.Forms.RadioButton RadioDiffGD;
        private System.Windows.Forms.RadioButton RadioFullGD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog OpenGD;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonMainGD;
        private System.Windows.Forms.Label LabelGDMain;
        private System.Windows.Forms.Button ButtonOtherGD;
        private System.Windows.Forms.Label LabelGDOther;
    }
}