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
            ButtonCancel = new System.Windows.Forms.Button();
            ButtonOK = new System.Windows.Forms.Button();
            ButtonMainGD = new System.Windows.Forms.Button();
            LabelGDMain = new System.Windows.Forms.Label();
            SaveGD = new System.Windows.Forms.SaveFileDialog();
            SuspendLayout();
            // 
            // ButtonCancel
            // 
            ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            ButtonCancel.Location = new System.Drawing.Point(13, 45);
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
            ButtonOK.Location = new System.Drawing.Point(393, 45);
            ButtonOK.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new System.Drawing.Size(88, 27);
            ButtonOK.TabIndex = 2;
            ButtonOK.Text = "OK";
            ButtonOK.UseVisualStyleBackColor = true;
            // 
            // ButtonMainGD
            // 
            ButtonMainGD.Location = new System.Drawing.Point(14, 12);
            ButtonMainGD.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            ButtonMainGD.Name = "ButtonMainGD";
            ButtonMainGD.Size = new System.Drawing.Size(132, 27);
            ButtonMainGD.TabIndex = 3;
            ButtonMainGD.Text = "Select gamedata";
            ButtonMainGD.UseVisualStyleBackColor = true;
            ButtonMainGD.Click += ButtonMainGD_Click;
            // 
            // LabelGDMain
            // 
            LabelGDMain.AutoSize = true;
            LabelGDMain.Location = new System.Drawing.Point(153, 17);
            LabelGDMain.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelGDMain.Name = "LabelGDMain";
            LabelGDMain.Size = new System.Drawing.Size(0, 15);
            LabelGDMain.TabIndex = 4;
            // 
            // SaveGD
            // 
            SaveGD.FileName = "GameData_new.cff";
            SaveGD.Filter = "CFF files|*.cff";
            // 
            // SaveGamedataForm
            // 
            AcceptButton = ButtonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = ButtonCancel;
            ClientSize = new System.Drawing.Size(493, 82);
            Controls.Add(LabelGDMain);
            Controls.Add(ButtonMainGD);
            Controls.Add(ButtonOK);
            Controls.Add(ButtonCancel);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SaveGamedataForm";
            Text = "Save gamedata";
            Load += LoadGamedataForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonMainGD;
        private System.Windows.Forms.Label LabelGDMain;
        private System.Windows.Forms.SaveFileDialog SaveGD;
    }
}