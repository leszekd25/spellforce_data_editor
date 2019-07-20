namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorHeightMapControl
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
            this.StrengthTextBox = new System.Windows.Forms.TextBox();
            this.LabelStrength = new System.Windows.Forms.Label();
            this.ComboDrawMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BrushControl = new SpellforceDataEditor.SFMap.map_controls.MapBrushControl();
            this.SuspendLayout();
            // 
            // StrengthTextBox
            // 
            this.StrengthTextBox.Location = new System.Drawing.Point(115, 166);
            this.StrengthTextBox.Name = "StrengthTextBox";
            this.StrengthTextBox.Size = new System.Drawing.Size(77, 20);
            this.StrengthTextBox.TabIndex = 9;
            this.StrengthTextBox.Text = "20";
            // 
            // LabelStrength
            // 
            this.LabelStrength.AutoSize = true;
            this.LabelStrength.Location = new System.Drawing.Point(14, 169);
            this.LabelStrength.Name = "LabelStrength";
            this.LabelStrength.Size = new System.Drawing.Size(47, 13);
            this.LabelStrength.TabIndex = 8;
            this.LabelStrength.Text = "Strength";
            // 
            // ComboDrawMode
            // 
            this.ComboDrawMode.FormattingEnabled = true;
            this.ComboDrawMode.Items.AddRange(new object[] {
            "Add",
            "Subtract",
            "Set",
            "Smooth",
            "Rough"});
            this.ComboDrawMode.Location = new System.Drawing.Point(115, 139);
            this.ComboDrawMode.Name = "ComboDrawMode";
            this.ComboDrawMode.Size = new System.Drawing.Size(77, 21);
            this.ComboDrawMode.TabIndex = 7;
            this.ComboDrawMode.Text = "Add";
            this.ComboDrawMode.SelectedIndexChanged += new System.EventHandler(this.ComboDrawMode_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Draw mode";
            // 
            // BrushControl
            // 
            this.BrushControl.Location = new System.Drawing.Point(3, 3);
            this.BrushControl.Name = "BrushControl";
            this.BrushControl.Size = new System.Drawing.Size(310, 124);
            this.BrushControl.TabIndex = 5;
            // 
            // MapInspectorHeightMapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.StrengthTextBox);
            this.Controls.Add(this.LabelStrength);
            this.Controls.Add(this.ComboDrawMode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BrushControl);
            this.Name = "MapInspectorHeightMapControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox StrengthTextBox;
        private System.Windows.Forms.Label LabelStrength;
        private System.Windows.Forms.ComboBox ComboDrawMode;
        private System.Windows.Forms.Label label1;
        private MapBrushControl BrushControl;
    }
}
