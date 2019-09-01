namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorLakeControl
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
            this.SelectedLakeType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LabelError = new System.Windows.Forms.Label();
            this.SelectedLakePanel = new System.Windows.Forms.Panel();
            this.SelectedLakeInternalDepth = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SelectedLakePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectedLakeType
            // 
            this.SelectedLakeType.FormattingEnabled = true;
            this.SelectedLakeType.Items.AddRange(new object[] {
            "Water",
            "Swamp",
            "Lava",
            "Ice"});
            this.SelectedLakeType.Location = new System.Drawing.Point(106, 33);
            this.SelectedLakeType.Name = "SelectedLakeType";
            this.SelectedLakeType.Size = new System.Drawing.Size(77, 21);
            this.SelectedLakeType.TabIndex = 1;
            this.SelectedLakeType.Text = "Water";
            this.SelectedLakeType.SelectedIndexChanged += new System.EventHandler(this.SelectedLakeType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lake type";
            // 
            // LabelError
            // 
            this.LabelError.AutoSize = true;
            this.LabelError.Location = new System.Drawing.Point(15, 338);
            this.LabelError.Name = "LabelError";
            this.LabelError.Size = new System.Drawing.Size(0, 13);
            this.LabelError.TabIndex = 7;
            // 
            // SelectedLakePanel
            // 
            this.SelectedLakePanel.Controls.Add(this.SelectedLakeInternalDepth);
            this.SelectedLakePanel.Controls.Add(this.label1);
            this.SelectedLakePanel.Controls.Add(this.label6);
            this.SelectedLakePanel.Controls.Add(this.SelectedLakeType);
            this.SelectedLakePanel.Enabled = false;
            this.SelectedLakePanel.Location = new System.Drawing.Point(18, 35);
            this.SelectedLakePanel.Name = "SelectedLakePanel";
            this.SelectedLakePanel.Size = new System.Drawing.Size(403, 170);
            this.SelectedLakePanel.TabIndex = 8;
            // 
            // SelectedLakeInternalDepth
            // 
            this.SelectedLakeInternalDepth.Enabled = false;
            this.SelectedLakeInternalDepth.Location = new System.Drawing.Point(106, 7);
            this.SelectedLakeInternalDepth.Name = "SelectedLakeInternalDepth";
            this.SelectedLakeInternalDepth.Size = new System.Drawing.Size(77, 20);
            this.SelectedLakeInternalDepth.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Internal depth";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Selected lake:";
            // 
            // MapInspectorLakeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SelectedLakePanel);
            this.Controls.Add(this.LabelError);
            this.Name = "MapInspectorLakeControl";
            this.SelectedLakePanel.ResumeLayout(false);
            this.SelectedLakePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox SelectedLakeType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabelError;
        private System.Windows.Forms.Panel SelectedLakePanel;
        private System.Windows.Forms.TextBox SelectedLakeInternalDepth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
    }
}
