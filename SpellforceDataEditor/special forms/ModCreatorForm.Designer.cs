namespace SpellforceDataEditor.special_forms
{
    partial class ModCreatorForm
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
            this.ButtonFinish = new System.Windows.Forms.Button();
            this.PanelInfo = new System.Windows.Forms.Panel();
            this.LabelModInfo = new System.Windows.Forms.Label();
            this.LabelBytecodeInfo = new System.Windows.Forms.Label();
            this.LabelAssetsInfo = new System.Windows.Forms.Label();
            this.LabelDataInfo = new System.Windows.Forms.Label();
            this.ButtonChooseAssets = new System.Windows.Forms.Button();
            this.OpenAssetDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.ButtonInfo = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.StatusBar = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.TextBoxFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelInfo.SuspendLayout();
            this.StatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonFinish
            // 
            this.ButtonFinish.Location = new System.Drawing.Point(230, 390);
            this.ButtonFinish.Name = "ButtonFinish";
            this.ButtonFinish.Size = new System.Drawing.Size(75, 23);
            this.ButtonFinish.TabIndex = 1;
            this.ButtonFinish.Text = "Create mod";
            this.ButtonFinish.UseVisualStyleBackColor = true;
            this.ButtonFinish.Click += new System.EventHandler(this.ButtonFinish_Click);
            // 
            // PanelInfo
            // 
            this.PanelInfo.Controls.Add(this.LabelModInfo);
            this.PanelInfo.Controls.Add(this.LabelBytecodeInfo);
            this.PanelInfo.Controls.Add(this.LabelAssetsInfo);
            this.PanelInfo.Controls.Add(this.LabelDataInfo);
            this.PanelInfo.Location = new System.Drawing.Point(12, 73);
            this.PanelInfo.Name = "PanelInfo";
            this.PanelInfo.Size = new System.Drawing.Size(293, 310);
            this.PanelInfo.TabIndex = 2;
            // 
            // LabelModInfo
            // 
            this.LabelModInfo.AutoEllipsis = true;
            this.LabelModInfo.Location = new System.Drawing.Point(3, 109);
            this.LabelModInfo.Name = "LabelModInfo";
            this.LabelModInfo.Size = new System.Drawing.Size(287, 176);
            this.LabelModInfo.TabIndex = 5;
            this.LabelModInfo.Text = "label1";
            // 
            // LabelBytecodeInfo
            // 
            this.LabelBytecodeInfo.AutoSize = true;
            this.LabelBytecodeInfo.Location = new System.Drawing.Point(3, 285);
            this.LabelBytecodeInfo.Name = "LabelBytecodeInfo";
            this.LabelBytecodeInfo.Size = new System.Drawing.Size(35, 13);
            this.LabelBytecodeInfo.TabIndex = 4;
            this.LabelBytecodeInfo.Text = "label1";
            // 
            // LabelAssetsInfo
            // 
            this.LabelAssetsInfo.AutoSize = true;
            this.LabelAssetsInfo.Location = new System.Drawing.Point(3, 58);
            this.LabelAssetsInfo.Name = "LabelAssetsInfo";
            this.LabelAssetsInfo.Size = new System.Drawing.Size(35, 13);
            this.LabelAssetsInfo.TabIndex = 3;
            this.LabelAssetsInfo.Text = "label1";
            // 
            // LabelDataInfo
            // 
            this.LabelDataInfo.AutoSize = true;
            this.LabelDataInfo.Location = new System.Drawing.Point(3, 9);
            this.LabelDataInfo.Name = "LabelDataInfo";
            this.LabelDataInfo.Size = new System.Drawing.Size(35, 13);
            this.LabelDataInfo.TabIndex = 2;
            this.LabelDataInfo.Text = "label1";
            // 
            // ButtonChooseAssets
            // 
            this.ButtonChooseAssets.Location = new System.Drawing.Point(12, 44);
            this.ButtonChooseAssets.Name = "ButtonChooseAssets";
            this.ButtonChooseAssets.Size = new System.Drawing.Size(130, 23);
            this.ButtonChooseAssets.TabIndex = 0;
            this.ButtonChooseAssets.Text = "Choose assets directory";
            this.ButtonChooseAssets.UseVisualStyleBackColor = true;
            this.ButtonChooseAssets.Click += new System.EventHandler(this.ButtonChooseAssets_Click);
            // 
            // ButtonInfo
            // 
            this.ButtonInfo.Location = new System.Drawing.Point(175, 44);
            this.ButtonInfo.Name = "ButtonInfo";
            this.ButtonInfo.Size = new System.Drawing.Size(130, 23);
            this.ButtonInfo.TabIndex = 5;
            this.ButtonInfo.Text = "Add mod information";
            this.ButtonInfo.UseVisualStyleBackColor = true;
            this.ButtonInfo.Click += new System.EventHandler(this.ButtonInfo_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(12, 389);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 6;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // StatusBar
            // 
            this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.StatusBar.Location = new System.Drawing.Point(0, 421);
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(317, 22);
            this.StatusBar.TabIndex = 7;
            this.StatusBar.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(39, 17);
            this.StatusText.Text = "Ready";
            // 
            // TextBoxFileName
            // 
            this.TextBoxFileName.Location = new System.Drawing.Point(97, 12);
            this.TextBoxFileName.Name = "TextBoxFileName";
            this.TextBoxFileName.Size = new System.Drawing.Size(100, 20);
            this.TextBoxFileName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Mod file name:";
            // 
            // ModCreatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(317, 443);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxFileName);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonInfo);
            this.Controls.Add(this.ButtonChooseAssets);
            this.Controls.Add(this.PanelInfo);
            this.Controls.Add(this.ButtonFinish);
            this.MaximizeBox = false;
            this.Name = "ModCreatorForm";
            this.Text = "Mod Creator";
            this.PanelInfo.ResumeLayout(false);
            this.PanelInfo.PerformLayout();
            this.StatusBar.ResumeLayout(false);
            this.StatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonFinish;
        private System.Windows.Forms.Panel PanelInfo;
        private System.Windows.Forms.Label LabelBytecodeInfo;
        private System.Windows.Forms.Label LabelAssetsInfo;
        private System.Windows.Forms.Label LabelDataInfo;
        private System.Windows.Forms.Button ButtonChooseAssets;
        private System.Windows.Forms.FolderBrowserDialog OpenAssetDirectory;
        private System.Windows.Forms.Label LabelModInfo;
        private System.Windows.Forms.Button ButtonInfo;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.StatusStrip StatusBar;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.TextBox TextBoxFileName;
        private System.Windows.Forms.Label label1;
    }
}