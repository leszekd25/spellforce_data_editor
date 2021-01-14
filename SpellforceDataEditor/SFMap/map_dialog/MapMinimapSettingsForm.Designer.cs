namespace SpellforceDataEditor.SFMap.map_dialog
{
    partial class MapMinimapSettingsForm
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
            this.LabelOptionDescription = new System.Windows.Forms.Label();
            this.ButtonImportCustomMinimap = new System.Windows.Forms.Button();
            this.UseCustomMap = new System.Windows.Forms.RadioButton();
            this.UseEditorMap = new System.Windows.Forms.RadioButton();
            this.UseOriginalMap = new System.Windows.Forms.RadioButton();
            this.MinimapPicture = new System.Windows.Forms.PictureBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ImportMinimapDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinimapPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.LabelOptionDescription);
            this.panel1.Controls.Add(this.ButtonImportCustomMinimap);
            this.panel1.Controls.Add(this.UseCustomMap);
            this.panel1.Controls.Add(this.UseEditorMap);
            this.panel1.Controls.Add(this.UseOriginalMap);
            this.panel1.Location = new System.Drawing.Point(218, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(238, 200);
            this.panel1.TabIndex = 0;
            // 
            // LabelOptionDescription
            // 
            this.LabelOptionDescription.AutoEllipsis = true;
            this.LabelOptionDescription.Location = new System.Drawing.Point(3, 112);
            this.LabelOptionDescription.Name = "LabelOptionDescription";
            this.LabelOptionDescription.Size = new System.Drawing.Size(232, 78);
            this.LabelOptionDescription.TabIndex = 5;
            // 
            // ButtonImportCustomMinimap
            // 
            this.ButtonImportCustomMinimap.Enabled = false;
            this.ButtonImportCustomMinimap.Location = new System.Drawing.Point(160, 46);
            this.ButtonImportCustomMinimap.Name = "ButtonImportCustomMinimap";
            this.ButtonImportCustomMinimap.Size = new System.Drawing.Size(75, 23);
            this.ButtonImportCustomMinimap.TabIndex = 4;
            this.ButtonImportCustomMinimap.Text = "Import...";
            this.ButtonImportCustomMinimap.UseVisualStyleBackColor = true;
            this.ButtonImportCustomMinimap.Click += new System.EventHandler(this.ButtonImportCustomMinimap_Click);
            // 
            // UseCustomMap
            // 
            this.UseCustomMap.AutoSize = true;
            this.UseCustomMap.Location = new System.Drawing.Point(3, 49);
            this.UseCustomMap.Name = "UseCustomMap";
            this.UseCustomMap.Size = new System.Drawing.Size(122, 17);
            this.UseCustomMap.TabIndex = 3;
            this.UseCustomMap.Text = "Import custom image";
            this.UseCustomMap.UseVisualStyleBackColor = true;
            this.UseCustomMap.Click += new System.EventHandler(this.CustomMap_Click);
            // 
            // UseEditorMap
            // 
            this.UseEditorMap.AutoSize = true;
            this.UseEditorMap.Location = new System.Drawing.Point(3, 26);
            this.UseEditorMap.Name = "UseEditorMap";
            this.UseEditorMap.Size = new System.Drawing.Size(114, 17);
            this.UseEditorMap.TabIndex = 2;
            this.UseEditorMap.Text = "Use editor minimap";
            this.UseEditorMap.UseVisualStyleBackColor = true;
            this.UseEditorMap.Click += new System.EventHandler(this.UseEditorMap_Click);
            // 
            // UseOriginalMap
            // 
            this.UseOriginalMap.AutoSize = true;
            this.UseOriginalMap.Location = new System.Drawing.Point(3, 3);
            this.UseOriginalMap.Name = "UseOriginalMap";
            this.UseOriginalMap.Size = new System.Drawing.Size(121, 17);
            this.UseOriginalMap.TabIndex = 1;
            this.UseOriginalMap.Text = "Use original minimap";
            this.UseOriginalMap.UseVisualStyleBackColor = true;
            this.UseOriginalMap.Click += new System.EventHandler(this.UseOriginalMap_Click);
            // 
            // MinimapPicture
            // 
            this.MinimapPicture.Location = new System.Drawing.Point(12, 12);
            this.MinimapPicture.Name = "MinimapPicture";
            this.MinimapPicture.Size = new System.Drawing.Size(200, 200);
            this.MinimapPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MinimapPicture.TabIndex = 0;
            this.MinimapPicture.TabStop = false;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 218);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 1;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(381, 218);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 2;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // MapMinimapSettingsForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(468, 244);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.MinimapPicture);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapMinimapSettingsForm";
            this.Text = "Minimap settings";
            this.Shown += new System.EventHandler(this.MapMinimapSettingsForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinimapPicture)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox MinimapPicture;
        private System.Windows.Forms.Label LabelOptionDescription;
        private System.Windows.Forms.Button ButtonImportCustomMinimap;
        private System.Windows.Forms.RadioButton UseCustomMap;
        private System.Windows.Forms.RadioButton UseEditorMap;
        private System.Windows.Forms.RadioButton UseOriginalMap;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.OpenFileDialog ImportMinimapDialog;
    }
}