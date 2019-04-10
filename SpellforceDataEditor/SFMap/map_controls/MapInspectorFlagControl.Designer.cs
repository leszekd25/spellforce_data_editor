namespace SpellforceDataEditor.SFMap.map_controls
{
    partial class MapInspectorFlagControl
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
            this.CheckMovement = new System.Windows.Forms.CheckBox();
            this.CheckVision = new System.Windows.Forms.CheckBox();
            this.ComboFlagMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BrushControl
            // 
            this.BrushControl.Location = new System.Drawing.Point(3, 49);
            this.BrushControl.Name = "BrushControl";
            this.BrushControl.Size = new System.Drawing.Size(310, 124);
            this.BrushControl.TabIndex = 0;
            // 
            // CheckMovement
            // 
            this.CheckMovement.AutoSize = true;
            this.CheckMovement.Location = new System.Drawing.Point(18, 3);
            this.CheckMovement.Name = "CheckMovement";
            this.CheckMovement.Size = new System.Drawing.Size(130, 17);
            this.CheckMovement.TabIndex = 1;
            this.CheckMovement.Text = "Show movement flags";
            this.CheckMovement.UseVisualStyleBackColor = true;
            this.CheckMovement.CheckedChanged += new System.EventHandler(this.CheckMovement_CheckedChanged);
            // 
            // CheckVision
            // 
            this.CheckVision.AutoSize = true;
            this.CheckVision.Location = new System.Drawing.Point(18, 26);
            this.CheckVision.Name = "CheckVision";
            this.CheckVision.Size = new System.Drawing.Size(108, 17);
            this.CheckVision.TabIndex = 2;
            this.CheckVision.Text = "Show vision flags";
            this.CheckVision.UseVisualStyleBackColor = true;
            this.CheckVision.CheckedChanged += new System.EventHandler(this.CheckVision_CheckedChanged);
            // 
            // ComboFlagMode
            // 
            this.ComboFlagMode.FormattingEnabled = true;
            this.ComboFlagMode.Items.AddRange(new object[] {
            "Movement",
            "Vision"});
            this.ComboFlagMode.Location = new System.Drawing.Point(115, 179);
            this.ComboFlagMode.Name = "ComboFlagMode";
            this.ComboFlagMode.Size = new System.Drawing.Size(77, 21);
            this.ComboFlagMode.TabIndex = 3;
            this.ComboFlagMode.Text = "Movement";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 182);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Flag";
            // 
            // MapInspectorFlagControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComboFlagMode);
            this.Controls.Add(this.CheckVision);
            this.Controls.Add(this.CheckMovement);
            this.Controls.Add(this.BrushControl);
            this.Name = "MapInspectorFlagControl";
            this.VisibleChanged += new System.EventHandler(this.MapInspectorFlagControl_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MapBrushControl BrushControl;
        private System.Windows.Forms.CheckBox CheckMovement;
        private System.Windows.Forms.CheckBox CheckVision;
        private System.Windows.Forms.ComboBox ComboFlagMode;
        private System.Windows.Forms.Label label1;
    }
}
