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
            this.BrushControl = new SpellforceDataEditor.SFMap.map_controls.MapBrushControl();
            this.TypeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DepthTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SlopeTextBox = new System.Windows.Forms.TextBox();
            this.LabelError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BrushControl
            // 
            this.BrushControl.Location = new System.Drawing.Point(3, 82);
            this.BrushControl.Name = "BrushControl";
            this.BrushControl.Size = new System.Drawing.Size(310, 124);
            this.BrushControl.TabIndex = 0;
            // 
            // TypeCombo
            // 
            this.TypeCombo.FormattingEnabled = true;
            this.TypeCombo.Items.AddRange(new object[] {
            "Water",
            "Lava",
            "Swamp"});
            this.TypeCombo.Location = new System.Drawing.Point(115, 3);
            this.TypeCombo.Name = "TypeCombo";
            this.TypeCombo.Size = new System.Drawing.Size(77, 21);
            this.TypeCombo.TabIndex = 1;
            this.TypeCombo.Text = "Water";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Lake type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lake depth";
            // 
            // DepthTextBox
            // 
            this.DepthTextBox.Location = new System.Drawing.Point(115, 30);
            this.DepthTextBox.Name = "DepthTextBox";
            this.DepthTextBox.Size = new System.Drawing.Size(77, 20);
            this.DepthTextBox.TabIndex = 4;
            this.DepthTextBox.Text = "150";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Slope strength";
            // 
            // SlopeTextBox
            // 
            this.SlopeTextBox.Location = new System.Drawing.Point(115, 56);
            this.SlopeTextBox.Name = "SlopeTextBox";
            this.SlopeTextBox.Size = new System.Drawing.Size(77, 20);
            this.SlopeTextBox.TabIndex = 6;
            this.SlopeTextBox.Text = "50";
            // 
            // LabelError
            // 
            this.LabelError.AutoSize = true;
            this.LabelError.Location = new System.Drawing.Point(15, 338);
            this.LabelError.Name = "LabelError";
            this.LabelError.Size = new System.Drawing.Size(0, 13);
            this.LabelError.TabIndex = 7;
            // 
            // MapInspectorLakeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.LabelError);
            this.Controls.Add(this.SlopeTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.DepthTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TypeCombo);
            this.Controls.Add(this.BrushControl);
            this.Name = "MapInspectorLakeControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapBrushControl BrushControl;
        private System.Windows.Forms.ComboBox TypeCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DepthTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SlopeTextBox;
        private System.Windows.Forms.Label LabelError;
    }
}
